using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BMSCRatesManagement.WPAgreementsFormRequest
{
    public partial class WPAgreementsFormRequestUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblExecutive.Text = Microsoft.SharePoint.SPContext.Current.Web.CurrentUser.Name;

            txbFisa.Focus();
        }

        protected void btnAgreementSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblAgreementSearch.Text = "";
                List<Agreement> agreementsList = SharePointConnector.GetAgreements(txbAgreement.Text.Trim());

                //System.Threading.Thread.Sleep(1000);

                if (agreementsList.Count == 0)
                    lblAgreementSearch.Text = "No existen nombres de convenios que contengan los caracteres ingresados.";
                else
                    lsbAgreement.Enabled = true;

                lsbAgreement.DataSource = agreementsList;
                lsbAgreement.DataTextField = "AgreementName";
                lsbAgreement.DataValueField = "AgreementImage";
                lsbAgreement.DataBind();

                txbAgreement.Focus();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void btnShowAgreement_Click(object sender, EventArgs e)
        {
            try
            {
                pnlForm.Visible = false;
                pnlPrint.Visible = true;

                lblMessage.Visible = false;
                btnPrint.Visible = false;
                btnSaveAndPrint.Visible = true;

                lblFisa.Text = txbFisa.Text.Trim();
                lblCreditType.Text = rblCreditType.SelectedValue;
                lblClientCode.Text = txbClientCode.Text.Trim();
                lblClientName.Text = txbClientName.Text.Trim();
                lblCpop.Text = rblCpop.SelectedValue;
                lblDisabled.Text = rblDisabled.SelectedValue;
                lblClientBirthdate.Text = SharePointConnector.IsBanxClient(txbClientBirthdate.Text) ? "SI" : "NO";
                ltrAgreementImage.Text = string.Format(
                    "<img src='{0}' alt='' />",
                    lsbAgreement.SelectedValue);
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
                SharePointConnector.SaveAgreement(lblFisa.Text, lblCreditType.Text, lblClientCode.Text,
                    lblClientName.Text, lblCpop.Text + " | " + lblClientBirthdate.Text + " | " + lblDisabled.Text,
                    lsbAgreement.SelectedValue);

                lblMessage.Visible = true;
                btnPrint.Visible = true;
                btnSaveAndPrint.Visible = false;
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
