using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BMSCRatesManagement.WPAsfiRatesReport
{
    public partial class WPAsfiRatesReportUserControl : UserControl
    {
        bool isUserContentEditor;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            isUserContentEditor = SharePointConnector.IsContentEditor();
        }

        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            if (isUserContentEditor)
                btnExportReport.Visible = true;

            try
            {
                string[] creditTypesList = new string[] { "PERSONAL", "PYME", "EMPRESARIAL" };
                dgrReport.DataSource = SharePointConnector.GetReportTableRates(creditTypesList, rblCurrency.SelectedValue);
                dgrReport.DataBind();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=BMSCResumenTasasASFI.csv");
                Response.Charset = "";
                Response.ContentType = "text/csv";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("MONEDA;");
                sb.Append("CREDITO;");
                sb.Append("PRODUCTO;");
                sb.Append("TIPO PRODUCTO;");
                sb.Append("TF;");
                sb.Append("TV");
                sb.Append("\r\n");

                for (int i = 0; i < dgrReport.Items.Count; i++)
                {
                    sb.Append(rblCurrency.SelectedValue + ";");

                    for (int k = 0; k < dgrReport.Items[i].Cells.Count; k++)
                    {
                        string text = dgrReport.Items[i].Cells[k].Text;

                        byte[] bytes = Encoding.Default.GetBytes(text);
                        text = Encoding.Default.GetString(bytes);

                        if (k == dgrReport.Items[i].Cells.Count - 1)
                            sb.Append(text);
                        else
                            sb.Append(text + ';');
                    }

                    sb.Append("\r\n");
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.Close();
                Response.End();
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
