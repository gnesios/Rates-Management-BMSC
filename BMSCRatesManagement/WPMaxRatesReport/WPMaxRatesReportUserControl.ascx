<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPMaxRatesReportUserControl.ascx.cs" Inherits="BMSCRatesManagement.WPMaxRatesReport.WPMaxRatesReportUserControl" %>

<link href="/tps/SiteAssets/rates.css" rel="stylesheet">
<link href="/tps/SiteAssets/print.css" rel="stylesheet">

<script>
    /*function MutExChkList(chk) {
        var chkList = chk.parentNode.parentNode.parentNode.parentNode;
        var chks = chkList.getElementsByTagName("input");
        for (var i = 0; i < chks.length; i++) {
            if (chks[i] != chk && chk.checked) {
                chks[i].checked = false;
            }
        }
    }*/

    function SetCheckBoxes(sender) {
        var cbl;

        if (sender.id.indexOf('cbxSegment') >= 0) {
            cbl = document.getElementById('<%= cblSegment.ClientID %>').getElementsByTagName("input");
        }
        if (sender.id.indexOf('cbxProductType') >= 0) {
            cbl = document.getElementById('<%= cblProductType.ClientID %>').getElementsByTagName("input");
        }
        if (sender.id.indexOf('cbxWarranty') >= 0) {
            cbl = document.getElementById('<%= cblWarranty.ClientID %>').getElementsByTagName("input");
        }
        if (sender.id.indexOf('cbxTerm') >= 0) {
            cbl = document.getElementById('<%= cblTerm.ClientID %>').getElementsByTagName("input");
        }
        if (sender.id.indexOf('cbxPeriod') >= 0) {
            cbl = document.getElementById('<%= cblPeriod.ClientID %>').getElementsByTagName("input");
        }

        for (i = 0; i < cbl.length; i++) {
            if (!cbl[i].disabled)
                cbl[i].checked = sender.checked;
        }
    }
</script>

<div class="container">
    <div class="form-area filter-area">
        <asp:UpdatePanel ID="updRate" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <div class="brand">
                    <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
                </div>
                <div class="title">
                    Resumen Tasas
                    <asp:UpdateProgress ID="uppRate" AssociatedUpdatePanelID="updRate" runat="server">
                        <ProgressTemplate><span class="loader report"></span></ProgressTemplate>
                    </asp:UpdateProgress>
                </div>
                <table class="form filter">
                    <tr>
                        <td colspan="2">
                            <table><tr>
                                <td class="options2" style="width:24%">
                                    <strong>Tipo de Resumen</strong>
                                    <asp:RadioButtonList ID="rblReportType" runat="server">
                                        <asp:ListItem Value="MAX" Selected="True">TASAS MÁXIMAS</asp:ListItem>
                                        <asp:ListItem Value="MIN">TASAS MÍNIMAS</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td class="options2" style="width:24%">
                                    <strong>Tipo de Moneda</strong>
                                    <asp:RadioButtonList ID="rblCurrency" runat="server">
                                        <asp:ListItem Value="MONEDA NACIONAL" Selected="True">MONEDA NACIONAL</asp:ListItem>
                                        <asp:ListItem Value="MONEDA EXTRANJERA">MONEDA EXTRANJERA</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <br />
                                    <strong>BANX</strong>
                                    <asp:RadioButtonList ID="rblBanx" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="rblBanx_SelectedIndexChanged">
                                        <asp:ListItem Value="SI">SI</asp:ListItem>
                                        <asp:ListItem Value="NO" Selected="True">NO</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td class="options2" style="width:24%">
                                    <strong>Tipo de Crédito</strong>
                                    <asp:RadioButtonList ID="rblCreditType" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="rblCreditType_SelectedIndexChanged">
                                        <asp:ListItem Value="PERSONAL">PERSONAL</asp:ListItem>
                                        <asp:ListItem Value="PYME">PYME</asp:ListItem>
                                        <asp:ListItem Value="EMPRESARIAL">EMPRESARIAL</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td class="options2 last" style="width:28%">
                                    <strong>Segmento</strong>
                                    [<span class="all"><asp:CheckBox ID="cbxSegment" runat="server" onclick="SetCheckBoxes(this);" Text="Todos" /></span>]
                                    <asp:CheckBoxList ID="cblSegment" runat="server" AppendDataBoundItems="false"></asp:CheckBoxList>
                                </td>
                            </tr></table>
                        </td>
                    </tr>
                    <tr><td colspan="2"><hr></td></tr>
                    <tr>
                        <td colspan="2">
                            <strong>Producto</strong>
                            <asp:RadioButtonList ID="rblProduct" runat="server" AppendDataBoundItems="false"
                                RepeatColumns="2" RepeatDirection="Horizontal" AutoPostBack="true"
                                OnSelectedIndexChanged="cblProduct_SelectedIndexChanged"></asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr><td colspan="2"><hr></td></tr>
                    <tr>
                        <td class="options1">
                            <strong>Tipo de Producto</strong>
                            [<span class="all"><asp:CheckBox ID="cbxProductType" runat="server" onclick="SetCheckBoxes(this);" Text="Todos" /></span>]
                            <asp:CheckBoxList ID="cblProductType" runat="server" AppendDataBoundItems="false"></asp:CheckBoxList>
                        </td>
                        <td class="options1 last">
                            <strong>Garantía</strong>
                            [<span class="all"><asp:CheckBox ID="cbxWarranty" runat="server" onclick="SetCheckBoxes(this);" Text="Todas" /></span>]
                            <asp:CheckBoxList ID="cblWarranty" runat="server" AppendDataBoundItems="false"></asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr><td colspan="2"><hr></td></tr>
                    <tr>
                        <td class="options1">
                            <strong>Plazo</strong>
                            [<span class="all"><asp:CheckBox ID="cbxTerm" runat="server" onclick="SetCheckBoxes(this);" Text="Todos" /></span>]
                            <asp:CheckBoxList ID="cblTerm" runat="server" AppendDataBoundItems="false"></asp:CheckBoxList>
                        </td>
                        <td class="options1 last">
                            <strong>Período Fijo</strong>
                            [<span class="all"><asp:CheckBox ID="cbxPeriod" runat="server" onclick="SetCheckBoxes(this);" Text="Todos" /></span>]
                            <asp:CheckBoxList ID="cblPeriod" runat="server" AppendDataBoundItems="false"></asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="buttons-area">
            <asp:Button runat="server" ID="btnShowReport" Text="Mostrar Reporte" OnClick="btnShowReport_Click" />
            <asp:Button runat="server" ID="btnExportReport" Text="Exportar Reporte" OnClick="btnExportReport_Click"
                OnClientClick="_spFormOnSubmitCalled = false; _spSuppressFormOnSubmitWrapper=true;" UseSubmitBehavior="false" Visible="false" />
        </div>
    </div>

    <div class="report-area">
        <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
        <asp:DataGrid ID="dgrReport" runat="server" AllowPaging="false" GridLines="none" BorderStyle="None">
        </asp:DataGrid>
    </div>
</div>