using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BMSCRatesManagement
{
    partial class SharePointConnector
    {
        #region Global and constant variables
        const string LIST_PRODUCTS = "Productos";
        const string LIST_SEGMENTS = "Segmentos";
        const string LIST_WARRANTIES = "Garantías";
        const string LIST_TERMS = "Plazos";
        const string LIST_PERIODS = "Periodos Fijos";

        const string LIST_AGREEMENTS = "Convenios";
        const string LIST_CAMPAIGNS = "Campañas";

        const string LIST_RATES_MARGINS = "Márgenes Negociación";
        const string LIST_RATES_VARIATIONS = "Variaciones";
        const string LIST_RATES_PEOPLE = "Tasas Personas";
        const string LIST_RATES_PYMES = "Tasas Pymes";
        const string LIST_RATES_COMPANIES = "Tasas Empresas";
        const string LIST_RATES_WARRANTIES = "Tasas Garantías";
        const string LIST_RATES_TERMS = "Tasas Plazos";
        const string LIST_RATES_PERIODS = "Tasas Periodos Fijos";

        const string LIST_RATES_SAVE = "Tasas Guardadas";
        const string LIST_AGREEMENTS_SAVE = "Convenios Guardados";
        const string LIST_CAMPAIGNS_SAVE = "Campañas Guardadas";

        const string CREDIT_PERSON = "PERSONAL";
        const string CREDIT_PYME = "PYME";
        const string CREDIT_COMPANY = "EMPRESARIAL";

        const string VARIATION_RATE_MINIMAL = "TASA FIJA MÍNIMA";
        const string VARIATION_RATE_BANX = "SEGMENTO BANX";

        const string MARGIN_RATE_VARIABLE_DIFFERENCE = "DIFERENCIA ENTRE TASA FIJA Y VARIABLE";
        const string MARGIN_CPOP = "CPOP";
        const string MARGIN_DISABLED = "BENEFICIO DISCAPACITADO";
        const string MARGIN_IVC_NEG = "IVC";
        const string MARGIN_IRT_NEG = "IRT";
        const string MARGIN_FOREIGN_CURRENCY = "MONEDA EXTRANJERA";

        const string BENEFIT_BANX = "BANX";
        const string BENEFIT_OTHER = "OTRO";
        const string BENEFIT_ALL = "TODOS";

        const string ROLE_MEMBER = "Integrantes Tasas Por Segmento";
        #endregion

        #region Get methods
        internal static List<Agreement> GetAgreements(string searchString)
        {
            List<Agreement> theAgreements = new List<Agreement>();

            #region SharePoint query
            SPQuery query = new SPQuery();

            if (searchString == "*")
            {
                query.Query = 
                    "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                    "<Where>" +
                    "<Geq><FieldRef Name='Vigencia_x0020_Convenio' /><Value Type='DateTime'><Today /></Value></Geq>" +
                    "</Where>";
            }
            else
            {
                query.Query = string.Format(
                    "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                    "<Where><And>" +
                    "<Contains><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Contains>" +
                    "<Geq><FieldRef Name='Vigencia_x0020_Convenio' /><Value Type='DateTime'><Today /></Value></Geq>" +
                    "</And></Where>",
                    searchString);
            }

            SPListItemCollection queriedAgreements = GetSharePointList(LIST_AGREEMENTS).GetItems(query);
            #endregion

            foreach (SPListItem agreement in queriedAgreements)
            {
                theAgreements.Add(
                    new Agreement(agreement.ID, agreement.Title, agreement["Imagen_x0020_Convenio"].ToString()));
            }

            return theAgreements;
        }

        internal static List<Campaign> GetCampaigns(string searchString)
        {
            List<Campaign> theCampaigns = new List<Campaign>();

            #region SharePoint query
            SPQuery query = new SPQuery();

            if (searchString == "*")
            {
                query.Query = 
                    "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                    "<Where>" +
                    "<Geq><FieldRef Name='Vigencia_x0020_Campa_x00f1_a' /><Value Type='DateTime'><Today /></Value></Geq>" +
                    "</Where>";
            }
            else
            {
                query.Query = string.Format(
                    "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                    "<Where><And>" +
                    "<Contains><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Contains>" +
                    "<Geq><FieldRef Name='Vigencia_x0020_Campa_x00f1_a' /><Value Type='DateTime'><Today /></Value></Geq>" +
                    "</And></Where>",
                    searchString);
            }

            SPListItemCollection queriedCampaigns = GetSharePointList(LIST_CAMPAIGNS).GetItems(query);
            #endregion

            foreach (SPListItem campaign in queriedCampaigns)
            {
                theCampaigns.Add(
                    new Campaign(campaign.ID, campaign.Title, campaign["Imagen_x0020_Campa_x00f1_a"].ToString()));
            }

            return theCampaigns;
        }

        /// <summary>
        /// Calculate the final rates to show.
        /// </summary>
        /// <param name="creditType"></param>
        /// <param name="segmentType"></param>
        /// <param name="segmentTypeId"></param>
        /// <param name="productName"></param>
        /// <param name="productType"></param>
        /// <param name="isBanx"></param>
        /// <param name="isDisabled"></param>
        /// <param name="isCpop"></param>
        /// <param name="sector"></param>
        /// <param name="ivc"></param>
        /// <param name="irt"></param>
        /// <param name="marginName"></param>
        /// <param name="warrantyName"></param>
        /// <param name="termName"></param>
        /// <param name="isForeignCurrency"></param>
        /// <returns>'Rate' object.</returns>
        internal static Rate GetRates(
            string creditType,
            int segmentTypeId,
            string productName,
            string productType,
            bool isBanx,
            bool isDisabled,
            bool isCpop,
            string sector,
            string ivc,
            string irt,
            string marginName,
            string warrantyName,
            string termName,
            string periodName,
            string currency)
        {
            Rate theRate;

            //variations rates
            double _rateFixedMin = GetRateFromVariationsList(
                segmentTypeId, productName, creditType, VARIATION_RATE_MINIMAL);
            double _rateBanx = isBanx ? GetRateFromVariationsList(
                segmentTypeId, productName, creditType, VARIATION_RATE_BANX) : 0.0;
            //produtc rate
            double _rateProductType = GetRateFromProductsList(
                creditType, productName, productType, segmentTypeId);
            //warranty rate
            double _rateWarranty = GetRateFromWarrantiesList(
                creditType, productName, warrantyName, segmentTypeId);
            //term rate
            double _rateTerm = GetRateFromTermsList(
                creditType, productName, termName, segmentTypeId, isBanx);
            //period rate
            double _ratePeriod = GetRateFromPeriodsList(
                creditType, productName, periodName, segmentTypeId, isBanx);
            //margins rates
            Margin cpopMargin = GetRateFromMarginsList(
                creditType, productName, segmentTypeId, MARGIN_CPOP);
            Margin disabledMargin = GetRateFromMarginsList(
                creditType, productName, segmentTypeId, MARGIN_DISABLED);
            double _rateMargin = GetRateFromMarginsList(
                creditType, productName, segmentTypeId, marginName).Value;
            double _rateCpop = isCpop ? cpopMargin.Value : 0.0;
            double _rateDisabled = isDisabled ? disabledMargin.Value : 0.0;
            double _rateIvcIrt = GetRateFromMarginsList(
                creditType, productName, segmentTypeId, sector, ivc, irt);
            double _rateVariable = GetRateFromMarginsList(
                creditType, productName, segmentTypeId, MARGIN_RATE_VARIABLE_DIFFERENCE).Value;
            double _rateCurrency = currency.Equals(MARGIN_FOREIGN_CURRENCY, StringComparison.CurrentCultureIgnoreCase) ?
                GetRateFromMarginsList(creditType, productName, segmentTypeId, MARGIN_FOREIGN_CURRENCY).Value : 0.0;
            string cpopBenefit = cpopMargin.Comment;
            string disabledBenefit = disabledMargin.Comment;

            double rateFixedFinal =
                _rateFixedMin +
                _rateBanx +
                _rateProductType +
                _rateWarranty +
                _rateTerm +
                _ratePeriod +
                _rateMargin +
                _rateCpop +
                _rateDisabled +
                _rateCurrency +
                _rateIvcIrt;

            double rateVariableFinal =
                rateFixedFinal +
                _rateVariable;

            theRate = new Rate(rateFixedFinal, rateVariableFinal, _rateCpop, cpopBenefit, _rateDisabled, disabledBenefit);

            return theRate;
        }

        /// <summary>
        /// Get all the margins from the list 'Márgenes Negociación'
        /// where the filed 'Delegación Comercial' is True and the item is Approved,
        /// by the product name, segment ID and credit type.
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <param name="creditType">The credit type</param>
        /// <returns>'Margin' objects list.</returns>
        internal static List<Margin> GetMargins(string productName, string segmentTypeId, string creditType)
        {
            List<Margin> theMargins = new List<Margin>();

            SPList marginsList = GetSharePointList(LIST_RATES_MARGINS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            #region SharePoint query
            SPQuery queryMargin = new SPQuery();
            queryMargin.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Delegaci_x00f3_n_x0020_Comercial' /><Value Type='Boolean'>1</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                productName, segmentTypeId);
            SPListItemCollection queriedMargins = marginsList.GetItems(queryMargin);

            if (queriedMargins.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryMargin = new SPQuery();
                queryMargin.Query = string.Format(
                    "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Delegaci_x00f3_n_x0020_Comercial' /><Value Type='Boolean'>1</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    productName, baseSegmentTypeId);
                queriedMargins = marginsList.GetItems(queryMargin);
            }
            #endregion

            foreach (SPListItem margin in queriedMargins)
            {
                theMargins.Add(
                    new Margin(margin.ID, margin.Title));
            }

            return theMargins;
        }

        /// <summary>
        /// Get all the periods from the list 'Periodos Fijos', by the product name and credit type.
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="creditType">The credit type</param>
        /// <returns>'Period' objects list.</returns>
        internal static List<Period> GetPeriods(string productName, string creditType, string birthdate, string banxIndicator)
        {
            List<Period> thePeriods = new List<Period>();

            #region SharePoint query
            bool isBanx;
            if (string.IsNullOrEmpty(birthdate))
                isBanx = (string.IsNullOrEmpty(banxIndicator) || banxIndicator == "NO" ? false : true);
            else
                isBanx = IsBanxClient(birthdate);

            SPQuery queryPeriod = new SPQuery();
            queryPeriod.Query = string.Format(
                "<OrderBy><FieldRef Name='_orden' Ascending='TRUE' /></OrderBy>" +
                "<Where><And><And><Or>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{2}</Value></Eq>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{3}</Value></Eq>" +
                "</Or><Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And></Where>",
                productName, creditType, BENEFIT_ALL, isBanx ? BENEFIT_BANX : BENEFIT_OTHER);
            SPListItemCollection queriedPeriods = GetSharePointList(LIST_PERIODS).GetItems(queryPeriod);
            #endregion

            foreach (SPListItem period in queriedPeriods)
            {
                Int16 res;
                thePeriods.Add(
                    new Period(period.ID, period.Name,
                        Int16.TryParse(period.Name, out res) ? period.Name + " MESES" : period.Name));
            }

            return thePeriods;
        }

        /// <summary>
        /// Get all the terms from the list 'Plazos', by the product name and credit type.
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="creditType">The credit type</param>
        /// <returns>'Term' objects list.</returns>
        internal static List<Term> GetTerms(string productName, string creditType, string birthdate, string banxIndicator)
        {
            List<Term> theTerms = new List<Term>();

            #region SharePoint query
            bool isBanx;
            if (string.IsNullOrEmpty(birthdate))
                isBanx = (string.IsNullOrEmpty(banxIndicator) || banxIndicator == "NO" ? false : true);
            else
                isBanx = IsBanxClient(birthdate);

            SPQuery queryTerm = new SPQuery();
            queryTerm.Query = string.Format(
                "<OrderBy><FieldRef Name='Plazo_x0020__x0028_meses_x0029_' Ascending='TRUE' /></OrderBy>" +
                "<Where><And><And><Or>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{2}</Value></Eq>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{3}</Value></Eq>" +
                "</Or><Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And></Where>",
                productName, creditType, BENEFIT_ALL, isBanx ? BENEFIT_BANX : BENEFIT_OTHER);
            SPListItemCollection queriedTerms = GetSharePointList(LIST_TERMS).GetItems(queryTerm);
            #endregion

            foreach (SPListItem term in queriedTerms)
            {
                string years = term["Plazo_x0020__x0028_meses_x0029_"].ToString().
                    Remove(term["Plazo_x0020__x0028_meses_x0029_"].ToString().IndexOf('.') + 2).
                    Substring(term["Plazo_x0020__x0028_meses_x0029_"].ToString().IndexOf('#') + 1);
                theTerms.Add(
                    new Term(term.ID, term.Title, term.Title + " MESES (" + years + " AÑOS)"));
            }

            return theTerms;
        }

        /// <summary>
        /// Get all the warranties from the list 'Garantías', by the product name and credit type.
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="creditType">The credit type</param>
        /// <returns>'Warranty' objects list.</returns>
        internal static List<Warranty> GetWarranties(string productName, string creditType)
        {
            List<Warranty> theWarranties = new List<Warranty>();

            #region SharePoint query
            SPQuery queryWarranty = new SPQuery();
            queryWarranty.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><And>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And></Where>",
                productName, creditType);
            SPListItemCollection queriedWarranties = GetSharePointList(LIST_WARRANTIES).GetItems(queryWarranty);
            #endregion

            foreach (SPListItem warranty in queriedWarranties)
            {
                theWarranties.Add(
                    new Warranty(warranty.ID, warranty.Title));
            }

            return theWarranties;
        }

        /// <summary>
        /// Get all the product types from the list 'Productos', by the product name and credit type.
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="creditType">The credit type</param>
        /// <returns>'Product' objects list.</returns>
        internal static List<Product> GetProductTypes(string productName, string creditType)
        {
            List<Product> theProducts = new List<Product>();

            #region SharePoint query
            SPQuery queryProductType = new SPQuery();
            queryProductType.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><And>" +
                "<Eq><FieldRef Name='Tipo_x0020_Producto' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And></Where>",
                productName, creditType);
            SPListItemCollection queriedProducts = GetSharePointList(LIST_PRODUCTS).GetItems(queryProductType);
            #endregion

            foreach (SPListItem product in queriedProducts)
            {
                theProducts.Add(
                    new Product(product.ID, product.Title));
            }

            return theProducts;
        }

        /// <summary>
        /// Get all the products from the list 'Productos', by the credit type.
        /// </summary>
        /// <param name="creditType">PERSONAL | PYME | EMPRESARIAL</param>
        /// <returns>String values.</returns>
        internal static List<string> GetProducts(string creditType, string birthdate, string banxIndicator)
        {
            List<string> theProducts = new List<string>();

            #region SharePoint query
            bool isBanx;
            if (string.IsNullOrEmpty(birthdate))
                isBanx = (string.IsNullOrEmpty(banxIndicator) || banxIndicator == "NO" ? false : true);
            else
                isBanx = IsBanxClient(birthdate);

            SPQuery queryProduct = new SPQuery();
            queryProduct.Query = string.Format(
                "<OrderBy><FieldRef Name='Tipo_x0020_Producto' Ascending='TRUE' /></OrderBy>" +
                "<Where><And><Or>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{1}</Value></Eq>" +
                "<Eq><FieldRef Name='Aplica_x0020_a' /><Value Type='Text'>{2}</Value></Eq>" +
                "</Or><Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                "</And></Where>",
                creditType, BENEFIT_ALL, isBanx ? BENEFIT_BANX : BENEFIT_OTHER);
            SPListItemCollection queriedProducts = GetSharePointList(LIST_PRODUCTS).GetItems(queryProduct);
            #endregion

            foreach (SPListItem product in queriedProducts)
            {
                string productName = product["Tipo_x0020_Producto"].ToString();

                if (!theProducts.Contains(productName))
                    theProducts.Add(productName);
            }

            return theProducts;
        }

        /// <summary>
        /// Get all the segments from the list 'Segmentos', by the credit type.
        /// </summary>
        /// <param name="creditType">PERSONAL | PYME | EMPRESARIAL</param>
        /// <returns>'Segment' objects list.</returns>
        internal static List<Segment> GetSegments(string creditType)
        {
            List<Segment> theSegments = new List<Segment>();

            #region SharePoint query
            SPQuery querySegment = new SPQuery();
            querySegment.Query = string.Format(
                "<OrderBy><FieldRef Name='Nivel_x0020_Segmento' Ascending='TRUE' /></OrderBy>" +
                "<Where><Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq></Where>",
                creditType);
            SPListItemCollection queriedSegments = GetSharePointList(LIST_SEGMENTS).GetItems(querySegment);
            #endregion

            foreach (SPListItem segment in queriedSegments)
            {
                theSegments.Add(
                    new Segment(segment.ID, segment.Title, segment["Tipo_x0020_Segmento"].ToString(),
                        segment["Nivel_x0020_Segmento"].ToString()));
            }

            return theSegments;
        }
        #endregion

        #region Set or Save methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fisaCode"></param>
        /// <param name="bankName"></param>
        /// <param name="clientNameCode"></param>
        /// <param name="cpopBanxDisabled"></param>
        /// <param name="sector"></param>
        /// <param name="segmentType"></param>
        /// <param name="ivcIrt"></param>
        /// <param name="productName"></param>
        /// <param name="productType"></param>
        /// <param name="warranty"></param>
        /// <param name="term"></param>
        /// <param name="currency"></param>
        /// <param name="margin"></param>
        /// <param name="rate"></param>
        /// <param name="cpopBenefit"></param>
        /// <param name="disabledBenefit"></param>
        internal static void SaveRate(string fisaCode, string executive, string bankName,
            string clientName, string clientCode, string cpop, string banx, string disabled,
            string sector, string segmentType, string ivc, string irt, 
            string productName, string productType, string warranty, string term, string period,
            string currency, string margin, string rate, string cpopBenefit, string disabledBenefit)
        {
            int currentUser = SPContext.Current.Web.CurrentUser.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList savedRatesList = GetSharePointList(LIST_RATES_SAVE);
                SPListItemCollection itemsRatesSave = savedRatesList.Items;
                SPListItem newItem = itemsRatesSave.Add();

                newItem["Title"] = fisaCode;
                newItem["Ejecutivo"] = executive;
                newItem["Banca"] = bankName;
                newItem["Cliente_x0020_Nombre_x002f_C_x00"] = clientName;
                newItem["Cliente_x0020_C_x00f3_digo"] = clientCode;
                newItem["CPOP_x002f_BANX"] = cpop;
                newItem["BANX"] = banx;
                newItem["DISCAP"] = disabled;
                newItem["Sector"] = sector;
                newItem["Tipo_x0020_Segmento"] = segmentType;
                newItem["IVC_x002f_IRT"] = ivc;
                newItem["IRT"] = irt;
                newItem["Producto"] = productName;
                newItem["Tipo_x0020_Producto"] = productType;
                newItem["Garant_x00ed_a"] = warranty;
                newItem["Plazo"] = term;
                newItem["Periodo_x0020_Fijo"] = period;
                newItem["Moneda"] = currency;
                newItem["Margen"] = margin;
                newItem["Tasa"] = rate;
                newItem["Beneficio_x0020_CPOP"] = cpopBenefit;
                newItem["Beneficio_x0020_Discapacitado"] = disabledBenefit;
                newItem["Author"] = currentUser;
                newItem["Editor"] = currentUser;

                try
                {
                    newItem.Web.AllowUnsafeUpdates = true;
                    newItem.Update();
                }
                finally
                {
                    newItem.Web.AllowUnsafeUpdates = false;
                }
            });
        }

        internal static void SaveAgreement(string fisaCode, string creditType, string clientCode,
            string clientName, string cpop_banx_disabled, string agreementImage)
        {
            int currentUser = SPContext.Current.Web.CurrentUser.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList agreementsList = GetSharePointList(LIST_AGREEMENTS_SAVE);
                SPListItemCollection itemsAgreementsSave = agreementsList.Items;
                SPListItem newItem = itemsAgreementsSave.Add();

                newItem["Código FISA"] = fisaCode;
                newItem["Tipo Crédito"] = creditType;
                newItem["Código Cliente"] = clientCode;
                newItem["Nombre Cliente"] = clientName;
                newItem["CPOP/BANX/DISC"] = cpop_banx_disabled;
                newItem["Author"] = currentUser;
                newItem["Editor"] = currentUser;
                newItem.Attachments.Add(
                    agreementImage.Substring(agreementImage.LastIndexOf('/') + 1),
                    agreementsList.ParentWeb.GetFile(agreementImage).OpenBinary());

                try
                {
                    newItem.Web.AllowUnsafeUpdates = true;
                    newItem.Update();
                }
                finally
                {
                    newItem.Web.AllowUnsafeUpdates = false;
                }
            });
        }

        internal static void SaveCampaign(string fisaCode, string creditType, string clientCode,
            string clientName, string cpop_banx_disabled, string campaignImage)
        {
            int currentUser = SPContext.Current.Web.CurrentUser.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList campaignsList = GetSharePointList(LIST_CAMPAIGNS_SAVE);
                SPListItemCollection itemsCampaignsSave = campaignsList.Items;
                SPListItem newItem = itemsCampaignsSave.Add();

                newItem["Código FISA"] = fisaCode;
                newItem["Tipo Crédito"] = creditType;
                newItem["Código Cliente"] = clientCode;
                newItem["Nombre Cliente"] = clientName;
                newItem["CPOP/BANX/DISC"] = cpop_banx_disabled;
                newItem["Author"] = currentUser;
                newItem["Editor"] = currentUser;
                newItem.Attachments.Add(
                    campaignImage.Substring(campaignImage.LastIndexOf('/') + 1),
                    campaignsList.ParentWeb.GetFile(campaignImage).OpenBinary());

                try
                {
                    newItem.Web.AllowUnsafeUpdates = true;
                    newItem.Update();
                }
                finally
                {
                    newItem.Web.AllowUnsafeUpdates = false;
                }
            });
        }
        #endregion

        #region Internal support methods
        /// <summary>
        /// Ask for the currency of the selected Product.
        /// </summary>
        /// <param name="productName">The selected product</param>
        /// <param name="creditType">The credit type</param>
        /// <param name="segmentTypeId">The segment type id</param>
        /// <returns>True if has a foreign currency, or False if not.</returns>
        internal static bool HasForeignCurrency(
            string productName, string creditType, string segmentTypeId)
        {
            SPList marginsList = GetSharePointList(LIST_RATES_MARGINS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryMargin = new SPQuery();
            queryMargin.Query = string.Format(
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                productName, segmentTypeId, MARGIN_FOREIGN_CURRENCY);
            SPListItemCollection queriedMargins = marginsList.GetItems(queryMargin);

            if (queriedMargins.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryMargin = new SPQuery();
                queryMargin.Query = string.Format(
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    productName, baseSegmentTypeId, MARGIN_FOREIGN_CURRENCY);
                queriedMargins = marginsList.GetItems(queryMargin);

                if (queriedMargins.Count == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Ask for any clients's disability.
        /// </summary>
        /// <param name="disabled"></param>
        /// <returns></returns>
        internal static bool IsDisabledClient(string disabled)
        {
            if (disabled == "SI")
                return true;

            return false;
        }

        /// <summary>
        /// Ask for CPOP client's benefit.
        /// </summary>
        /// <param name="cpop"></param>
        /// <returns></returns>
        internal static bool IsCpopClient(string cpop)
        {
            if (cpop == "SI")
                return true;

            return false;
        }

        /// <summary>
        /// Ask for the age of the client to know if is BANX.
        /// </summary>
        /// <param name="birthdate">The client birthdate</param>
        /// <returns></returns>
        internal static bool IsBanxClient(string birthdate)
        {
            if (!string.IsNullOrEmpty(birthdate))
            {
                DateTime theBirthdate = Convert.ToDateTime(birthdate);
                DateTime today = DateTime.Today;

                int age = today.Year - theBirthdate.Year;

                if (today < theBirthdate.AddYears(age))
                    age--;

                if (age < 36)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ask for current user role.
        /// </summary>
        /// <returns></returns>
        internal static bool IsContentEditor()
        {
            bool result = false;

            using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb spw = sps.OpenWeb())
            {
                foreach (SPGroup group in spw.CurrentUser.Groups)
                {
                    if (group.Name == ROLE_MEMBER)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        #endregion

        #region Private support methods
        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Márgenes Negociación',
        /// for IVC and IRT only.
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <param name="sector">The client sector</param>
        /// <param name="ivc">The IVC client value</param>
        /// <param name="irt">The IRT client value</param>
        /// <returns>'Double' value of the rate (IVC + IRT).</returns>
        private static double GetRateFromMarginsList(
            string creditType, string productName, int segmentTypeId, string sector, string ivc, string irt)
        {
            double rateMargin = 0.0;

            double rateIVCNeg = 0.0;
            double rateIRTNeg = 0.0;

            SPList marginsList = GetSharePointList(LIST_RATES_MARGINS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            #region IVC
            SPQuery queryMarginIVC = new SPQuery();
            queryMarginIVC.Query = string.Format(
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                MARGIN_IVC_NEG, productName, segmentTypeId);
            SPListItemCollection queriedMarginsIVC = marginsList.GetItems(queryMarginIVC);

            if (queriedMarginsIVC.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryMarginIVC = new SPQuery();
                queryMarginIVC.Query = string.Format(
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    MARGIN_IVC_NEG, productName, baseSegmentTypeId);
                queriedMarginsIVC = marginsList.GetItems(queryMarginIVC);

                try
                {
                    foreach (SPListItem margin in queriedMarginsIVC)
                    {
                        double rateValue =
                            double.Parse(margin["Valor_x0020_Margen"].ToString());
                        Int16 segmentLevel =
                            Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                        double segDif =
                            double.Parse(margin["Dif_x002e__x0020_Seg_x002e_"].ToString());

                        rateIVCNeg = rateValue - (segmentLevel * segDif);
                        break;
                    }
                }
                catch { }
            }
            else
            {
                foreach (SPListItem margin in queriedMarginsIVC)
                {
                    rateIVCNeg = double.Parse(margin["Valor_x0020_Margen"].ToString());
                    break;
                }
            }
            #endregion

            #region IRT
            SPQuery queryMarginIRT = new SPQuery();
            queryMarginIRT.Query = string.Format(
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                MARGIN_IRT_NEG, productName, segmentTypeId);
            SPListItemCollection queriedMarginsIRT = marginsList.GetItems(queryMarginIRT);

            if (queriedMarginsIRT.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryMarginIRT = new SPQuery();
                queryMarginIRT.Query = string.Format(
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    MARGIN_IRT_NEG, productName, baseSegmentTypeId);
                queriedMarginsIRT = marginsList.GetItems(queryMarginIRT);

                try
                {
                    foreach (SPListItem margin in queriedMarginsIRT)
                    {
                        double rateValue =
                            double.Parse(margin["Valor_x0020_Margen"].ToString());
                        Int16 segmentLevel =
                            Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                        double segDif =
                            double.Parse(margin["Dif_x002e__x0020_Seg_x002e_"].ToString());

                        rateIRTNeg = rateValue - (segmentLevel * segDif);
                        break;
                    }
                }
                catch { }
            }
            else
            {
                foreach (SPListItem margin in queriedMarginsIRT)
                {
                    rateIRTNeg = double.Parse(margin["Valor_x0020_Margen"].ToString());
                    break;
                }
            }
            #endregion

            //Formula for IVC and IRT
            if (!string.IsNullOrWhiteSpace(ivc))
            {
                if (creditType == CREDIT_COMPANY)
                {
                    if (double.Parse(ivc) > 5.0)
                        rateMargin = rateIVCNeg;
                }
                else
                {
                    if (double.Parse(ivc) > 4.0)
                        rateMargin = rateIVCNeg;
                }
            }

            if (!string.IsNullOrWhiteSpace(irt))
            {
                if (double.Parse(irt) > 1.0)
                    rateMargin = rateMargin + rateIRTNeg;
            }

            return rateMargin;
        }
        
        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Márgenes Negociación'.
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <param name="MARGIN">The negotiation margins</param>
        /// <returns>'Double' value of the rate.</returns>
        private static Margin GetRateFromMarginsList(
            string creditType, string productName, int segmentTypeId, string MARGIN)
        {
            double rateMargin = 0.0;
            string commentMargin = "";

            SPList marginsList = GetSharePointList(LIST_RATES_MARGINS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryMargin = new SPQuery();
            queryMargin.Query = string.Format(
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                productName, segmentTypeId, MARGIN);
            SPListItemCollection queriedMargins = marginsList.GetItems(queryMargin);

            if (queriedMargins.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryMargin = new SPQuery();
                queryMargin.Query = string.Format(
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    productName, baseSegmentTypeId, MARGIN);
                queriedMargins = marginsList.GetItems(queryMargin);

                try
                {//in case that the query returns no values
                    double rateValue =
                        double.Parse(queriedMargins[0]["Valor_x0020_Margen"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedMargins[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());

                    //Formula: "Valor Margen" - ("Nivel Segmento" * "Dif. Seg.")
                    rateMargin = rateValue - (segmentLevel * segDif);
                    commentMargin =
                        queriedMargins[0]["Comentarios_x0020_Margen"] != null ?
                        queriedMargins[0]["Comentarios_x0020_Margen"].ToString() : "";
                }
                catch { }
            }
            else
            {
                rateMargin = double.Parse(queriedMargins[0]["Valor_x0020_Margen"].ToString());
                commentMargin =
                    queriedMargins[0]["Comentarios_x0020_Margen"] != null ?
                    queriedMargins[0]["Comentarios_x0020_Margen"].ToString() : "";
            }

            return (new Margin(rateMargin, commentMargin));
        }

        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Tasas Periodos Fijos'
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="periodName">The period name</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <returns>'Double' value of the rate.</returns>
        private static double GetRateFromPeriodsList(
            string creditType, string productName, string periodName, int segmentTypeId, bool isBanx)
        {
            double ratePeriod = 0.0;

            SPList periodsList = GetSharePointList(LIST_RATES_PERIODS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryPeriod = new SPQuery();
            queryPeriod.Query = string.Format(
                "<Where><And><And><Or>" +
                "<Eq><FieldRef Name='Tipo_x0020_Periodo' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Tipo_x0020_Periodo' /><Value Type='Text'>{1}</Value></Eq>" +
                "</Or><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                creditType + " | " + productName + " | " + periodName + " | " + BENEFIT_ALL,
                creditType + " | " + productName + " | " + periodName + " | " + (isBanx ? BENEFIT_BANX : BENEFIT_OTHER),
                segmentTypeId);
            SPListItemCollection queriedTerms = periodsList.GetItems(queryPeriod);

            if (queriedTerms.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryPeriod = new SPQuery();
                queryPeriod.Query = string.Format(
                    "<Where><And><And><Or>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Periodo' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Periodo' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</Or><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType + " | " + productName + " | " + periodName + " | " + BENEFIT_ALL,
                    creditType + " | " + productName + " | " + periodName + " | " + (isBanx ? BENEFIT_BANX : BENEFIT_OTHER),
                    baseSegmentTypeId);
                queriedTerms = periodsList.GetItems(queryPeriod);

                try
                {//in case that the query returns no values
                    double rateValue =
                        double.Parse(queriedTerms[0]["Valor_x0020_Tasa"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedTerms[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());

                    //Formula: "Valor Tasa" - ("Nivel Segmento" * "Dif. Seg.")
                    ratePeriod = rateValue - (segmentLevel * segDif);
                }
                catch { }
            }
            else
            {
                ratePeriod = double.Parse(queriedTerms[0]["Valor_x0020_Tasa"].ToString());
            }

            return ratePeriod;
        }

        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Tasas Plazos'
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="termName">The term name</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <returns>'Double' value of the rate.</returns>
        private static double GetRateFromTermsList(
            string creditType, string productName, string termName, int segmentTypeId, bool isBanx)
        {
            double rateTerm = 0.0;

            SPList termsList = GetSharePointList(LIST_RATES_TERMS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryTerm = new SPQuery();
            queryTerm.Query = string.Format(
                "<Where><And><And><Or>" +
                "<Eq><FieldRef Name='Tipo_x0020_Plazo' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Tipo_x0020_Plazo' /><Value Type='Text'>{1}</Value></Eq>" +
                "</Or><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                creditType + " | " + productName + " | " + termName + " | " + BENEFIT_ALL,
                creditType + " | " + productName + " | " + termName + " | " + (isBanx ? BENEFIT_BANX : BENEFIT_OTHER),
                segmentTypeId);
            SPListItemCollection queriedTerms = termsList.GetItems(queryTerm);

            if (queriedTerms.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryTerm = new SPQuery();
                queryTerm.Query = string.Format(
                    "<Where><And><And><Or>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Plazo' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Plazo' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</Or><Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType + " | " + productName + " | " + termName + " | " + BENEFIT_ALL,
                    creditType + " | " + productName + " | " + termName + " | " + (isBanx ? BENEFIT_BANX : BENEFIT_OTHER),
                    baseSegmentTypeId);
                queriedTerms = termsList.GetItems(queryTerm);

                try
                {//in case that the query returns no values
                    double rateValue =
                        double.Parse(queriedTerms[0]["Valor_x0020_Tasa"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedTerms[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());

                    //Formula: "Valor Tasa" - ("Nivel Segmento" * "Dif. Seg.")
                    rateTerm = rateValue - (segmentLevel * segDif);
                }
                catch { }
            }
            else
            {
                rateTerm = double.Parse(queriedTerms[0]["Valor_x0020_Tasa"].ToString());
            }

            return rateTerm;
        }

        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Tasas Garantías'
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="warrantyName">The warranty name</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <returns>'Double' value of the rate.</returns>
        private static double GetRateFromWarrantiesList(
            string creditType, string productName, string warrantyName, int segmentTypeId)
        {
            double rateWarranty = 0.0;

            SPList warrantiesList = GetSharePointList(LIST_RATES_WARRANTIES);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryWarranty = new SPQuery();
            queryWarranty.Query = string.Format(
                "<Where><And><And>" +
                "<Eq><FieldRef Name='Tipo_x0020_Garant_x00ed_a' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                creditType + " | " + productName + " | " + warrantyName, segmentTypeId);
            SPListItemCollection queriedWarranties = warrantiesList.GetItems(queryWarranty);

            if (queriedWarranties.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryWarranty = new SPQuery();
                queryWarranty.Query = string.Format(
                    "<Where><And><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Garant_x00ed_a' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType + " | " + productName + " | " + warrantyName, baseSegmentTypeId);
                queriedWarranties = warrantiesList.GetItems(queryWarranty);

                try
                {//in case that the query returns no values
                    double rateValue =
                        double.Parse(queriedWarranties[0]["Valor_x0020_Tasa"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedWarranties[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());

                    //Formula: "Valor Tasa" - ("Nivel Segmento" * "Dif. Seg.")
                    rateWarranty = rateValue - (segmentLevel * segDif);
                }
                catch { }
            }
            else
            {
                rateWarranty = double.Parse(queriedWarranties[0]["Valor_x0020_Tasa"].ToString());
            }

            return rateWarranty;
        }

        /// <summary>
        /// Get the associated rate of the parameters, from the one of the lists
        /// 'Tasas Personas', 'Tasas Pymes' o 'Tasas Empresas'.
        /// </summary>
        /// <param name="creditType">The credit type</param>
        /// <param name="productName">The product name</param>
        /// <param name="productType">The product type</param>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <returns>'Double' value of the rate.</returns>
        private static double GetRateFromProductsList(
            string creditType, string productName, string productType, int segmentTypeId)
        {
            double rateProductType = 0.0;
            
            SPList productRatesList = null;
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            switch (creditType)
            {
                case CREDIT_PERSON:
                    productRatesList = GetSharePointList(LIST_RATES_PEOPLE);
                    break;
                case CREDIT_PYME:
                    productRatesList = GetSharePointList(LIST_RATES_PYMES);
                    break;
                case CREDIT_COMPANY:
                    productRatesList = GetSharePointList(LIST_RATES_COMPANIES);
                    break;
            }

            SPQuery queryProduct = new SPQuery();
            queryProduct.Query = string.Format(
                "<Where><And><And>" +
                "<Eq><FieldRef Name='Tipo_x0020_Producto1' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                creditType + " | " + productName + " | " + productType, segmentTypeId);
            SPListItemCollection queriedProducts = productRatesList.GetItems(queryProduct);

            if (queriedProducts.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryProduct = new SPQuery();
                queryProduct.Query = string.Format(
                    "<Where><And><And>" +
                    "<Eq><FieldRef Name='Tipo_x0020_Producto1' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType + " | " + productName + " | " + productType, baseSegmentTypeId);
                queriedProducts = productRatesList.GetItems(queryProduct);

                try
                {//in case that the query returns no values
                    double rateValue =
                        double.Parse(queriedProducts[0]["Valor_x0020_Tasa"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedProducts[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());

                    //Formula: "Valor Tasa" - ("Nivel Segmento" * "Dif. Seg.")
                    rateProductType = rateValue - (segmentLevel * segDif);
                }
                catch { }
            }
            else
            {
                rateProductType = double.Parse(queriedProducts[0]["Valor_x0020_Tasa"].ToString());
            }

            return rateProductType;
        }

        /// <summary>
        /// Get the associated rate of the parameters, from the list 'Variaciones'.
        /// </summary>
        /// <param name="segmentTypeId">The segment ID</param>
        /// <param name="productName">The product name</param>
        /// <param name="creditType">The credit type</param>
        /// <param name="VARIATION">The name of the variation</param>
        /// <returns>'Double' value of the rate.</returns>
        private static double GetRateFromVariationsList(
            int segmentTypeId, string productName, string creditType, string VARIATION)
        {
            double rateVariation = 0.0;

            SPList variationsList = GetSharePointList(LIST_RATES_VARIATIONS);
            SPList segmentsList = GetSharePointList(LIST_SEGMENTS);

            SPQuery queryVariation = new SPQuery();
            queryVariation.Query = string.Format(
                "<Where><And><And><And>" +
                "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                productName, segmentTypeId, VARIATION);
            SPListItemCollection queriedVariations = variationsList.GetItems(queryVariation);

            if (queriedVariations.Count == 0)
            {//if no query results, then query for the base segment
                SPQuery querySegmentLevel = new SPQuery();
                querySegmentLevel.Query = string.Format(
                    "<Where><And>"+
                    "<Eq><FieldRef Name='Tipo_x0020_Cr_x00e9_dito' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Nivel_x0020_Segmento' /><Value Type='Text'>0</Value></Eq>" +
                    "</And></Where>",
                    creditType);

                string baseSegmentTypeId = string.Empty;
                try
                {//in case that the query returns no values
                    baseSegmentTypeId = segmentsList.GetItems(querySegmentLevel)[0].ID.ToString();
                }
                catch { }

                queryVariation = new SPQuery();
                queryVariation.Query = string.Format(
                    "<Where><And><And><And>" +
                    "<Eq><FieldRef Name='Producto_x0020_Asociado' /><Value Type='Text'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='Segmento_x0020_Asociado_x003a_ID' /><Value Type='Text'>{1}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='Title' /><Value Type='Text'>{2}</Value></Eq>" +
                    "</And><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    productName, baseSegmentTypeId, VARIATION);
                queriedVariations = variationsList.GetItems(queryVariation);

                try
                {//in case that the query returns no values
                    double variationValue =
                        double.Parse(queriedVariations[0]["Valor_x0020_Variaci_x00f3_n"].ToString());
                    Int16 segmentLevel =
                        Int16.Parse(segmentsList.GetItemById(segmentTypeId)["Nivel_x0020_Segmento"].ToString());
                    double segDif =
                        double.Parse(queriedVariations[0]["Dif_x002e__x0020_Seg_x002e_"].ToString());
                    
                    //Formula: "Valor Variación" - ("Nivel Segmento" * "Dif. Seg.")
                    rateVariation = variationValue - (segmentLevel * segDif);
                }
                catch { }
            }
            else
            {
                rateVariation = double.Parse(queriedVariations[0]["Valor_x0020_Variaci_x00f3_n"].ToString());
            }

            return rateVariation;
        }

        private static SPList GetSharePointList(string listName)
        {
            using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb spw = sps.OpenWeb())
            {
                return spw.Lists[listName];
            }
        }
        #endregion
    }
}
