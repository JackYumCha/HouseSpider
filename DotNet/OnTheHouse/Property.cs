using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OnTheHouse
{

    public class ArangoDocumentBase
    {
        public string _key { get; set; }
        public string _id { get; set; }
    }

    public class Property : ArangoDocumentBase
    {
        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string StreetSuffix { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }

        public string AddressUseType { get; set; }
        public string WardName { get; set; }
        public string PropertyDescription { get; set; }
        public DateTime LastSearch { get; set; }
        public bool LastSearchSuccess { get; set; }
        public int TryCount { get; set; }
        public List<HouseHistory> RealEstateRecords { get; set; }
        public List<HouseHistory> DomainRecords { get; set; }

        public bool RealEstateInvalidAddress { get; set; }
        public bool DomainInvalidAddress { get; set; }

        public string RealEstateLandSize { get; set; }
        public string RealEstateFloorSize { get; set; }
        public string RealEstateYearBuilt { get; set; }
        public string RealEstateBedroom { get; set; }
        public string RealEstateBathroom { get; set; }
        public string RealEstateParking { get; set; }
        public string RealEstateValueRange { get; set; }

        public string DomainPropertyStory { get; set; }
        public string DomainBedroom { get; set; }
        public string DomainBathroom { get; set; }
        public string DomainParking { get; set; }
        public string DomainLowValue { get; set; }
        public string DomainMidValue { get; set; }
        public string DomainHighValue { get; set; }

        public bool Searching { get; set; }
        public DateTime SearchingStartDate { get; set; }

        public string ClientName { get; set; }

    }

    public class HouseHistory
    {
        public string Action { get; set; } //sold rent
        public string Date { get; set; }
        public string Value { get; set; }
    }

    public class SearchResult
    {
        public string LandSize { get; set; }
        public string FloorArea { get; set; }
        public string YearBuilt { get; set; }
        public string PropertyStory { get; set; }
        public bool InvalidAddress { get; set; }

        public string ValueRange { get; set; }
        public string LowValue { get; set; }
        public string MidValue { get; set; }
        public string HighValue { get; set; }
        public string Bedroom { get; set; }
        public string Bathroom { get; set; }
        public string Parking { get; set; }

        public List<HouseHistory> Data { get; set; }
    }


    public static class PropertyExtensions
    {

        public static Dictionary<string, string> StreetTypes = new Dictionary<string, string>()
        {
            {"alley", "ally"}, {"arcade", "arc"}, {"avenue", "ave"}, { "bend", "bend" }, {"boulevard", "bvd"}, {"bypass", "bypa"},
            { "circuit", "cct"}, {"close", "cl"}, {"corner", "crn"}, { "corso","cso" }, {"court", "ct"}, {"crescent", "cres"}, {"cul-de-sac", "cds"},
            { "drive", "dr"}, {"esplanade", "esp"}, {"green", "grn"}, {"grove", "gr"}, {"highway", "hwy"}, {"junction", "jnc"},
            { "lane", "lane"}, {"link", "link"}, {"mews", "mews"}, {"outlook","otlk" }, {"parade", "pde"}, {"place", "pl"}, { "quay", "qy" },
            { "ridge", "rdge"}, { "rise", "rise" },
            { "road", "rd"}, { "row", "row" }, {"square", "sq"}, {"street", "st"}, {"terrace", "tce"}, { "walk", "walk" }, { "way", "way" },

            { "freeway", "freeway" }, { "island", "island" }, {"parkway", "parkway"}
        };

        public static string BuildKey(this Property prop)
        {

            StringBuilder stb = new StringBuilder();

            List<string> sections = new List<string>();

            if (prop.UnitNumber != null && prop.UnitNumber != "")
            {
                sections.Add(prop.UnitNumber.ToLower());
            }

            if (prop.HouseNumber != null && prop.HouseNumber != "")
            {
                sections.Add(prop.HouseNumber.ToLower());
            }

            if (prop.StreetName != null && prop.StreetName != "")
            {
                sections.Add(prop.StreetName.ToLower());
            }

            if (prop.StreetType != null && prop.StreetType != "")
            {
                if (StreetTypes.ContainsKey(prop.StreetType.ToLower()))
                {
                    sections.Add(prop.StreetType.ToLower());
                }
                else
                {
                    Debug.WriteLine($"Street Type {prop.StreetType} Not Supported!");
                    Debugger.Break();
                }
            }

            if (prop.StreetSuffix != null && prop.StreetSuffix != "")
            {
                sections.Add(prop.StreetSuffix.ToLower());
            }

            if (prop.Suburb != null && prop.Suburb != "")
            {
                sections.Add(prop.Suburb.ToLower());
            }
            if (prop.Postcode != null && prop.Postcode != "")
            {
                sections.Add(prop.Postcode.ToLower());
            }

            prop._key = string.Join("-", sections).Replace(" ", "-");

            return prop._key;

        }

        public static string BuildDomainAddress(this Property prop)
        {
            StringBuilder stb = new StringBuilder();

            List<string> sections = new List<string>();

            if (prop.UnitNumber != null && prop.UnitNumber != "")
            {
                sections.Add(prop.UnitNumber.ToLower());
            }

            if (prop.HouseNumber != null && prop.HouseNumber != "")
            {
                sections.Add(prop.HouseNumber.ToLower());
            }

            if (prop.StreetName != null && prop.StreetName != "")
            {
                sections.Add(prop.StreetName.ToLower());
            }

            if (prop.StreetType != null && prop.StreetType != "")
            {
                if (StreetTypes.ContainsKey(prop.StreetType.ToLower()))
                {
                    sections.Add(prop.StreetType.ToLower());
                }
                else
                {
                    Debug.WriteLine($"Street Type {prop.StreetType} Not Supported!");
                    Debugger.Break();
                }
            }

            if (prop.StreetSuffix != null && prop.StreetSuffix != "")
            {
                sections.Add(prop.StreetSuffix.ToLower());
            }

            if (prop.Suburb != null && prop.Suburb != "")
            {
                sections.Add(prop.Suburb.ToLower());
            }
            sections.Add("qld");

            if (prop.Postcode != null && prop.Postcode != "")
            {
                sections.Add(prop.Postcode.ToLower());
            }

            prop._key = string.Join("-", sections).Replace(" ", "-");

            return prop._key;

        }

        public static string BuildRealEstateAddress(this Property prop)
        {
            StringBuilder stb = new StringBuilder();

            List<string> sections = new List<string>();

            if (prop.UnitNumber != null && prop.UnitNumber != "")
            {
                sections.Add("unit");
                sections.Add(prop.UnitNumber.ToLower());
            }

            if (prop.HouseNumber != null && prop.HouseNumber != "")
            {
                sections.Add(prop.HouseNumber.ToLower());
            }

            if (prop.StreetName != null && prop.StreetName != "")
            {
                sections.Add(prop.StreetName.ToLower());
            }

            if (prop.StreetType != null && prop.StreetType != "")
            {
                if (StreetTypes.ContainsKey(prop.StreetType.ToLower()))
                {
                    var abbr = StreetTypes[prop.StreetType.ToLower()];
                    sections.Add(abbr);
                }
                else
                {
                    Debug.WriteLine($"Street Type {prop.StreetType} Not Supported!");
                    Debugger.Break();
                }
            }

            if (prop.StreetSuffix != null && prop.StreetSuffix != "")
            {
                sections.Add(prop.StreetSuffix.ToLower());
            }

            if (prop.Suburb != null && prop.Suburb != "")
            {
                sections.Add(prop.Suburb.ToLower());
            }

            sections.Add("qld");

            if (prop.Postcode != null && prop.Postcode != "")
            {
                sections.Add(prop.Postcode.ToLower());
            }

            prop._key = string.Join("-", sections).Replace(" ", "-");

            return prop._key;

        }
    }

    public class QueryCondition: ArangoDocumentBase
    {
        public string Postcode { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
        public bool Complete { get; set; }
        public DateTime  CompleteDate { get; set; }
    }

}
