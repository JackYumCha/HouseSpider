using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace OnTheHouse
{
    public static class WebElementExtensions
    {
        public static IWebElement TryFindElementByCss(this IWebElement webElement, string cssSelector)
        {
            try
            {
                return webElement.FindElement(By.CssSelector(cssSelector));
            }
            catch(NoSuchElementException ex)
            {
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement> TryFindElementsByCss(this IWebElement webElement, string cssSelector)
        {
            try
            {
                return webElement.FindElements(By.CssSelector(cssSelector));
            }
            catch (NoSuchElementException ex)
            {
                return null;
            }
        }


        public static IWebElement TryFindElementById(this ChromeDriver chromeDriver, string id)
        {
            try
            {
                return chromeDriver.FindElementById(id);
            }
            catch (NoSuchElementException ex)
            {
                return null;
            }
        }

        public static IWebElement TryFindElementByCss(this ChromeDriver chromeDriver, string cssSelector)
        {
            try
            {
                return chromeDriver.FindElementByCssSelector(cssSelector);
            }
            catch (NoSuchElementException ex)
            {
                return null;
            }
        }

        public static Regex TagRegex = new Regex("<[^>^<]+>");

        public static string TryGetInnerText(this IWebElement webElement, ChromeDriver chromeDriver)
        {
            try
            {
                IJavaScriptExecutor js = chromeDriver as IJavaScriptExecutor;
                return TagRegex.Replace((string)js.ExecuteScript("return arguments[0].innerHTML;", webElement), "");
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
