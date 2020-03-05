<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPAgreementsFormRequestUserControl.ascx.cs" Inherits="BMSCRatesManagement.WPAgreementsFormRequest.WPAgreementsFormRequestUserControl" %>

<link href="/tps/SiteAssets/rates.css" rel="stylesheet">
<link href="/tps/SiteAssets/print.css" rel="stylesheet">

<asp:Panel runat="server" ID="pnlForm">
<div class="container">
    <div class="form-area">
        <div class="brand">
            <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
        </div>
        <div class="title">Formulario Convenios</div>
        <table class="form">
            <tr>
                <td class="icon" rowspan="3"><span class="icon_exec" title="Datos del Ejecutivo de Cuenta"></span></td>
                <td class="left"><label>Nombre</label></td>
                <td class="right"><asp:Label ID="lblExecutive" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">
                    <label>C&oacute;digo FISA<br />
                        <asp:RequiredFieldValidator ID="rfvFisa" runat="server"
                            Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                            ControlToValidate="txbFisa" ValidationGroup="Show" CssClass="errorMessage" />
                        <asp:CompareValidator ID="cmvFisa" runat="server" Type="Integer"
                            Operator="DataTypeCheck" Text="Datos incorrectos." Display="Dynamic"
                            ControlToValidate="txbFisa" ValidationGroup="Show" CssClass="errorMessage" />
                    </label>
                </td>
                <td class="right"><asp:TextBox runat="server" ID="txbFisa" TabIndex="1" MaxLength="10"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="left"><label>Tipo de Cr&eacute;dito</label></td>
                <td class="right">
                    <asp:RadioButtonList runat="server" ID="rblCreditType" TabIndex="2" RepeatDirection="Horizontal">
                        <asp:ListItem Text="PERSONAL" Value="PERSONAS" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="PYME" Value="PYME"></asp:ListItem>
                        <asp:ListItem Text="EMPRESARIAL" Value="EMPRESARIAL"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
              <td colspan="3"><hr></td>
            </tr>
            <tr>
                <td class="icon" rowspan="6"><span class="icon_client" title="Datos del Cliente"></span></td>
                <td class="left">
                    <label>Nombre<br/>
                        <asp:RequiredFieldValidator ID="rfvClientName" runat="server"
                            Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                            ControlToValidate="txbClientName" ValidationGroup="Show" CssClass="errorMessage" />
                    </label>
                </td>
                <td class="right"><asp:TextBox runat="server" ID="txbClientName" TabIndex="3" MaxLength="40"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="left">
                    <label>C&oacute;digo<br />
                        <asp:RequiredFieldValidator ID="rfvClientCode" runat="server"
                            Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                            ControlToValidate="txbClientCode" ValidationGroup="Show" CssClass="errorMessage" />
                        <asp:CompareValidator ID="cmvClientCode" runat="server" Type="Integer" 
                            Operator="DataTypeCheck" Text="Datos incorrectos." Display="Dynamic"
                            ControlToValidate="txbClientCode" ValidationGroup="Show" CssClass="errorMessage" />
                    </label>
                </td>
                <td class="right"><asp:TextBox runat="server" ID="txbClientCode" TabIndex="4" MaxLength="10"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="left">
                    <label>Fecha de Nacimiento<br />
                        <asp:RequiredFieldValidator ID="rfvClientBirthdate" runat="server"
                            Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                            ControlToValidate="txbClientBirthdate" ValidationGroup="Show" CssClass="errorMessage" />
                        <asp:CompareValidator ID="cmvClientBirthdate" runat="server" Type="Date" 
                            Operator="DataTypeCheck" Text="Datos incorrectos." Display="Dynamic"
                            ControlToValidate="txbClientBirthdate" ValidationGroup="Show" CssClass="errorMessage" />
                    </label>
                </td>
                <td class="right"><asp:TextBox runat="server" ID="txbClientBirthdate" TabIndex="5" TextMode="Date" MaxLength="10" placeholder="día/mes/año"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="left"><label>Segmento</label></td>
                <td class="right">
                    <asp:RadioButtonList runat="server" ID="rblSegment" TabIndex="6" RepeatDirection="Horizontal">
                        <asp:ListItem Text="DEPENDIENTE" Value="DEPENDIENTE" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="INDEPENDIENTE" Value="INDEPENDIENTE"></asp:ListItem>
                        <asp:ListItem Text="JURÍDICO" Value="JURIDICO"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td class="left"><label>Discapacitado</label></td>
                <td class="right">
                    <asp:RadioButtonList runat="server" ID="rblDisabled" TabIndex="7" RepeatDirection="Horizontal">
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO" Selected="True"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td class="left"><label>CPOP</label></td>
                <td class="right">
                    <asp:RadioButtonList runat="server" ID="rblCpop" TabIndex="8" RepeatDirection="Horizontal">
                        <asp:ListItem Text="SI" Value="SI" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
              <td colspan="3"><hr></td>
            </tr>
            <tr>
                <td class="icon"><span class="icon_agreement" title="Datos del Convenio"></span></td>
                <td class="left">
                    <label>Empresa/Convenio<br />
                        <asp:RequiredFieldValidator ID="rfvAgreement" runat="server"
                            Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                            ControlToValidate="txbAgreement" ValidationGroup="Search" CssClass="errorMessage" />
                        <asp:RequiredFieldValidator ID="rfvListAgreement" runat="server"
                            Text="Seleccione el convenio." InitialValue="" Display="Dynamic"
                            ControlToValidate="lsbAgreement" ValidationGroup="Show" CssClass="errorMessage" />
                        <asp:Label ID="lblAgreementSearch" runat="server" CssClass="errorMessage"></asp:Label>
                    </label>
                </td>
                <td class="right">
                    <asp:UpdatePanel ID="updAgreementSearch" UpdateMode="Conditional" runat="server" class="updatePanel">
                        <ContentTemplate>
                            <asp:TextBox runat="server" ID="txbAgreement" TabIndex="9" MaxLength="20" placeholder="Ingrese * para buscar todas"></asp:TextBox>
                            <asp:Button ID="btnAgreementSearch" runat="server" TabIndex="10" Text="Buscar Convenio"
                                OnClick="btnAgreementSearch_Click" ValidationGroup="Search" />
                            <asp:ListBox ID="lsbAgreement" runat="server" TabIndex="11" Rows="5" SelectionMode="Single"
                                Enabled="false"></asp:ListBox>
                            <asp:UpdateProgress ID="uppAgreementSearch" AssociatedUpdatePanelID="updAgreementSearch" runat="server">
                                <ProgressTemplate><span class="loader"></span></ProgressTemplate>
                            </asp:UpdateProgress>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <div class="buttons-area">
            <asp:Button runat="server" ID="btnShowAgreement" TabIndex="12" Text="Mostrar Convenio"
                OnClick="btnShowAgreement_Click" ValidationGroup="Show" />
        </div>
    </div>
</div>
</asp:Panel>

<asp:Panel runat="server" ID="pnlPrint" Visible="false">
<div class="container">
    <div class="title-area"><asp:Label ID="lblAgreement" runat="server"></asp:Label></div>
    <div class="form-area">
        <div class="brand">
            <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
        </div>
        <table class="print">
            <tr>
                <td class="left">Ejecutivo de Cuenta:</td>
                <td class="right"><%: this.lblExecutive.Text %></td>
            </tr>
            <tr>
                <td class="left">C&oacute;digo FISA:</td>
                <td class="right"><asp:Label ID="lblFisa" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Tipo de Cr&eacute;dito:</td>
                <td class="right"><asp:Label ID="lblCreditType" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="2"><hr></td>
            </tr>
            <tr>
                <td class="left">Nombre del Cliente:</td>
                <td class="right"><asp:Label ID="lblClientName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">C&oacute;digo del Cliente:</td>
                <td class="right"><asp:Label ID="lblClientCode" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Cliente CPOP:</td>
                <td class="right"><asp:Label ID="lblCpop" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Cliente BANX:</td>
                <td class="right"><asp:Label ID="lblClientBirthdate" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Cliente con Discapacidad:</td>
                <td class="right"><asp:Label ID="lblDisabled" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="2"><hr></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Literal ID="ltrAgreementImage" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <div class="buttons-area">
            <asp:Label ID="lblMessage" runat="server" Visible="false" Text="Formulario guardado!" CssClass="formSaved"></asp:Label>
            <asp:Button runat="server" ID="btnBack" Text="Regresar" OnClick="btnBack_Click" />
            <asp:Button runat="server" ID="btnSaveAndPrint" Text="Guardar e Imprimir" OnClick="btnSaveAndPrint_Click" OnClientClick="javascript:window.print();" />
            <asp:Button runat="server" ID="btnPrint" Text="Imprimir" OnClientClick="javascript:window.print();" Visible="false" />
        </div>
    </div>
</div>
</asp:Panel>