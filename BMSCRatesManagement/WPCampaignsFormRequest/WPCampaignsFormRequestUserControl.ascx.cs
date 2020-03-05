using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BMSCRatesManagement.WPCampaignsFormRequest
{
    public partial class WPCampaignsFormRequestUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblExecutive.Text = Microsoft.SharePoint.SPContext.Current.Web.CurrentUser.Name;

            txbFisa.Focus();
        }

        protected void btnCampaignSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblCampaignSearch.Text = "";
                List<Campaign> campaignsList = SharePointConnector.GetCampaigns(txbCampaign.Text.Trim());

                //System.Threading.Thread.Sleep(1000);

                if (campaignsList.Count == 0)
                    lblCampaignSearch.Text = "No existen nombres de campañas que contengan los caracteres ingresados.<br/>Puede que la campaña buscada ya no esté vigente.";
                else
                    lsbCampaign.Enabled = true;

                lsbCampaign.DataSource = campaignsList;
                lsbCampaign.DataTextField = "CampaignName";
                lsbCampaign.DataValueField = "CampaignImage";
                lsbCampaign.DataBind();

                txbCampaign.Focus();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void btnShowCampaign_Click(object sender, EventArgs e)
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
                lblClientBirthdate.Text = SharePointConnector.IsBanxClient(txbClientBirthdate.Text) ? "SI": "NO";
                ltrCampaignImage.Text = string.Format(
                    "<img src='{0}' alt='' />",
                    lsbCampaign.SelectedValue);
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
                SharePointConnector.SaveCampaign(lblFisa.Text, lblCreditType.Text, lblClientCode.Text,
                    lblClientName.Text, lblCpop.Text + " | " + lblClientBirthdate.Text + " | " + lblDisabled.Text,
                    lsbCampaign.SelectedValue);

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
