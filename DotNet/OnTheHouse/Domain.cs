using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;


namespace OnTheHouse
{
    public static class Domain
    {
        public static string baseUrl = @"https://www.domain.com.au/property-profile/";
        public static SearchResult SearchDomain(this ChromeDriver chromeDriver, Property property)
        {
            var address = property.BuildDomainAddress();

            chromeDriver.Url = $@"{baseUrl}{address}";

            chromeDriver.Navigate();

            SearchResult result = new SearchResult();

            var contentWrap = chromeDriver.TryFindElementByCss("section.content-wrap");

            if(contentWrap != null && contentWrap.Text.Contains("Sorry, this page cannot be found."))
            {
                result.InvalidAddress = true;
                return result;
            }

            chromeDriver.TryFindElementById("terms-modal-agree")?.Click();

            chromeDriver.FindElementsByCssSelector("button.button.button__muted")?
                .Where(btn => btn.Text == "View more results")
                .FirstOrDefault()?
                .Click();

            var feature = chromeDriver.TryFindElementByCss("div.property-details-strip__features-wrap");
            if(feature != null)
            {
                var values = feature.TryFindElementsByCss("div.property-details-strip__feature-value");
                if (values != null && values .Count == 3)
                {
                    result.Bedroom = values[0]?.Text;
                    result.Bathroom = values[1]?.Text;
                    result.Parking = values[2]?.Text;
                }

            }

            var estimate = chromeDriver.TryFindElementByCss("div.estimate-range__values");
            if(estimate != null)
            {
                result.LowValue =  estimate.TryFindElementByCss("span.short-price-display.low-end")?.TryGetInnerText(chromeDriver);
                result.MidValue = estimate.TryFindElementByCss("span.short-price-display.mid-point")?.TryGetInnerText(chromeDriver);
                result.HighValue = estimate.TryFindElementByCss("span.short-price-display.high-end")?.TryGetInnerText(chromeDriver);

            }

            //var viewMoreButtons = chromeDriver.FindElementsByCssSelector("button.button.button__muted");
            //if (viewMoreButtons.Any())
            //{
            //    var viewMore = viewMoreButtons.Where(btn => btn.Text == "View more results");
            //    if (viewMore.Any())
            //    {
            //        viewMore.First().Click();
            //    }
            //}

            var historyEntries = chromeDriver.FindElementsByCssSelector("li.property-timeline-item");
            var dataHistory = new List<HouseHistory>();

            foreach (var history in historyEntries)
            {
                var month = history.TryFindElementByCss("div.property-timeline__card-date-month");
                var year = history.TryFindElementByCss("div.property-timeline__card-date-year");
                var action = history.TryFindElementByCss("span.property-timeline__card-category");
                var price = history.TryFindElementByCss("span.property-timeline__card-heading");

                if(year != null && price != null && action != null)
                dataHistory.Add(new HouseHistory()
                {
                    Action = action?.Text,
                    Date = $"{year?.Text}-{month?.Text}",
                    Value = price?.Text
                });
            }

            

            result.PropertyStory = chromeDriver.TryFindElementByCss("div.proprty-story")?.Text;
            result.Data = dataHistory;

            return result;
        }
    }
}
