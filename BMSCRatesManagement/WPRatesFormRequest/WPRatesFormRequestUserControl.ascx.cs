using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BMSCRatesManagement.WPRatesFormRequest
{
    public partial class WPRatesFormRequestUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblExecutive.Text = Microsoft.SharePoint.SPContext.Current.Web.CurrentUser.Name;

            txbFisa.Focus();
        }

        protected void rblCreditType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);

            try
            {
                ddlSegmentType.Focus();

                ddlSegmentType.DataSource = SharePointConnector.GetSegments(rblCreditType.SelectedValue);
                ddlSegmentType.DataTextField = "NameAndType";
                ddlSegmentType.DataValueField = "Id";
                ddlSegmentType.DataBind();
                ddlSegmentType.SelectedIndex = 0;

                ddlProduct.DataSource = SharePointConnector.GetProducts(rblCreditType.SelectedValue, txbClientBirthdate.Text, null);
                ddlProduct.DataBind();
                ddlProduct.Items.Insert(0, new ListItem("(Seleccione el producto)", string.Empty));
                ddlProduct.SelectedIndex = 0;

                ddlProductType.SelectedIndex = 0;
                ddlWarranty.SelectedIndex = 0;
                ddlTerm.SelectedIndex = 0;
                ddlMargin.SelectedIndex = 0;

                #region Forcing initial validation for later asignments
                rfvClientName.Validate();
                rfvClientCode.Validate();
                cmvClientCode.Validate();
                rfvClientBirthdate.Validate();
                cmvClientBirthdate.Validate();

                if (rfvClientName.IsValid &&
                    rfvClientCode.IsValid && cmvClientCode.IsValid &&
                    rfvClientBirthdate.IsValid && cmvClientBirthdate.IsValid)
                {
                    txbClientName.Enabled = false;
                    txbClientCode.Enabled = false;
                    txbClientBirthdate.Enabled = false;

                    ddlProduct.Enabled = true;
                }
                else
                {
                    ddlProduct.Enabled = false;
                }
                #endregion
            }
            catch(Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void ddlProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);

            try
            {
                if (SharePointConnector.HasForeignCurrency(
                    ddlProduct.SelectedValue,
                    rblCreditType.SelectedValue,
                    ddlSegmentType.SelectedItem.Value))
                {
                    rblCurrency.Enabled = true;
                    rblCurrency.Focus();
                }
                else
                {
                    rblCurrency.Enabled = false;
                    rblCurrency.SelectedIndex = 0;
                    ddlProductType.Focus();
                }

                ddlProductType.DataSource = SharePointConnector.GetProductTypes(ddlProduct.SelectedValue, rblCreditType.SelectedValue);
                ddlProductType.DataTextField = "Name";
                ddlProductType.DataValueField = "Id";
                ddlProductType.DataBind();
                ddlProductType.Items.Insert(0, new ListItem("(Seleccione el tipo de producto)", string.Empty));
                ddlProductType.SelectedIndex = 0;

                ddlWarranty.DataSource = SharePointConnector.GetWarranties(ddlProduct.SelectedValue, rblCreditType.SelectedValue);
                ddlWarranty.DataTextField = "Name";
                ddlWarranty.DataValueField = "Id";
                ddlWarranty.DataBind();
                ddlWarranty.Items.Insert(0, new ListItem("(Seleccione la garantía)", string.Empty));
                ddlWarranty.SelectedIndex = 0;

                ddlTerm.DataSource = SharePointConnector.GetTerms(ddlProduct.SelectedValue, rblCreditType.SelectedValue, txbClientBirthdate.Text, null);
                ddlTerm.DataTextField = "Display";
                ddlTerm.DataValueField = "Name";
                ddlTerm.DataBind();
                ddlTerm.Items.Insert(0, new ListItem("(Seleccione el plazo)", string.Empty));
                ddlTerm.SelectedIndex = 0;

                ddlPeriodFixed.DataSource = SharePointConnector.GetPeriods(ddlProduct.SelectedValue, rblCreditType.SelectedValue, txbClientBirthdate.Text, null);
                ddlPeriodFixed.DataTextField = "Display";
                ddlPeriodFixed.DataValueField = "Name";
                ddlPeriodFixed.DataBind();
                ddlPeriodFixed.Items.Insert(0, new ListItem("(Seleccione el período)", string.Empty));
                ddlPeriodFixed.SelectedIndex = 0;

                ddlMargin.DataSource = SharePointConnector.GetMargins(ddlProduct.SelectedValue, ddlSegmentType.SelectedItem.Value, rblCreditType.SelectedValue);
                ddlMargin.DataTextField = "Name";
                ddlMargin.DataValueField = "Id";
                ddlMargin.DataBind();
                ddlMargin.Items.Insert(0, new ListItem("(Seleccione el margen de negociación)", string.Empty));
                ddlMargin.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void btnShowRates_Click(object sender, EventArgs e)
        {
            try
            {
                pnlForm.Visible = false;
                pnlPrint.Visible = true;

                lblMessage.Visible = false;
                btnPrint.Visible = false;
                btnSaveAndPrint.Visible = true;

                Rate theRate = SharePointConnector.GetRates(
                    rblCreditType.SelectedValue,
                    int.Parse(ddlSegmentType.SelectedItem.Value),
                    ddlProduct.SelectedValue,
                    ddlProductType.SelectedItem.Text,
                    SharePointConnector.IsBanxClient(txbClientBirthdate.Text),
                    SharePointConnector.IsDisabledClient(rblDisabled.SelectedValue),
                    SharePointConnector.IsCpopClient(rblCpop.SelectedValue),
                    rblSector.SelectedValue,
                    txbIvc.Text,
                    txbIrt.Text,
                    ddlMargin.SelectedItem.Text,
                    ddlWarranty.SelectedItem.Text,
                    ddlTerm.SelectedItem.Value,
                    ddlPeriodFixed.SelectedItem.Value,
                    rblCurrency.SelectedValue);

                if (theRate.RateFixed > 50.0)
                {
                    ltrMessage.Text = "<table class='print' style='width:100%;padding:3em 12em;'>" +
                        "<tr><td><span class='number' style='padding:20px;'>" +
                        "LA TASA SOLICITADA NO APLICA.</span></td></tr></table>";

                    tblPrint.Visible = false;
                    btnSaveAndPrint.Visible = false;

                    return;
                }
                else if (rblCreditType.SelectedValue == "PERSONAL" &&
                    ddlProductType.SelectedItem.Text == "VISA INTERNACIONAL PAYBANX" &&
                    ddlWarranty.SelectedItem.Text.StartsWith("NO DEBIDAMENTE GARANTIZADO", StringComparison.CurrentCultureIgnoreCase))
                {
                    ltrMessage.Text = "<table class='print' style='width:100%;padding:3em 12em;'>" +
                        "<tr><td><span class='number' style='padding:20px;'>" +
                        "LA TASA SOLICITADA NO APLICA.</span></td></tr></table>";

                    tblPrint.Visible = false;
                    btnSaveAndPrint.Visible = false;
                }
                else
                {
                    ltrMessage.Text = "";
                    tblPrint.Visible = true;
                    btnSaveAndPrint.Visible = true;
                }

                #region Make the assignments
                lblFisa.Text = txbFisa.Text;
                lblBank.Text = rblBank.SelectedValue;
                lblClientCode.Text = txbClientCode.Text;
                lblClientName.Text = txbClientName.Text;
                lblBanx.Text = SharePointConnector.IsBanxClient(txbClientBirthdate.Text) ? "SI" : "NO";
                lblSector.Text = rblSector.SelectedValue;
                lblSegmentType.Text = ddlSegmentType.SelectedItem.Text;
                lblSegmentLabel.Text =
                    (rblCreditType.SelectedValue == "PERSONAL" ? "Segmento" : "Tamaño de Actividad") + " | Tipo de Segmento";
                lblCpop.Text = rblCpop.SelectedValue;
                lblDisabled.Text = rblDisabled.SelectedValue;
                lblIvc.Text = string.IsNullOrEmpty(txbIvc.Text) ? "(sin valor)" : txbIvc.Text;
                lblIrt.Text = string.IsNullOrEmpty(txbIrt.Text) ? "(sin valor)" : txbIrt.Text; ;
                lblProduct.Text = rblCreditType.SelectedValue + " | " + ddlProduct.SelectedValue;
                lblProductType.Text = ddlProductType.SelectedItem.Text;
                lblWarranty.Text = ddlWarranty.SelectedItem.Text;
                lblTerm.Text = ddlTerm.SelectedItem.Text;
                lblPeriodFixed.Text = ddlPeriodFixed.SelectedItem.Text;
                lblCurrency.Text = rblCurrency.SelectedValue;
                lblMargin.Text = ddlMargin.SelectedItem.Text;

                if (rblCreditType.SelectedValue != "PERSONAL" &&
                    !ddlProduct.SelectedValue.Contains("NO PRODUCTIVO") &&
                    ddlProduct.SelectedValue.Contains("PRODUCTIVO"))
                {
                    theRate.RateVariable = 100;
                }

                Int16 period, term;
                if (Int16.TryParse(ddlPeriodFixed.SelectedItem.Value, out period) &&
                    Int16.TryParse(ddlTerm.SelectedItem.Value, out term) &&
                    period < term &&
                    theRate.RateVariable < 50)
                {//if 'Periodo Fijo' (months) is a number & is smaller than 'Plazo' (months)
                    lblRate.Text =
                        "<span class='number'>" + string.Format("{0:0.00}", theRate.RateFixed) + "%</span> fijo los primeros " +
                        "<span class='number'>" + period + " meses</span>,<br/>" +
                        "<span class='number'>" + string.Format("{0:0.00}", theRate.RateVariable) + "%</span> + TRE a partir del " +
                        "<span class='number'>mes " + (period + 1) + "</span>.";
                }
                else
                {
                    lblRate.Text =
                        "<span class='number'>" + string.Format("{0:0.00}", theRate.RateFixed) + "%</span> todo el período del crédito.<br/>";
                }

                if (lblCpop.Text == "SI")
                {
                    if (!string.IsNullOrEmpty(theRate.CpopBenefit))
                        lblCpopBenefit.Text = theRate.CpopBenefit;
                    else if (theRate.CpopValue != 0.0)
                        lblCpopBenefit.Text = "Reducción de tasa (ya incluida en la tasa calculada).";
                    else
                        lblCpopBenefit.Text = "(no disponible)";
                }
                else
                { 
                    lblCpopBenefit.Text = "(no disponible)";
                }

                if (lblDisabled.Text == "SI")
                {
                    if (!string.IsNullOrEmpty(theRate.DisabledBenefit))
                        lblDisabledBenefit.Text = theRate.DisabledBenefit;
                    else if (theRate.DisabledValue != 0.0)
                        lblDisabledBenefit.Text = "Reducción de tasa (ya incluida en la tasa calculada).";
                    else
                        lblDisabledBenefit.Text = "(no disponible)";
                }
                else
                {
                    lblDisabledBenefit.Text = "(no disponible)";
                }
                #endregion
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = true;
            pnlPrint.Visible = false;
        }

        protected void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Visible = true;
                btnPrint.Visible = true;
                btnSaveAndPrint.Visible = false;

                SharePointConnector.SaveRate(
                    lblFisa.Text, lblExecutive.Text,
                    lblBank.Text,
                    lblClientName.Text, lblClientCode.Text,
                    lblCpop.Text, lblBanx.Text, lblDisabled.Text,
                    lblSector.Text,
                    lblSegmentType.Text,
                    lblIvc.Text, lblIrt.Text,
                    lblProduct.Text,
                    lblProductType.Text,
                    lblWarranty.Text,
                    lblTerm.Text,
                    lblPeriodFixed.Text,
                    lblCurrency.Text,
                    lblMargin.Text,
                    //System.Text.RegularExpressions.Regex.Replace(lblRate.Text, @"<[^>]+>|&nbsp;", "").Trim(),
                    lblRate.Text,
                    lblCpopBenefit.Text,
                    lblDisabledBenefit.Text);
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }
    }
}
