<%@ Assembly Name="BMSCRatesManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f9b0abbb10184e3e" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPRatesFormRequestUserControl.ascx.cs" Inherits="BMSCRatesManagement.WPRatesFormRequest.WPRatesFormRequestUserControl" %>

<link href="/tps/SiteAssets/rates.css" rel="stylesheet">
<link href="/tps/SiteAssets/print.css" rel="stylesheet">

<asp:Panel runat="server" ID="pnlForm">
<div class="container">
    <div class="form-area">
        <div class="brand">
            <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
        </div>
        <div class="title">Formulario Tasas</div>
        <asp:UpdatePanel ID="updRate" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <table class="form">
                    <tr>
                        <td class="icon" rowspan="3"><span class="icon_exec" title="Datos del Ejecutivo de Cuenta"></span></td>
                        <td class="left"><label>Nombre</label></td>
                        <td class="right"><asp:Label ID="lblExecutive" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="left">
                            <label>C&oacute;digo FISA EECC/ACE</label><br />
                            <asp:RequiredFieldValidator ID="rfvFisa" runat="server"
                                Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                                ControlToValidate="txbFisa" ValidationGroup="Show" CssClass="errorMessage" />
                            <asp:CompareValidator ID="cmvFisa" runat="server" Type="Integer"
                                Operator="DataTypeCheck" Text="Datos incorrectos." Display="Dynamic"
                                ControlToValidate="txbFisa" ValidationGroup="Show" CssClass="errorMessage" />
                        </td>
                        <td class="right"><asp:TextBox runat="server" ID="txbFisa" TabIndex="1" MaxLength="10"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="left"><label>Banca</label></td>
                        <td class="right">
                            <asp:RadioButtonList runat="server" ID="rblBank" TabIndex="2" RepeatDirection="Horizontal">
                                <asp:ListItem Text="BANCA PERSONAS" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="BANCA PYME"></asp:ListItem>
                                <asp:ListItem Text="BANCA CORP. Y EMPRESAS"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                      <td colspan="3"><hr></td>
                    </tr>
                    <tr>
                        <td class="icon" rowspan="8"><span class="icon_client" title="Datos del Cliente"></span></td>
                        <td class="left">
                            <label>Nombre del Cliente<br/>
                                <asp:RequiredFieldValidator ID="rfvClientName" runat="server"
                                    Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                                    ControlToValidate="txbClientName" ValidationGroup="Show" CssClass="errorMessage" />
                            </label>
                        </td>
                        <td class="right"><asp:TextBox runat="server" ID="txbClientName" TabIndex="3" MaxLength="40"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="left">
                            <label>C&oacute;digo del Cliente<br/>
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
                            <label>Fecha de Nacimiento<br/>
                                <asp:RequiredFieldValidator ID="rfvClientBirthdate" runat="server"
                                    Text="Datos incorrectos." InitialValue="" Display="Dynamic"
                                    ControlToValidate="txbClientBirthdate" ValidationGroup="Show" CssClass="errorMessage" />
                                <asp:CompareValidator ID="cmvClientBirthdate" runat="server" Type="Date" 
                                    Operator="DataTypeCheck" Text="Formato incorrecto." Display="Dynamic"
                                    ControlToValidate="txbClientBirthdate" ValidationGroup="Show" CssClass="errorMessage" />
                            </label>
                        </td>
                        <td class="right">
                            <asp:TextBox runat="server" ID="txbClientBirthdate" TabIndex="5" TextMode="Date" MaxLength="10" placeholder="día/mes/año"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="left"><label>Sector</label></td>
                        <td class="right">
                            <asp:RadioButtonList runat="server" ID="rblSector" TabIndex="6" RepeatDirection="Horizontal">
                                <asp:ListItem Text="DEPENDIENTE" Value="DEPENDIENTE" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="INDEPENDIENTE" Value="INDEPENDIENTE"></asp:ListItem>
                                <asp:ListItem Text="JURÍDICO" Value="JURIDICO"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="left"><label>CPOP</label></td>
                        <td class="right">
                            <asp:RadioButtonList runat="server" ID="rblCpop" TabIndex="7" RepeatDirection="Horizontal">
                                <asp:ListItem Text="SI" Value="SI" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="left"><label>Beneficio Discapacitado</label></td>
                        <td class="right">
                            <asp:RadioButtonList runat="server" ID="rblDisabled" TabIndex="8" RepeatDirection="Horizontal">
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                <asp:ListItem Text="NO" Value="NO" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                            <label>IVC<br/>
                                <asp:CompareValidator ID="cmvIvc" runat="server" Type="Double" ValueToCompare="0"
                                    Operator="GreaterThanEqual" Text="Datos incorrectos." Display="Dynamic"
                                    ControlToValidate="txbIvc" ValidationGroup="Show" CssClass="errorMessage" />
                            </label>
                        </td>
                        <td class="right"><asp:TextBox ID="txbIvc" runat="server" TabIndex="9" MaxLength="5"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="left">
                            <label>IRT<br />
                                <asp:CompareValidator ID="cmvIrt" runat="server" Type="Double" ValueToCompare="0"
                                    Operator="GreaterThanEqual" Text="Datos incorrectos." Display="Dynamic"
                                    ControlToValidate="txbIrt" ValidationGroup="Show" CssClass="errorMessage" />
                            </label>
                        </td>
                        <td class="right"><asp:TextBox ID="txbIrt" runat="server" TabIndex="10" MaxLength="5"></asp:TextBox></td>
                    </tr>
                    <tr>
                      <td colspan="3"><hr></td>
                    </tr>
                    <tr>
                        <td colspan="3" style="position: relative;">
                            <asp:UpdateProgress ID="uppRate" AssociatedUpdatePanelID="updRate" runat="server">
                                <ProgressTemplate><span class="loader"></span></ProgressTemplate>
                            </asp:UpdateProgress>
                            <table>
                                <tr>
                                    <td class="icon" rowspan="9"><span class="icon_rate" title="Datos para el Crédito"></span></td>
                                    <td class="left">
                                        <label>Cr&eacute;dito Solicitado<br/>
                                            <asp:RequiredFieldValidator ID="rfvCreditType" runat="server" CssClass="errorMessage"
                                                Text="Seleccione el crédito." InitialValue="" Display="Dynamic"
                                                ControlToValidate="rblCreditType" ValidationGroup="Show" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:RadioButtonList runat="server" ID="rblCreditType" TabIndex="11" RepeatDirection="Horizontal"
                                            OnSelectedIndexChanged="rblCreditType_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="PERSONAL" Value="PERSONAL"></asp:ListItem>
                                            <asp:ListItem Text="PYME" Value="PYME"></asp:ListItem>
                                            <asp:ListItem Text="EMPRESARIAL" Value="EMPRESARIAL"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Segmento | Tamaño de Actvidad<br/>
                                            <asp:RequiredFieldValidator ID="rfvSegmentType" runat="server"
                                                Text="Seleccione el segmento/tipo." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlSegmentType" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList runat="server" ID="ddlSegmentType" TabIndex="12" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione el tipo de segmento)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Producto<br />
                                            <asp:RequiredFieldValidator ID="rfvProduct" runat="server"
                                                Text="Seleccione el producto." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlProduct" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlProduct" runat="server" TabIndex="14" AppendDataBoundItems="false"
                                            OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="(Seleccione el producto)" Value="" />
                                        </asp:DropDownList>
                                        <a class="help" href="#" onclick="openDialog('/tps/SitePages/Ayuda.aspx');"><img src="/tps/SiteAssets/icon_help.png" alt="ayuda" title="ayuda memoria" /></a>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left"><label>Moneda</label></td>
                                    <td class="right">
                                        <asp:RadioButtonList ID="rblCurrency" runat="server" TabIndex="15" RepeatDirection="Horizontal" Enabled="false">
                                            <asp:ListItem Text="MONEDA NACIONAL" Value="MONEDA NACIONAL" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="MONEDA EXTRANJERA" Value="MONEDA EXTRANJERA"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Tipo de Producto<br />
                                            <asp:RequiredFieldValidator ID="rfvProductType" runat="server"
                                                Text="Seleccione el tipo de producto." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlProductType" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlProductType" runat="server" TabIndex="16" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione el tipo de producto)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Garant&iacute;a<br />
                                            <asp:RequiredFieldValidator ID="rfvWarranty" runat="server"
                                                Text="Seleccione la garantía." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlWarranty" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlWarranty" runat="server" TabIndex="17" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione la garantía)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Plazo<br />
                                            <asp:RequiredFieldValidator ID="rfvTerm" runat="server"
                                                Text="Seleccione el plazo." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlTerm" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlTerm" runat="server" TabIndex="18" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione el plazo)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Per&iacute;odo Fijo<br />
                                            <asp:RequiredFieldValidator ID="rfvPeriodFixed" runat="server"
                                                Text="Seleccione el período." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlPeriodFixed" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlPeriodFixed" runat="server" TabIndex="19" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione el período)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                        <label>Margen de Negociaci&oacute;n<br />
                                            <asp:RequiredFieldValidator ID="rfvMargin" runat="server"
                                                Text="Seleccione el margen." InitialValue="" Display="Dynamic"
                                                ControlToValidate="ddlMargin" ValidationGroup="Show" CssClass="errorMessage" />
                                        </label>
                                    </td>
                                    <td class="right">
                                        <asp:DropDownList ID="ddlMargin" runat="server" TabIndex="20" AppendDataBoundItems="false">
                                            <asp:ListItem Text="(Seleccione el margen de negociación)" Value="" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="buttons-area">
            <asp:Button runat="server" ID="btnShowRates" TabIndex="21" Text="Mostrar Tasas"
                OnClick="btnShowRates_Click" ValidationGroup="Show" />
        </div>
    </div>
</div>
</asp:Panel>

<asp:Panel runat="server" ID="pnlPrint" Visible="false">
<div class="container">
    <div class="form-area">
        <div class="brand">
            <div><img src="/tps/SiteAssets/logo_bmsc.png" alt="" /><span>Mercantil Santa Cruz</span></div>
        </div>
        <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
        <table id="tblPrint" runat="server" class="print">
            <tr>
                <td class="icon" rowspan="3"></td>
                <td class="left">Ejecutivo de Cuenta:</td>
                <td class="right"><%: this.lblExecutive.Text %></td>
            </tr>
            <tr>
                <td class="left">C&oacute;digo FISA EECC/ACE:</td>
                <td class="right"><asp:Label ID="lblFisa" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Banca:</td>
                <td class="right"><asp:Label ID="lblBank" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="3"><hr></td>
            </tr>
            <tr>
                <td class="icon" rowspan="9"></td>
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
                <td class="right"><asp:Label ID="lblBanx" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Cliente con Discapacidad:</td>
                <td class="right"><asp:Label ID="lblDisabled" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Sector:</td>
                <td class="right"><asp:Label ID="lblSector" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left"><asp:Label ID="lblSegmentLabel" runat="server"></asp:Label></td>
                <td class="right"><asp:Label ID="lblSegmentType" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">IVC del Cliente:</td>
                <td class="right"><asp:Label ID="lblIvc" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">IRT del Cliente:</td>
                <td class="right"><asp:Label ID="lblIrt" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="3"><hr></td>
            </tr>
            <tr>
                <td class="icon" rowspan="7"></td>
                <td class="left">Cr&eacute;dito Solicitado:</td>
                <td class="right"><asp:Label ID="lblProduct" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Producto:</td>
                <td class="right"><asp:Label ID="lblProductType" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Garant&iacute;a:</td>
                <td class="right"><asp:Label ID="lblWarranty" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Plazo:</td>
                <td class="right"><asp:Label ID="lblTerm" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Per&iacute;odo Fijo:</td>
                <td class="right"><asp:Label ID="lblPeriodFixed" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Moneda:</td>
                <td class="right"><asp:Label ID="lblCurrency" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Margen de Negociaci&oacute;n:</td>
                <td class="right"><asp:Label ID="lblMargin" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="3"><hr></td>
            </tr>
            <tr>
                <td class="icon" rowspan="3"><span class="icon_rate" title="Oferta al Cliente"></span></td>
                <td class="left">Oferta al Cliente:</td>
                <td class="right"><asp:Label ID="lblRate" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Beneficio CPOP:</td>
                <td class="right"><asp:Label ID="lblCpopBenefit" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="left">Beneficio Discapacitado:</td>
                <td class="right"><asp:Label ID="lblDisabledBenefit" runat="server"></asp:Label></td>
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
