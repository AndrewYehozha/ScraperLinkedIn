using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ScraperLinkedIn
{
    class Scraper
    {
        //https://sites.google.com/a/chromium.org/chromedriver/downloads
        private IWebDriver driver;
        private IWait<IWebDriver> wait;
        //private string token = "Test";

        public void Initialize()
        {
            while (true)
            {
                Console.Write("Enter session cookie (li_at): ");
                Cookie cookie = new Cookie("li_at", Console.ReadLine(), ".www.linkedin.com", "/", DateTime.Now.AddDays(7));
                Console.Write("\n\n");
                //Cookie cookie = new Cookie("li_at", token, ".www.linkedin.com", "/", DateTime.Now.AddDays(7));

                driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://www.linkedin.com");
                driver.Manage().Cookies.AddCookie(cookie);
                driver.Navigate().Refresh();
                try
                {
                    driver.FindElement(By.ClassName("sign-in-form"));
                    Console.WriteLine("\n\n\nWarning: 'Incorrect session cookie (li_at)'.\n");
                    Close();
                }
                catch { break; }
            }
            
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            GetCompaniesEmployees();
            Close();
        }

        private void GetCompaniesEmployees()
        {
            string[] urls = new string[] { "https://www.linkedin.com/company/fullbridge/", "https://www.linkedin.com/company/2713364" };

            foreach (var url in urls)
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);

                try
                {
                    driver.FindElement(By.CssSelector("div > .link-without-visited-state")).Click();
                }
                catch
                {
                    Console.WriteLine($"\nCould not scrape company { url } because No employees found\n");
                    continue;
                }

                wait.Until(d => driver.FindElements(By.ClassName("artdeco-pagination__indicator")).Count > 0);
                var test = driver.FindElements(By.ClassName("artdeco-pagination__indicator"));

            }
        }

        private void Close()
        {
            if (driver != null)
            {
                driver.Close();
            }

            var chromeDrivers = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriver in chromeDrivers)
            {
                chromeDriver.Kill();
            }
        }
    }
}
