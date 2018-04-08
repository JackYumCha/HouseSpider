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
    public static class RealEstate
    {
        public static string baseUrl = @"https://www.realestate.com.au/property/";

        public static SearchResult SearchRealEstate(this ChromeDriver chromeDriver, Property property)
        {
            var address = property.BuildRealEstateAddress();

            chromeDriver.Url = $@"{baseUrl}{address}";

            chromeDriver.Navigate();

            SearchResult result = new SearchResult();

            if (chromeDriver.Url == @"https://www.realestate.com.au/property")
            {
                // invalid address
                result.InvalidAddress = true;
                return result;
            }


            var noHistory = chromeDriver.TryFindElementByCss("p.property-timeline__no-event__highlight");


            chromeDriver.TryFindElementByCss("div.property-timeline__collapse-button")?
                .TryFindElementByCss("button")?
                .Click();

            // find history 

            var dataHistory = new List<HouseHistory>();

            var historyEntries = chromeDriver.FindElementsByCssSelector("div.property-timeline__item-content");

            if (historyEntries.Any())
                foreach (var historyCard in historyEntries)
                {
                    var rentLabel = historyCard.TryFindElementByCss("span.property-timeline__label.property-timeline__label--rent");
                    var soldLabel = historyCard.TryFindElementByCss("span.property-timeline__label.property-timeline__label--sold");

                    //property-timeline__price
                    var price = historyCard.TryFindElementByCss("div.property-timeline__price");
                    //property-timeline__date
                    var date = historyCard.TryFindElementByCss("span.property-timeline__date");

                    if (rentLabel != null)
                    {
                        dataHistory.Add(new HouseHistory()
                        {
                            Action = "Rent",
                            Date = date.Text,
                            Value = price.Text
                        });
                    }
                    else if (soldLabel != null)
                    {
                        dataHistory.Add(new HouseHistory()
                        {
                            Action = "Sold",
                            Date = date.Text,
                            Value = price.Text
                        });
                    }
                }

            result.Data = dataHistory;

            // value range
            var summary = chromeDriver.TryFindElementByCss("div.property-summary");
            if(summary != null)
            {
                var attributes = summary.TryFindElementByCss("div.property-details").TryFindElementByCss("div.property-info").TryFindElementByCss("div.property-info__attributes");
                if (attributes != null)
                {
                    var atts = attributes.TryFindElementsByCss("span.rui-property-feature");
                    if(atts.Count == 3)
                    {
                        result.Bedroom = atts[0].TryFindElementByCss("span.config-num")?.Text;
                        result.Bathroom = atts[1].TryFindElementByCss("span.config-num")?.Text;
                        result.Parking = atts[2].TryFindElementByCss("span.config-num")?.Text;
                    }
                }

                result.ValueRange = summary.TryFindElementByCss("p.avm-price__estimate")?.Text;
            }

            // details;

            var detailTable = chromeDriver.FindElementsByCssSelector("div.property-details")?
                .Where(div => div.TryFindElementByCss("table.info-table") != null)?.FirstOrDefault();

            if(detailTable != null)
            {
                foreach(var row in detailTable.TryFindElementsByCss("tr"))
                {
                    var cells =  row.TryFindElementsByCss("td");
                    if(cells != null && cells.Count == 2)
                    {
                        switch (cells[0].Text)
                        {
                            case "Land size":
                                result.LandSize = cells[1].Text;
                                break;
                            case "Floor area":
                                result.FloorArea = cells[1].Text;
                                break;
                            case "Year built":
                                result.YearBuilt = cells[1].Text;
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
