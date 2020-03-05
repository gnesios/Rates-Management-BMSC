using System;
using System.Text;

namespace BMSCRatesManagement
{
    class Average
    {
        string warranty;
        string term;
        string period;
        double maxFixed;
        double maxVariable;
        double minFixed;
        double minVariable;
        int dividerFixed;
        int dividerVariable;

        public string Period
        {
            get { return period; }
            set { period = value; }
        }
        public string Term
        {
            get { return term; }
            set { term = value; }
        }
        public string Warranty
        {
            get { return warranty; }
            set { warranty = value; }
        }
        public double MinVariable
        {
            get { return minVariable; }
            set { minVariable = value; }
        }
        public double MinFixed
        {
            get { return minFixed; }
            set { minFixed = value; }
        }
        public double MaxVariable
        {
            get { return maxVariable; }
            set { maxVariable = value; }
        }
        public double MaxFixed
        {
            get { return maxFixed; }
            set { maxFixed = value; }
        }
        public int DividerFixed
        {
            get { return dividerFixed; }
            set { dividerFixed = value; }
        }
        public int DividerVariable
        {
            get { return dividerVariable; }
            set { dividerVariable = value; }
        }

        public Average(string warranty, string term, string period,
            double maxFixed, double maxVariable, double minFixed, double minVariable)
        {
            Warranty = warranty;
            Term = term;
            Period = period;
            MinVariable = 0.0;
            MinFixed = 0.0;
            MaxVariable = 0.0;
            MaxFixed = 0.0;
            DividerFixed = 0;
            DividerVariable = 0;

            if (maxFixed <= 50.0)
            {
                MaxFixed = maxFixed;
                MinFixed = minFixed;
                DividerFixed = 1;
            }

            if (maxVariable <= 50.0)
            {
                MaxVariable = maxVariable;
                MinVariable = minVariable;
                DividerVariable = 1;
            }
        }

        public void Increment(double maxFixed, double maxVariable, double minFixed, double minVariable)
        {
            if (maxFixed <= 50.0)
            {
                MaxFixed += maxFixed;
                MinFixed += minFixed;
                DividerFixed++;
            }

            if (maxVariable <= 50.0)
            {
                MaxVariable += maxVariable;
                MinVariable += minVariable;
                DividerVariable++;
            }
        }
    }

    class ProductRate
    {
        int id;
        string product;
        string segment;
        int segmentId;
        double value;
        
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public ProductRate(int id, string product, string segment, int segmentId, double value)
        {
            Id = id;
            Product = product;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
    }

    class Variation
    {
        int id;
        string name;
        string product;
        string segment;
        int segmentId;
        double value;
        
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Variation(int id, string name, string product, string segment, int segmentId, double value)
        {
            Id = id;
            Name = name;
            Product = product;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
    }

    class Margin
    {
        int id;
        string name;
        string product;
        string segment;
        int segmentId;
        double value;
        string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Margin(int id, string name, string product, string segment, int segmentId, double value)
        {
            Id = id;
            Name = name;
            Product = product;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
        public Margin(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public Margin(double value, string comment)
        {
            Value = value;
            Comment = comment;
        }
    }

    class Rate
    {
        double rateFixed;
        double rateVariable;
        double cpopValue;
        string cpopBenefit;
        double disabledValue;
        string disabledBenefit;

        public string DisabledBenefit
        {
            get { return disabledBenefit; }
            set { disabledBenefit = value; }
        }
        public double DisabledValue
        {
            get { return disabledValue; }
            set { disabledValue = value; }
        }
        public string CpopBenefit
        {
            get { return cpopBenefit; }
            set { cpopBenefit = value; }
        }
        public double CpopValue
        {
            get { return cpopValue; }
            set { cpopValue = value; }
        }
        public double RateVariable
        {
            get { return rateVariable; }
            set { rateVariable = value; }
        }
        public double RateFixed
        {
            get { return rateFixed; }
            set { rateFixed = value; }
        }

        public Rate(double rateFixed, double rateVariable, double cpopValue, string cpopBenefit,
            double disabledValue, string disabledBenefit)
        {
            RateFixed = rateFixed;
            RateVariable = rateVariable;
            CpopValue = cpopValue;
            CpopBenefit = cpopBenefit;
            DisabledValue = disabledValue;
            DisabledBenefit = disabledBenefit;
        }
    }

    class Period
    {
        int id;
        string name;
        string display;
        string segment;
        int segmentId;
        double value;

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Display
        {
            get { return display; }
            set { display = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Period(int id, string name, string segment, int segmentId, double value)
        {
            Id = id;
            Name = name;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
        public Period(int id, string name, string display)
        {
            Id = id;
            Name = name;
            Display = display;
        }
    }

    class Term
    {
        int id;
        string name;
        string display;
        string segment;
        int segmentId;
        double value;

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Display
        {
            get { return display; }
            set { display = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Term(int id, string name, string segment, int segmentId, double value)
        {
            Id = id;
            Name = name;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
        public Term(int id, string name, string display)
        {
            Id = id;
            Name = name;
            Display = display;
        }
    }

    class Warranty
    {
        int id;
        string name;
        string segment;
        int segmentId;
        double value;

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public string Segment
        {
            get { return segment; }
            set { segment = value; }
        }
        public int SegmentId
        {
            get { return segmentId; }
            set { segmentId = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Warranty(int id, string name, string segment, int segmentId, double value)
        {
            Id = id;
            Name = name;
            Segment = segment;
            SegmentId = segmentId;
            Value = value;
        }
        public Warranty(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    class Product
    {
        int id;
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Product(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    class Segment
    {
        int id;
        string name;
        string type;
        string level;
        string nameAndType;
        string nameTypeLevel;

        public string NameTypeLevel
        {
            get { return nameTypeLevel; }
            set { nameTypeLevel = value; }
        }
        public string NameAndType
        {
            get { return nameAndType; }
            set { nameAndType = value; }
        }
        public string Level
        {
            get { return level; }
            set { level = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Segment(int id, string name, string type, string level)
        {
            Id = id;
            Name = name;
            Type = type;
            Level = level;
            NameAndType = name + " | " + type;
            NameTypeLevel = name + " | " + type + " | " + level;
        }
    }

    class Agreement
    {
        int agreementId;
        string agreementName;
        string agreementImage;

        public int AgreementId
        {
            get { return agreementId; }
            set { agreementId = value; }
        }
        public string AgreementName
        {
            get { return agreementName; }
            set { agreementName = value; }
        }
        public string AgreementImage
        {
            get { return agreementImage; }
            set { agreementImage = value; }
        }

        public Agreement(int agreementId, string agreementName, string agreementImage)
        {
            AgreementId = agreementId;
            AgreementName = agreementName;
            AgreementImage = agreementImage.Contains(",") ? agreementImage.Remove(agreementImage.IndexOf(',')) : agreementImage;
        }
    }

    class Campaign
    {
        int campaignId;
        string campaignName;
        string campaignImage;

        public int CampaignId
        {
            get { return campaignId; }
            set { campaignId = value; }
        }
        public string CampaignName
        {
            get { return campaignName; }
            set { campaignName = value; }
        }
        public string CampaignImage
        {
            get { return campaignImage; }
            set { campaignImage = value; }
        }

        public Campaign(int campaignId, string campaignName, string campaignImage)
        {
            CampaignId = campaignId;
            CampaignName = campaignName;
            CampaignImage = campaignImage.Contains(",") ? campaignImage.Remove(campaignImage.IndexOf(',')) : campaignImage;
        }
    }
}
