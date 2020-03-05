<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPAsfiRatesReportUserControl.ascx.cs" Inherits="BMSCRatesManagement.WPAsfiRatesReport.WPAsfiRatesReportUserControl" %>

<link href="/tps/SiteAssets/rates.css" rel="stylesheet">
<link href="/tps/SiteAssets/print.css" rel="stylesheet">

<div class="container">
    <div class="form-area filter-area">
        <div class="brand">
            <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
        </div>
        <div class="title">Resumen Tasas ASFI</div>
        <table class="form filter" style="text-align:center;">
            <tr>
                <td>
                    <strong>Tipo de Moneda</strong>
                    <asp:RadioButtonList ID="rblCurrency" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="MONEDA NACIONAL" Selected="True">MONEDA NACIONAL</asp:ListItem>
                        <asp:ListItem Value="MONEDA EXTRANJERA">MONEDA EXTRANJERA</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        <div class="buttons-area">
            <asp:Button runat="server" ID="btnShowReport" Text="Mostrar Reporte" OnClick="btnShowReport_Click" />
            <asp:Button runat="server" ID="btnExportReport" Text="Exportar Reporte" OnClick="btnExportReport_Click"
                OnClientClick="_spFormOnSubmitCalled = false; _spSuppressFormOnSubmitWrapper=true;" UseSubmitBehavior="false" Visible="false" />
        </div>
    </div>

    <div class="report-area">
        <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
        <asp:DataGrid ID="dgrReport" runat="server" AllowPaging="false" GridLines="None" BorderStyle="None">
        </asp:DataGrid>
    </div>
</div>