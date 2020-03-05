using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSCRatesManagement
{
    partial class SharePointConnector
    {
        #region Get methods
        internal static DataTable GetReportTableRates(
            List<string[]> selectedReportTypes, List<string[]> selectedCurrencies,
            List<string[]> selectedBanx, List<string[]> selectedCreditTypes,
            List<string[]> selectedSegments, List<string[]> selectedProducts,
            List<string[]> selectedProductTypes, List<string[]> selectedWarranties,
            List<string[]> selectedTerms, List<string[]> selectedPeriods)
        {
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            #region Creating table columns
            dTable.Columns.Add("Producto", typeof(string));
            dTable.Columns.Add("Producto Tipo", typeof(string));
            dTable.Columns.Add("Segmento", typeof(string));
            dTable.Columns.Add("Garantía", typeof(string));
            dTable.Columns.Add("Plazo", typeof(string));
            dTable.Columns.Add("Período", typeof(string));
            dTable.Columns.Add("Tasa Fija", typeof(string));
            dTable.Columns.Add("Tasa Var.", typeof(string));
            #endregion

            #region Filter asignments
            string selectedReportTypeValue = selectedReportTypes[0][1];
            string selectedCurrencyValue = selectedCurrencies[0][1];
            string selectedBanxValue = selectedBanx[0][1];
            string selectedCreditTypeValue = selectedCreditTypes[0][1];
            #endregion

            #region Geting table information (from 'Parámetros' section)
            List<string[]> segments = selectedSegments;
            List<string[]> products = selectedProducts;
            List<List<string[]>> productTypeList = new List<List<string[]>>();
            List<List<string[]>> warrantieList = new List<List<string[]>>();
            List<List<string[]>> termList = new List<List<string[]>>();
            List<List<string[]>> periodList = new List<List<string[]>>();

            foreach (string[] product in products)
            {
                productTypeList.Add(selectedProductTypes);
                warrantieList.Add(selectedWarranties);
                termList.Add(selectedTerms);
                periodList.Add(selectedPeriods);
            }
            #endregion

            #region Geting table information (from 'Tasas Asociadas' section)
            List<Variation> variationRates = LoadVariationRatesForReport(segments);
            List<Margin> marginRates = LoadMarginRatesForReport(segments);
            List<ProductRate> productRates = LoadProductRatesForReport(selectedCreditTypeValue, segments);
            List<Warranty> warrantyRates = LoadWarrantyRatesForReport(selectedCreditTypeValue, segments);
            List<Term> termRates = LoadTermRatesForReport(selectedCreditTypeValue, segments);
            List<Period> periodRates = LoadPeriodRatesForReport(selectedCreditTypeValue, segments);
            #endregion

            #region Creating table rows
            double _rate_variation_minfixed = 0.0;
            double _rate_variation_banx = 100;
            double _rate_margin_currency = 100;
            double _rate_margin_variabledifference = 0.0;
            double _rate_producttype = 0.0;
            double _rate_warranty = 0.0;
            double _rate_term = 0.0;
            double _rate_period = 0.0;

            int cont = 1;
            for (int i = 0; i < products.Count; i++)
            {
                foreach (string[] productType in productTypeList[i])
                {
                    List<Average> averageList = new List<Average>();
                    bool band = true;

                    try
                    {
                        foreach (string[] segment in segments)
                        {
                            int avg = 0;//aux variable for get the averages

                            #region Find Variations
                            try
                            {//Get 'Variacion = TASA FIJA MÍNIMA'
                                _rate_variation_minfixed = variationRates.Find(delegate(Variation var)
                                {
                                    return var.Name == VARIATION_RATE_MINIMAL &&
                                        var.Product == products[i][1] &&
                                        var.SegmentId == int.Parse(segment[1]);
                                }).Value;
                            }
                            catch { _rate_variation_minfixed = 100; }
                            try
                            {//Get 'Variacion = SEGMENTO BANX'
                                _rate_variation_banx = variationRates.Find(delegate(Variation var)
                                {
                                    return var.Name == VARIATION_RATE_BANX &&
                                        var.Product == products[i][1] &&
                                        var.SegmentId == int.Parse(segment[1]);
                                }).Value;
                            }
                            catch { _rate_variation_banx = 100; }
                            #endregion
                            #region Find Margins
                            try
                            {//Get 'Margen = DIFERENCIA ENTRE TASA FIJA Y VARIABLE'
                                _rate_margin_variabledifference = marginRates.Find(delegate(Margin mar)
                                {
                                    return mar.Name == MARGIN_RATE_VARIABLE_DIFFERENCE &&
                                        mar.Product == products[i][1] &&
                                        mar.SegmentId == int.Parse(segment[1]);
                                }).Value;
                            }
                            catch { _rate_margin_variabledifference = 100; }
                            try
                            {//Get 'Margen = MONEDA EXTRANJERA'
                                _rate_margin_currency = marginRates.Find(delegate(Margin mar)
                                {
                                    return mar.Name == MARGIN_FOREIGN_CURRENCY &&
                                        mar.Product == products[i][1] &&
                                        mar.SegmentId == int.Parse(segment[1]);
                                }).Value;
                            }
                            catch { _rate_margin_currency = 100; }
                            #endregion

                            #region Find Product Rates
                            try
                            {
                                _rate_producttype = productRates.Find(delegate(ProductRate prr)
                                {
                                    return prr.Product == string.Format("{0} | {1} | {2}", selectedCreditTypeValue, products[i][1], productType[0]) &&
                                        prr.SegmentId == int.Parse(segment[1]);
                                }).Value;
                            } catch { _rate_producttype = 100; }
                            #endregion

                            try
                            {
                                foreach (string[] warranty in warrantieList[i])
                                {
                                    #region Find Warranties
                                    try
                                    {
                                        _rate_warranty = warrantyRates.Find(delegate(Warranty war)
                                        {
                                            return war.Name == string.Format("{0} | {1} | {2}", selectedCreditTypeValue, products[i][1], warranty[0]) &&
                                                war.SegmentId == int.Parse(segment[1]);
                                        }).Value;
                                    } catch { _rate_warranty = 100; }
                                    #endregion

                                    try
                                    {
                                        foreach (string[] term in termList[i])
                                        {
                                            #region Find Terms
                                            try
                                            {
                                                _rate_term = termRates.Find(delegate(Term ter)
                                                {
                                                    return (ter.Name == string.Format("{0} | {1} | {2} | {3}", selectedCreditTypeValue, products[i][1], term[1], BENEFIT_ALL) ||
                                                        ter.Name == string.Format("{0} | {1} | {2} | {3}", selectedCreditTypeValue, products[i][1], term[1], selectedBanxValue == "SI" ? BENEFIT_BANX : BENEFIT_OTHER)) &&
                                                        ter.SegmentId == int.Parse(segment[1]);
                                                }).Value;
                                            } catch { _rate_term = 100; }
                                            #endregion

                                            try
                                            {
                                                foreach (string[] period in periodList[i])
                                                {
                                                    #region Find Periods
                                                    try
                                                    {
                                                        _rate_period = periodRates.Find(delegate(Period per)
                                                        {
                                                            return (per.Name == string.Format("{0} | {1} | {2} | {3}", selectedCreditTypeValue, products[i][1], period[1], BENEFIT_ALL) ||
                                                                per.Name == string.Format("{0} | {1} | {2} | {3}", selectedCreditTypeValue, products[i][1], period[1], selectedBanxValue == "SI" ? BENEFIT_BANX : BENEFIT_OTHER)) &&
                                                                per.SegmentId == int.Parse(segment[1]);
                                                        }).Value;
                                                    } catch { _rate_period = 100; }
                                                    #endregion

                                                    #region Calculate MAX/MIN rates
                                                    double maxFixedRate =
                                                        _rate_variation_minfixed +
                                                        _rate_producttype +
                                                        _rate_warranty +
                                                        _rate_term +
                                                        _rate_period;
                                                    double maxVariableRate =
                                                        _rate_variation_minfixed +
                                                        _rate_margin_variabledifference +
                                                        _rate_producttype +
                                                        _rate_warranty +
                                                        _rate_term +
                                                        _rate_period;
                                                    double minFixedRate = 0.0;//FORMULA PENDIENTE, OJO
                                                    double minVariableRate = 0.0;//FORMULA PENDIENTE, OJO

                                                    if (selectedBanxValue == "SI")
                                                    {
                                                        if (_rate_variation_banx < 100)
                                                        {
                                                            maxFixedRate += _rate_variation_banx;
                                                            maxVariableRate += _rate_variation_banx;
                                                            minFixedRate += _rate_variation_banx;
                                                            minVariableRate += _rate_variation_banx;
                                                        }
                                                        else
                                                        {
                                                            maxFixedRate = 100;
                                                            maxVariableRate = 100;
                                                            minFixedRate = 100;
                                                            minVariableRate = 100;
                                                        }
                                                    }
                                                    
                                                    if (selectedCurrencyValue == MARGIN_FOREIGN_CURRENCY)
                                                    {
                                                        if (_rate_margin_currency < 100)
                                                        {
                                                            maxFixedRate += _rate_margin_currency;
                                                            maxVariableRate += _rate_margin_currency;
                                                            minFixedRate += _rate_margin_currency;
                                                            minVariableRate += _rate_margin_currency;
                                                        }
                                                        else
                                                        {
                                                            maxFixedRate = 100;
                                                            maxVariableRate = 100;
                                                            minFixedRate = 100;
                                                            minVariableRate = 100;
                                                        }
                                                    }

                                                    //SPECIAL CONDITIONS BEGIN
                                                    if (selectedCreditTypeValue != CREDIT_PERSON &&
                                                        !products[i][1].Contains("NO PRODUCTIVO") &&
                                                        products[i][1].Contains("PRODUCTIVO"))
                                                    {
                                                        maxVariableRate = 100;
                                                        minVariableRate = 100;
                                                    }
                                                    //SPECIAL CONDITIONS END

                                                    Int16 p, t;
                                                    if (Int16.TryParse(period[1], out p) &&
                                                        Int16.TryParse(term[1], out t) &&
                                                        p < t)
                                                    { }
                                                    else
                                                    {
                                                        maxVariableRate = 100;
                                                        minVariableRate = 100;
                                                    }
                                                    #endregion

                                                    #region Calculate average
                                                    if (band)
                                                    {
                                                        Average average = new Average(warranty[0], term[0], period[0],
                                                            maxFixedRate, maxVariableRate, minFixedRate, minVariableRate);
                                                        averageList.Add(average);
                                                    }
                                                    else
                                                    {
                                                        averageList[avg].Increment(
                                                            maxFixedRate, maxVariableRate, minFixedRate, minVariableRate);
                                                        avg++;
                                                    }
                                                    #endregion

                                                    #region Place data in the table
                                                    dRow = dTable.NewRow();
                                                    dRow[0] = products[i][1];
                                                    dRow[1] = productType[0];
                                                    dRow[2] = segment[0];
                                                    dRow[3] = warranty[0];
                                                    dRow[4] = term[0];
                                                    dRow[5] = period[0];
                                                    
                                                    if (selectedReportTypeValue == "MAX")
                                                    {
                                                        dRow[6] = maxFixedRate > 50.0 ? "N.A." : String.Format("{0:#,0.00}", maxFixedRate) + "%";
                                                        dRow[7] = maxVariableRate > 50.0 ? "N.A." : String.Format("{0:#,0.00}", maxVariableRate) + "%";
                                                    }
                                                    else
                                                    {
                                                        dRow[6] = minFixedRate > 50.0 ? "N.A." : String.Format("{0:#,0.00}", minFixedRate) + "%";
                                                        dRow[7] = minVariableRate > 50.0 ? "N.A." : String.Format("{0:#,0.00}", minVariableRate) + "%";
                                                    }

                                                    dTable.Rows.Add(dRow);
                                                    cont++;
                                                    #endregion
                                                }
                                            } catch { }
                                        }
                                    } catch { }
                                }
                            } catch { }

                            band = false;
                        }
                    } catch { }

                    #region Placing average rows
                    foreach (Average average in averageList)
                    {
                        dRow = dTable.NewRow();
                        dRow[0] = products[i][1];
                        dRow[1] = productType[i];
                        dRow[2] = "<span style='color:#BB9959'>PROMEDIO</span>";
                        dRow[3] = average.Warranty;
                        dRow[4] = average.Term;
                        dRow[5] = average.Period;

                        if (selectedReportTypeValue == "MAX")
                        {
                            dRow[6] = average.DividerFixed != 0 ?
                                String.Format("{0:#,0.00}", average.MaxFixed / average.DividerFixed) + "%" : "N.A.";
                            dRow[7] = average.DividerVariable != 0 ?
                                String.Format("{0:#,0.00}", average.MaxVariable / average.DividerVariable) + "%" : "N.A.";
                        }
                        else
                        {
                            dRow[6] = average.DividerFixed != 0 ?
                                String.Format("{0:#,0.00}", average.MinFixed / average.DividerFixed) + "%" : "N.A.";
                            dRow[7] = average.DividerVariable != 0 ?
                                String.Format("{0:#,0.00}", average.MinVariable / average.DividerVariable) + "%" : "N.A.";
                        }

                        dTable.Rows.Add(dRow);
                        cont++;
                    }
                    #endregion
                }
            }
            #endregion

            return dTable;
        }

        internal static DataTable GetReportTableRates(string[] creditTypes, string selectedCurrencyValue)
        {
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            #region Creating table columns
            dTable.Columns.Add("Tipo Crédito", typeof(string));
            dTable.Columns.Add("Producto", typeof(string));
            dTable.Columns.Add("Producto Tipo", typeof(string));
            dTable.Columns.Add("Tasa Fija", typeof(string));
            dTable.Columns.Add("Tasa Var.", typeof(string));
            #endregion

            foreach (string creditType in creditTypes)
            {
                #region Geting table information (from 'Parámetros' section)
                List<Segment> segments = GetSegments(creditType);
                List<string> products = GetProducts(creditType, null, null);
                List<List<Product>> productTypeList = new List<List<Product>>();
                List<List<Warranty>> warrantieList = new List<List<Warranty>>();
                List<List<Term>> termList = new List<List<Term>>();
                List<List<Period>> periodList = new List<List<Period>>();

                foreach (string product in products)
                {
                    productTypeList.Add(GetProductTypes(product, creditType));
                    warrantieList.Add(GetWarranties(product, creditType));
                    termList.Add(GetTerms(product, creditType, null, null));
                    periodList.Add(GetPeriods(product, creditType, null, null));
                }
                #endregion

                #region Geting table information (from 'Tasas Asociadas' section)
                List<string[]> segmentsArray = new List<string[]>();
                foreach (Segment item in segments)
                {
                    segmentsArray.Add(new string[] { item.NameAndType, item.Id.ToString(), item.Level });
                }

                List<Variation> variationRates = LoadVariationRatesForReport(segmentsArray);
                List<Margin> marginRates = LoadMarginRatesForReport(segmentsArray);
                List<ProductRate> productRates = LoadProductRatesForReport(creditType, segmentsArray);
                List<Warranty> warrantyRates = LoadWarrantyRatesForReport(creditType, segmentsArray);
                List<Term> termRates = LoadTermRatesForReport(creditType, segmentsArray);
                List<Period> periodRates = LoadPeriodRatesForReport(creditType, segmentsArray);
                #endregion

                #region Creating table rows
                double _rate_variation_minfixed = 0.0;
                double _rate_margin_currency = 100;
                double _rate_margin_variabledifference = 0.0;
                double _rate_producttype = 0.0;
                double _rate_warranty = 0.0;
                double _rate_term = 0.0;
                double _rate_period = 0.0;

                for (int i = 0; i < products.Count; i++)
                {
                    foreach (Product productType in productTypeList[i])
                    {
                        double maxFixedProductTypeRate = 0.0;
                        double maxVariableProductTypeRate = 0.0;

                        try
                        {
                            foreach (string[] segment in segmentsArray)
                            {
                                #region Find Variations
                                try
                                {//Get 'Variacion = TASA FIJA MÍNIMA'
                                    _rate_variation_minfixed = variationRates.Find(delegate(Variation var)
                                    {
                                        return var.Name == VARIATION_RATE_MINIMAL &&
                                            var.Product == products[i] &&
                                            var.SegmentId == int.Parse(segment[1]);
                                    }).Value;
                                }
                                catch { _rate_variation_minfixed = 100; }
                                #endregion
                                #region Find Margins
                                try
                                {//Get 'Margen = DIFERENCIA ENTRE TASA FIJA Y VARIABLE'
                                    _rate_margin_variabledifference = marginRates.Find(delegate(Margin mar)
                                    {
                                        return mar.Name == MARGIN_RATE_VARIABLE_DIFFERENCE &&
                                            mar.Product == products[i] &&
                                            mar.SegmentId == int.Parse(segment[1]);
                                    }).Value;
                                }
                                catch { _rate_margin_variabledifference = 100; }
                                try
                                {//Get 'Margen = MONEDA EXTRANJERA'
                                    _rate_margin_currency = marginRates.Find(delegate(Margin mar)
                                    {
                                        return mar.Name == MARGIN_FOREIGN_CURRENCY &&
                                            mar.Product == products[i] &&
                                            mar.SegmentId == int.Parse(segment[1]);
                                    }).Value;
                                }
                                catch { _rate_margin_currency = 100; }
                                #endregion
                                #region Find Product Rates
                                try
                                {
                                    _rate_producttype = productRates.Find(delegate(ProductRate prr)
                                    {
                                        return prr.Product == string.Format("{0} | {1} | {2}", creditType, products[i], productType.Name) &&
                                            prr.SegmentId == int.Parse(segment[1]);
                                    }).Value;
                                }
                                catch { _rate_producttype = 100; }
                                #endregion

                                try
                                {
                                    foreach (Warranty warranty in warrantieList[i])
                                    {
                                        #region Find Warranties
                                        try
                                        {
                                            _rate_warranty = warrantyRates.Find(delegate(Warranty war)
                                            {
                                                return war.Name == string.Format("{0} | {1} | {2}", creditType, products[i], warranty.Name) &&
                                                    war.SegmentId == int.Parse(segment[1]);
                                            }).Value;
                                        }
                                        catch { _rate_warranty = 100; }
                                        #endregion

                                        try
                                        {
                                            foreach (Term term in termList[i])
                                            {
                                                #region Find Terms
                                                try
                                                {
                                                    _rate_term = termRates.Find(delegate(Term ter)
                                                    {
                                                        return (ter.Name == string.Format("{0} | {1} | {2} | {3}", creditType, products[i], term.Name, BENEFIT_ALL) ||
                                                            ter.Name == string.Format("{0} | {1} | {2} | {3}", creditType, products[i], term.Name, BENEFIT_OTHER)) &&
                                                            ter.SegmentId == int.Parse(segment[1]);
                                                    }).Value;
                                                }
                                                catch { _rate_term = 100; }
                                                #endregion

                                                try
                                                {
                                                    foreach (Period period in periodList[i])
                                                    {
                                                        #region Find Periods
                                                        try
                                                        {
                                                            _rate_period = periodRates.Find(delegate(Period per)
                                                            {
                                                                return (per.Name == string.Format("{0} | {1} | {2} | {3}", creditType, products[i], period.Name, BENEFIT_ALL) ||
                                                                    per.Name == string.Format("{0} | {1} | {2} | {3}", creditType, products[i], period.Name, BENEFIT_OTHER)) &&
                                                                    per.SegmentId == int.Parse(segment[1]);
                                                            }).Value;
                                                        }
                                                        catch { _rate_period = 100; }
                                                        #endregion

                                                        #region Calculate MAX rates
                                                        double maxFixedRate =
                                                            _rate_variation_minfixed +
                                                            _rate_producttype +
                                                            _rate_warranty +
                                                            _rate_term +
                                                            _rate_period;
                                                        double maxVariableRate =
                                                            _rate_variation_minfixed +
                                                            _rate_margin_variabledifference +
                                                            _rate_producttype +
                                                            _rate_warranty +
                                                            _rate_term +
                                                            _rate_period;

                                                        if (selectedCurrencyValue == MARGIN_FOREIGN_CURRENCY)
                                                        {
                                                            if (_rate_margin_currency < 100)
                                                            {
                                                                maxFixedRate += _rate_margin_currency;
                                                                maxVariableRate += _rate_margin_currency;
                                                            }
                                                            else
                                                            {
                                                                maxFixedRate = 100;
                                                                maxVariableRate = 100;
                                                            }
                                                        }

                                                        //SPECIAL CONDITIONS BEGIN
                                                        if (creditType != CREDIT_PERSON &&
                                                            !products[i].Contains("NO PRODUCTIVO") &&
                                                            products[i].Contains("PRODUCTIVO"))
                                                        {
                                                            maxVariableRate = 100;
                                                        }
                                                        //SPECIAL CONDITIONS END

                                                        Int16 p, t;
                                                        if (Int16.TryParse(period.Name, out p) &&
                                                            Int16.TryParse(term.Name, out t) &&
                                                            p < t)
                                                        { }
                                                        else
                                                        {
                                                            maxVariableRate = 100;
                                                        }

                                                        if (maxFixedRate <= 50.0 && maxFixedRate > maxFixedProductTypeRate)
                                                        {
                                                            maxFixedProductTypeRate = maxFixedRate;
                                                        }
                                                        if (maxVariableRate <= 50.0 && maxVariableRate > maxVariableProductTypeRate)
                                                        {
                                                            maxVariableProductTypeRate = maxVariableRate;
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                                catch { }
                            }

                            #region Place data in the table
                            dRow = dTable.NewRow();
                            dRow[0] = creditType;
                            dRow[1] = products[i];
                            dRow[2] = productType.Name;
                            dRow[3] = maxFixedProductTypeRate == 0 ? "N.A." :
                                String.Format("{0:#,0.00}", maxFixedProductTypeRate) + "%";
                            dRow[4] = maxVariableProductTypeRate == 0 ? "N.A." :
                                String.Format("{0:#,0.00}", maxVariableProductTypeRate) + "%";

                            dTable.Rows.Add(dRow);
                            #endregion
                        }
                        catch { }
                    }
                }
                #endregion
            }

            return dTable;
        }
        #endregion

        #region Private support methods
        private static List<Period> LoadPeriodRatesForReport(string creditType, List<string[]> segmentsList)
        {
            List<Period> periodRatesResult = new List<Period>();

            SPList periodsList = GetSharePointList(LIST_RATES_PERIODS);

            SPQuery queryPeriod = new SPQuery();
            queryPeriod.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><Eq><FieldRef Name='Tipo_x0020_Periodo_x003a_Tipo_Cr' /><Value Type='Text'>{0}</Value></Eq></Where>",
                creditType);
            SPListItemCollection queriedPeriods = periodsList.GetItems(queryPeriod);

            foreach (SPListItem period in queriedPeriods)
            {
                int id = period.ID;
                string name = period["Tipo_x0020_Periodo"].ToString().Substring(
                    period["Tipo_x0020_Periodo"].ToString().IndexOf('#') + 1);
                string segment = period["Segmento_x0020_Asociado"].ToString().Substring(
                    period["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(period["Segmento_x0020_Asociado"].ToString().Remove(
                    period["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(period["Valor_x0020_Tasa"].ToString());
                object difference = period["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        periodRatesResult.Add(
                            new Period(id, name, segment, segmentId, newValue));
                    }
                }
                else
                {
                    periodRatesResult.Add(
                        new Period(id, name, segment, segmentId, value));
                }
            }

            return periodRatesResult;
        }

        private static List<Term> LoadTermRatesForReport(string creditType, List<string[]> segmentsList)
        {
            List<Term> termRatesResult = new List<Term>();

            SPList termsList = GetSharePointList(LIST_RATES_TERMS);

            SPQuery queryTerm = new SPQuery();
            queryTerm.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><Eq><FieldRef Name='Tipo_x0020_Plazo_x003a_Plazo_Tip' /><Value Type='Text'>{0}</Value></Eq></Where>",
                creditType);
            SPListItemCollection queriedTerms = termsList.GetItems(queryTerm);

            foreach (SPListItem term in queriedTerms)
            {
                int id = term.ID;
                string name = term["Tipo_x0020_Plazo"].ToString().Substring(
                    term["Tipo_x0020_Plazo"].ToString().IndexOf('#') + 1);
                string segment = term["Segmento_x0020_Asociado"].ToString().Substring(
                    term["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(term["Segmento_x0020_Asociado"].ToString().Remove(
                    term["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(term["Valor_x0020_Tasa"].ToString());
                object difference = term["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        termRatesResult.Add(
                            new Term(id, name, segment, segmentId, newValue));
                    }
                }
                else
                {
                    termRatesResult.Add(
                        new Term(id, name, segment, segmentId, value));
                }
            }

            return termRatesResult;
        }

        private static List<Warranty> LoadWarrantyRatesForReport(string creditType, List<string[]> segmentsList)
        {
            List<Warranty> warrantiesRatesResult = new List<Warranty>();

            SPList warrantiesList = GetSharePointList(LIST_RATES_WARRANTIES);

            SPQuery queryWarranty = new SPQuery();
            queryWarranty.Query = string.Format(
                "<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>" +
                "<Where><Eq><FieldRef Name='Tipo_x0020_Garant_x00ed_a_x003a_0' /><Value Type='Text'>{0}</Value></Eq></Where>",
                creditType);
            SPListItemCollection queriedWarranties = warrantiesList.GetItems(queryWarranty);

            foreach (SPListItem warranty in queriedWarranties)
            {
                int id = warranty.ID;
                string name = warranty["Tipo_x0020_Garant_x00ed_a"].ToString().Substring(
                    warranty["Tipo_x0020_Garant_x00ed_a"].ToString().IndexOf('#') + 1);
                string segment = warranty["Segmento_x0020_Asociado"].ToString().Substring(
                    warranty["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(warranty["Segmento_x0020_Asociado"].ToString().Remove(
                    warranty["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(warranty["Valor_x0020_Tasa"].ToString());
                object difference = warranty["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        warrantiesRatesResult.Add(
                            new Warranty(id, name, segment, segmentId, newValue));
                    }
                }
                else
                {
                    warrantiesRatesResult.Add(
                        new Warranty(id, name, segment, segmentId, value));
                }
            }

            return warrantiesRatesResult;
        }

        private static List<ProductRate> LoadProductRatesForReport(string creditType, List<string[]> segmentsList)
        {
            List<ProductRate> productsRatesResult = new List<ProductRate>();

            SPList productRatesList = null;
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
                "<Where><And>" +
                "<Eq><FieldRef Name='Tipo_x0020_Producto_x003a_Produc0' /><Value Type='Text'>{0}</Value></Eq>" +
                "<Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                "</And></Where>",
                creditType);
            SPListItemCollection queriedProducts = productRatesList.GetItems(queryProduct);

            foreach (SPListItem product in queriedProducts)
            {
                int id = product.ID;
                string name = product["Tipo_x0020_Producto1"].ToString().Substring(
                    product["Tipo_x0020_Producto1"].ToString().IndexOf('#') + 1);
                string segment = product["Segmento_x0020_Asociado"].ToString().Substring(
                    product["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(product["Segmento_x0020_Asociado"].ToString().Remove(
                    product["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(product["Valor_x0020_Tasa"].ToString());
                object difference = product["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        productsRatesResult.Add(
                            new ProductRate(id, name, segment, segmentId, newValue));
                    }
                }
                else
                {
                    productsRatesResult.Add(
                        new ProductRate(id, name, segment, segmentId, value));
                }
            }

            return productsRatesResult;
        }

        private static List<Margin> LoadMarginRatesForReport(List<string[]> segmentsList)
        {
            List<Margin> marginsResult = new List<Margin>();

            SPList marginsList = GetSharePointList(LIST_RATES_MARGINS);

            SPQuery queryMargin = new SPQuery();
            queryMargin.Query = string.Format(
                "<Where><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq></Where>");
            SPListItemCollection queriedMargins = marginsList.GetItems(queryMargin);

            foreach (SPListItem margin in queriedMargins)
            {
                int id = margin.ID;
                string name = margin.Title;
                string product = margin["Producto_x0020_Asociado"].ToString();
                string segment = margin["Segmento_x0020_Asociado"].ToString().Substring(
                    margin["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(margin["Segmento_x0020_Asociado"].ToString().Remove(
                    margin["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(margin["Valor_x0020_Margen"].ToString());
                object difference = margin["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        marginsResult.Add(
                            new Margin(id, name, product, segment, segmentId, newValue));
                    }
                }
                else
                {
                    marginsResult.Add(
                        new Margin(id, name, product, segment, segmentId, value));
                }
            }

            return marginsResult;
        }

        private static List<Variation> LoadVariationRatesForReport(List<string[]> segmentsList)
        {
            List<Variation> variationsResult = new List<Variation>();

            SPList variationsList = GetSharePointList(LIST_RATES_VARIATIONS);

            SPQuery queryVariation = new SPQuery();
            queryVariation.Query = string.Format(
                "<Where><Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq></Where>");
            SPListItemCollection queriedVariations = variationsList.GetItems(queryVariation);

            foreach (SPListItem variation in queriedVariations)
            {
                int id = variation.ID;
                string name = variation.Title;
                string product = variation["Producto_x0020_Asociado"].ToString();
                string segment = variation["Segmento_x0020_Asociado"].ToString().Substring(
                    variation["Segmento_x0020_Asociado"].ToString().IndexOf('#') + 1);
                int segmentId = int.Parse(variation["Segmento_x0020_Asociado"].ToString().Remove(
                    variation["Segmento_x0020_Asociado"].ToString().IndexOf(';')));
                double value = double.Parse(variation["Valor_x0020_Variaci_x00f3_n"].ToString());
                object difference = variation["Dif_x002e__x0020_Seg_x002e_"];

                if (difference != null && !string.IsNullOrWhiteSpace(difference.ToString()))
                {//if is base segment
                    foreach (string[] item in segmentsList)
                    {
                        segment = item[0];
                        segmentId = int.Parse(item[1]);
                        Int16 segmentLevel = Int16.Parse(item[2]);
                        double newValue = value - (segmentLevel * double.Parse(difference.ToString()));

                        variationsResult.Add(
                            new Variation(id, name, product, segment, segmentId, newValue));
                    }
                }
                else
                {
                    variationsResult.Add(
                        new Variation(id, name, product, segment, segmentId, value));
                }
            }

            return variationsResult;
        }
        #endregion
    }
}
