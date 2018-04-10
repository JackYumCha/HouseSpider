using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace OnTheHouse
{
    public static class Domain
    {
        public static string baseUrl = @"https://www.domain.com.au/property-profile/";

        public static SearchResult SearchDomain(Property property)
        {
            var address = property.BuildDomainAddress();

            return SearchDomain(address);
        }

        public static SearchResult SearchDomain(string path)
        {
            SearchResult result = new SearchResult();

            using (HttpClient httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(@"https://www.domain.com.au/property-profile/");

                //httpClient.DefaultRequestHeaders.Add("referer", @"https://www.domain.com.au/property-profile");
                httpClient.DefaultRequestHeaders.Add("user-agent", " Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                var response =  httpClient.GetAsync(path).GetAwaiter().GetResult();


                string html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                

                HtmlDocument document = new HtmlDocument();

                document.LoadHtml(html);

                var title = document.DocumentNode.DescendantsAndSelf().Where(n => n.Name == "title").FirstOrDefault();

                if(title != null && title.InnerText == "Page cannot be found")
                {
                    result.InvalidAddress = true;
                    return result;
                }

                Regex rgxPropertyObject = new Regex(@"propertyObject: *{");

                var propertObjectNode = document.DocumentNode.DescendantsAndSelf()
                    .Where(n =>
                   n.Name.ToLower() == "script" &&
                   n.GetAttributeValue("type", "") == "text/javascript" &&
                   rgxPropertyObject.IsMatch(n.InnerText)).FirstOrDefault();

                var propertyScript = propertObjectNode.InnerText;

                var propObjectText = rgxPropertyObject.Match(propertyScript);

                if (propObjectText.Success)
                {
                    var objectJson = propertyScript.Substring(propObjectText.Index).PairMatchSearch("{", "}").FirstOrDefault();

                    var propObj = JsonConvert.DeserializeObject<PropertyObject>(objectJson);

                    result.Bedroom = propObj.bedrooms.ToString();
                    result.Bathroom = propObj.bathrooms.ToString();
                    result.Parking = propObj.carSpaces.ToString();
                    result.LandSize = propObj.areaSize.ToString();

                    result.LowValue = propObj.valuation.lowerPrice.ToString();
                    result.MidValue = propObj.valuation.midPrice.ToString();
                    result.HighValue = propObj.valuation.upperPrice.ToString();
                    result.RentPerWeek = propObj.valuation.rentPerWeek.ToString();

                    //foreach(var entry in propObj.timeline)
                    //{
                    //    result.Data.Add(new HouseHistory()
                    //    {
                    //        Action = entry.category,
                    //        Date = entry.eventDate.ToString(),
                    //        Agent = entry.agency.name,
                    //        Value = entry.eventPrice.ToString(),
                    //    });
                    //}

                    result.Data = propObj.timeline.Select(entry => new HouseHistory()
                    {
                        Action = entry.category,
                        Date = entry.eventDate.ToString(),
                        Agent = entry.agency?.name,
                        Value = entry.eventPrice.ToString(),
                    }).ToList();
                }

            }

            return result;
        }


    }


    public class AddressCoordinate
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class Cadastre
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class First
    {
        public DateTime advertisedDate { get; set; }
        public int advertisedPrice { get; set; }
        public string agency { get; set; }
        public int agencyId { get; set; }
        public string agencyLogo { get; set; }
        public string agencyUrl { get; set; }
        public int apmAgencyId { get; set; }
        public string source { get; set; }
        public bool suppressDetails { get; set; }
        public bool suppressPrice { get; set; }
        public string type { get; set; }
    }

    public class Last
    {
        public DateTime advertisedDate { get; set; }
        public int advertisedPrice { get; set; }
        public string agency { get; set; }
        public int agencyId { get; set; }
        public string agencyLogo { get; set; }
        public string agencyUrl { get; set; }
        public int apmAgencyId { get; set; }
        public string source { get; set; }
        public bool suppressDetails { get; set; }
        public bool suppressPrice { get; set; }
        public string type { get; set; }
    }

    public class Rental
    {
        public First first { get; set; }
        public long id { get; set; }
        public Last last { get; set; }
        public string propertyType { get; set; }
    }

    public class First2
    {
        public bool suppressDetails { get; set; }
    }

    public class Last2
    {
        public bool suppressDetails { get; set; }
    }

    public class Sale
    {
        public string agency { get; set; }
        public int apmAgencyId { get; set; }
        public DateTime date { get; set; }
        public bool documentedAsSold { get; set; }
        public int price { get; set; }
        public bool reportedAsSold { get; set; }
        public bool suppressDetails { get; set; }
        public bool suppressPrice { get; set; }
        public string type { get; set; }
        public First2 first { get; set; }
        public object id { get; set; }
        public Last2 last { get; set; }
        public string propertyType { get; set; }
    }

    public class Valuation
    {
        public string confidence { get; set; }
        public DateTime date { get; set; }
        public int lowerPrice { get; set; }
        public int upperPrice { get; set; }
    }

    public class History
    {
        public List<Rental> rentals { get; set; }
        public List<Sale> sales { get; set; }
        public List<Valuation> valuations { get; set; }
    }

    public class Photo
    {
        public int advertId { get; set; }
        public DateTime date { get; set; }
        public string fullUrl { get; set; }
        public int rank { get; set; }
    }

    public class SaleMetadata
    {
        public bool isSold { get; set; }
        public List<object> marketSteps { get; set; }
    }

    public class Agency
    {
        public string name { get; set; }
    }

    public class Timeline
    {
        public SaleMetadata saleMetadata { get; set; }
        public Agency agency { get; set; }
        public string category { get; set; }
        public DateTime eventDate { get; set; }
        public int eventPrice { get; set; }
        public string priceDescription { get; set; }
        public bool isMajorEvent { get; set; }
    }

    public class Valuation2
    {
        public DateTime date { get; set; }
        public int lowerPrice { get; set; }
        public string priceConfidence { get; set; }
        public int rentPerWeek { get; set; }
        public double rentYield { get; set; }
        public int upperPrice { get; set; }
        public int midPrice { get; set; }
    }

    public class LastRentActivity
    {
        public int advertId { get; set; }
        public DateTime advertisedDate { get; set; }
        public int advertisedPrice { get; set; }
        public int rentedPrice { get; set; }
        public bool suppressPrice { get; set; }
    }

    public class CapitalGrowth
    {
        public double annualGrowth { get; set; }
        public int previousSoldPrice { get; set; }
        public DateTime previousSoldDate { get; set; }
        public bool suppressedPreviousSoldPrice { get; set; }
    }

    public class RentalYield
    {
        public double grossRentalYield { get; set; }
        public int rentedPrice { get; set; }
        public DateTime rentedDate { get; set; }
        public bool suppressedRentedPrice { get; set; }
    }

    public class LastSaleActivity
    {
        public DateTime date { get; set; }
        public bool documentedAsSold { get; set; }
        public int price { get; set; }
        public bool reportedAsSold { get; set; }
        public string type { get; set; }
        public bool suppressPrice { get; set; }
        public CapitalGrowth capitalGrowth { get; set; }
        public RentalYield rentalYield { get; set; }
        public long activityId { get; set; }
    }

    public class AdvertHistory
    {
        public int advertId { get; set; }
        public int agencyId { get; set; }
        public bool canDisplayPrice { get; set; }
        public DateTime actionDate { get; set; }
        public DateTime advertisedDate { get; set; }
        public string soldAction { get; set; }
        public string saleMethod { get; set; }
        public int price { get; set; }
        public string status { get; set; }
        public string objective { get; set; }
        public string rentalMethod { get; set; }
    }

    public class PropertyObject
    {
        public string address { get; set; }
        public AddressCoordinate addressCoordinate { get; set; }
        public int addressId { get; set; }
        public List<object> adverts { get; set; }
        public int areaSize { get; set; }
        public int bathrooms { get; set; }
        public int bedrooms { get; set; }
        public Cadastre cadastre { get; set; }
        public string cadastreType { get; set; }
        public int carSpaces { get; set; }
        public DateTime created { get; set; }
        public List<string> features { get; set; }
        public string flatNumber { get; set; }
        public History history { get; set; }
        public string id { get; set; }
        public bool isResidential { get; set; }
        public int legacyPropertyId { get; set; }
        public string lotNumber { get; set; }
        public List<object> onMarketTypes { get; set; }
        public int pdsId { get; set; }
        public List<Photo> photos { get; set; }
        public string planNumber { get; set; }
        public string postcode { get; set; }
        public string propertyCategory { get; set; }
        public int propertyCategoryId { get; set; }
        public string propertyType { get; set; }
        public int propertyTypeId { get; set; }
        public string state { get; set; }
        public string streetAddress { get; set; }
        public string streetName { get; set; }
        public string streetNumber { get; set; }
        public string streetType { get; set; }
        public string streetTypeLong { get; set; }
        public string suburb { get; set; }
        public int suburbId { get; set; }
        public List<Timeline> timeline { get; set; }
        public DateTime updated { get; set; }
        public string urlSlug { get; set; }
        public string urlSlugShort { get; set; }
        public Valuation2 valuation { get; set; }
        public string zone { get; set; }
        public string defaultPhotoUrl { get; set; }
        public string occupancy { get; set; }
        public LastRentActivity lastRentActivity { get; set; }
        public LastSaleActivity lastSaleActivity { get; set; }
        public string flatNumberSortKey { get; set; }
        public string streetNumberSortKey { get; set; }
        public List<AdvertHistory> advertHistory { get; set; }
    }
}
