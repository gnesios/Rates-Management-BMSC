using System.Data;

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Text;

namespace BMSCRatesManagement.WPMaxRatesReport
{
    public partial class WPMaxRatesReportUserControl : UserControl
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

        protected void rblBanx_SelectedIndexChanged(object sender, EventArgs e)
        {
            ltrMessage.Text = "";

            cbxSegment.Checked = false;

            rblCreditType.ClearSelection();
            cblSegment.Items.Clear();
            rblProduct.Items.Clear();
            cblProductType.Items.Clear();
            cblWarranty.Items.Clear();
            cblTerm.Items.Clear();
            cblPeriod.Items.Clear();
        }

        protected void rblCreditType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ltrMessage.Text = "";

            cbxSegment.Checked = false;

            cblProductType.Items.Clear();
            cblWarranty.Items.Clear();
            cblTerm.Items.Clear();
            cblPeriod.Items.Clear();
            
            try
            {
                rblProduct.DataSource = SharePointConnector.GetProducts(rblCreditType.SelectedValue, null, rblBanx.SelectedValue);
                rblProduct.DataBind();
                
                cblSegment.DataSource = SharePointConnector.GetSegments(rblCreditType.SelectedValue);
                cblSegment.DataTextField = "NameTypeLevel";
                cblSegment.DataValueField = "Id";
                cblSegment.DataBind();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                ltrMessage.Text = ex.Message;
            }
        }

        protected void cblProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ltrMessage.Text = "";

            cbxProductType.Checked = false;
            cbxWarranty.Checked = false;
            cbxTerm.Checked = false;
            cbxPeriod.Checked = false;

            cblProductType.Items.Clear();
            cblWarranty.Items.Clear();
            cblTerm.Items.Clear();
            cblPeriod.Items.Clear();

            try
            {
                foreach (ListItem item in rblProduct.Items)
                {
                    if (item.Selected)
                    {
                        List<Product> products =
                            SharePointConnector.GetProductTypes(item.Value, rblCreditType.SelectedValue);
                        List<Warranty> warranties =
                            SharePointConnector.GetWarranties(item.Value, rblCreditType.SelectedValue);
                        List<Term> terms =
                            SharePointConnector.GetTerms(item.Value, rblCreditType.SelectedValue, null, rblBanx.SelectedValue);
                        List<Period> periods =
                            SharePointConnector.GetPeriods(item.Value, rblCreditType.SelectedValue, null, rblBanx.SelectedValue);

                        cblProductType.Items.Add(new ListItem(item.Value, "", false));
                        cblWarranty.Items.Add(new ListItem(item.Value, "", false));
                        cblTerm.Items.Add(new ListItem(item.Value, "", false));
                        cblPeriod.Items.Add(new ListItem(item.Value, "", false));

                        foreach (Product product in products)
                        {
                            cblProductType.Items.Add(new ListItem(product.Name, product.Id.ToString()));
                        }
                        foreach (Warranty warranty in warranties)
                        {
                            cblWarranty.Items.Add(new ListItem(warranty.Name, warranty.Id.ToString()));
                        }
                        foreach (Term term in terms)
                        {
                            cblTerm.Items.Add(new ListItem(term.Display, term.Name));
                        }
                        foreach (Period period in periods)
                        {
                            cblPeriod.Items.Add(new ListItem(period.Display, period.Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl errorMessage = new System.Web.UI.LiteralControl();
                errorMessage.Text = ex.Message;

                ltrMessage.Text = ex.Message;
            }
        }

        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            if (rblReportType.SelectedIndex != -1 && rblCreditType.SelectedIndex != -1 &&
                rblCurrency.SelectedIndex != -1 && rblBanx.SelectedIndex != -1 &&
                cblSegment.SelectedIndex != -1 && rblProduct.SelectedIndex != -1 &&
                cblProductType.SelectedIndex != -1 && cblWarranty.SelectedIndex != -1 &&
                cblTerm.SelectedIndex != -1 && cblPeriod.SelectedIndex != -1)
            {
                if (isUserContentEditor)
                    btnExportReport.Visible = true;

                dgrReport.Visible = true;
                ltrMessage.Text = "";

                List<string[]> reportTypeList = new List<string[]>();
                List<string[]> currencyList = new List<string[]>();
                List<string[]> banxList = new List<string[]>();
                List<string[]> creditTypeList = new List<string[]>();
                List<string[]> segmentList = new List<string[]>();
                List<string[]> productList = new List<string[]>();
                List<string[]> productTypeList = new List<string[]>();
                List<string[]> warrantyList = new List<string[]>();
                List<string[]> termList = new List<string[]>();
                List<string[]> periodList = new List<string[]>();

                foreach (ListItem item in rblReportType.Items)
                {
                    if (item.Selected)
                        reportTypeList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in rblCurrency.Items)
                {
                    if (item.Selected)
                        currencyList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in rblBanx.Items)
                {
                    if (item.Selected)
                        banxList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in rblCreditType.Items)
                {
                    if (item.Selected)
                        creditTypeList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in cblSegment.Items)
                {
                    if (item.Selected)
                    {
                        string itemText = item.Text.Remove(item.Text.LastIndexOf('|') - 1);
                        string itemLevel = item.Text.Substring(item.Text.LastIndexOf('|') + 2);

                        segmentList.Add(new string[] { itemText, item.Value, itemLevel });
                    }
                }
                foreach (ListItem item in rblProduct.Items)
                {
                    if (item.Selected)
                        productList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in cblProductType.Items)
                {
                    if (item.Selected)
                        productTypeList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in cblWarranty.Items)
                {
                    if (item.Selected)
                        warrantyList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in cblTerm.Items)
                {
                    if (item.Selected)
                        termList.Add(new string[] { item.Text, item.Value });
                }
                foreach (ListItem item in cblPeriod.Items)
                {
                    if (item.Selected)
                        periodList.Add(new string[] { item.Text, item.Value });
                }

                dgrReport.DataSource = SharePointConnector.GetReportTableRates(
                    reportTypeList, currencyList, banxList, creditTypeList, segmentList,
                    productList, productTypeList, warrantyList, termList, periodList);
                dgrReport.DataBind();
            }
            else
            {
                dgrReport.Visible = false;
                btnExportReport.Visible = false;
                ltrMessage.Text = "<strong style='color:#e82523'>Los filtros elegidos no son correctos.</strong>";
            }
        }

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            ltrMessage.Text = "";

            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=BMSCResumenTasas.csv");
                Response.Charset = "";
                Response.ContentType = "text/csv";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("RESUMEN;");
                sb.Append("MONEDA;");
                sb.Append("BANX;");
                sb.Append("CREDITO;");
                sb.Append("PRODUCTO;");
                sb.Append("TIPO PRODUCTO;");
                sb.Append("SEGMENTO;");
                sb.Append("GARANTIA;");
                sb.Append("PLAZO;");
                sb.Append("PERIODO;");
                sb.Append("TF;");
                sb.Append("TV");
                sb.Append("\r\n");

                for (int i = 0; i < dgrReport.Items.Count; i++)
                {
                    sb.Append(rblReportType.SelectedValue + ";");
                    sb.Append(rblCurrency.SelectedValue + ";");
                    sb.Append(rblBanx.SelectedValue + ";");
                    sb.Append(rblCreditType.SelectedValue +";");

                    for (int k = 0; k < dgrReport.Items[i].Cells.Count; k++)
                    {
                        string text = dgrReport.Items[i].Cells[k].Text;

                        if (text.Contains("span"))
                        {
                            string subs = text.Substring(text.IndexOf('>') + 1);
                            text = subs.Remove(subs.LastIndexOf('<'));
                        }

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

                ltrMessage.Text = ex.Message;
            }
        }
    }
}
