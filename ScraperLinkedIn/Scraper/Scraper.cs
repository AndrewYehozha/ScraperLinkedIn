﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScraperLinkedIn.Email;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Services;
using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.Scrapers
{
    class Scraper
    {
        //https://sites.google.com/a/chromium.org/chromedriver/downloads
        private IWebDriver driver;
        private IJavaScriptExecutor js;

        private CompaniesService _companyService;
        private ProfilesService _profilesService;
        private DataService _dataService;

        private int CompanyBatchSize;
        private int ProfileBatchSize;
        private string Login;
        private string Password;

        private const string Line = "-----------------------------------------------------------------------";

        public Scraper()
        {
            _companyService = new CompaniesService();
            _dataService = new DataService();
            _profilesService = new ProfilesService();
        }

        public void Initialize()
        {
            try
            {
                if (driver == null)
                {
                    Console.WriteLine($"\n{ Line }");

                    var options = new ChromeOptions();
                    options.AddArgument("no-sandbox");

                    driver = new ChromeDriver(options);
                    js = (IJavaScriptExecutor)driver;

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    driver.Manage().Window.Maximize();

                    Console.WriteLine($"{ Line }\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Error initialize scraper:\n{ Line }\n{ ex }");
                Console.WriteLine($"{ Line }\n\n");
            }
        }

        public void Run()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["COMPANY_BATCH_SIZE"], out CompanyBatchSize))
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Company batch size must be an integer.\n{ Line }\nPlease, check the value of <COMPANY_BATCH_SIZE> in App.config.");
                Console.WriteLine($"{ Line }\n\n");
                return;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["PROFILE_BATCH_SIZE"], out ProfileBatchSize))
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Company batch size must be an integer.\n{ Line }\nPlease, check the value of <PROFILE_BATCH_SIZE> in App.config.");
                Console.WriteLine($"{ Line }\n\n");
                return;
            }

            Login = ConfigurationManager.AppSettings["LOGIN"];
            Password = ConfigurationManager.AppSettings["PASSWORD"];

            Console.WriteLine($"Connecting to LinkedIn...");

            var cookie = new Cookie("li_at", ConfigurationManager.AppSettings["TOKEN"], ".www.linkedin.com", "/", DateTime.Now.AddDays(7));

            driver.Navigate().GoToUrl("https://www.linkedin.com");
            driver.Manage().Cookies.AddCookie(cookie);
            driver.Navigate().Refresh();

            try // Validation of the entered token
            {
                var profileName = driver.FindElement(By.ClassName("profile-rail-card__actor-link")).Text;
                Console.WriteLine($"Connected successfully as { profileName }");
            }
            catch
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Invalid Token.\n{ Line }\nPlease, check the value of <TOKEN> in App.config.");
                Console.WriteLine($"{ Line }\n\n");

                if (!SignIn())
                {
                    Close();
                    return;
                }
            }

            //Scraped data processing
            var searchCompanies = _companyService.GetCompaniesForSearch();
            _dataService.SearchSuitableDirectorsCompanies(searchCompanies);


            //Scraper process

            //var rawProfilesCount = _profilesService.GetCountRawProfiles();

            //if (rawProfilesCount < 200)
            //{
            var newPofilesCount = _profilesService.GetCountNewProfiles();

            while (newPofilesCount <= ProfileBatchSize)
            {
                var companies = _companyService.GetCompanies(CompanyBatchSize);
                GetCompaniesEmployees(companies);

                newPofilesCount = _profilesService.GetCountNewProfiles();
            }
            //}
            //else
            //{
            var profiles = _profilesService.GetProfiles(ProfileBatchSize);
            GetEmployeeProfiles(profiles);
            //}

            Close();
        }

        private void GetCompaniesEmployees(IEnumerable<CompanyEmployeesViewModel> companies)
        {
            Console.WriteLine($"\nCompanies URLs to scrape: [\n{ string.Join(",\n", companies.Select(x => $"\t{ x.LinkedIn }")) }\n]");

            foreach (var company in companies)
            {
                Thread.Sleep(30000); //A break between requests.

                Console.WriteLine($"\n\nGetting employees for company with url: { company.LinkedIn }");
                driver.Navigate().GoToUrl(company.LinkedIn);

                if (!CheckAuthorization() && !SignIn())
                {
                    return;
                }

                try
                {
                    driver.FindElement(By.ClassName("nav-header__guest-nav"));
                    Console.WriteLine($"Find and apply for a job to contact { company.LinkedIn } and learn more about this company");

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("not-found__main-heading")); // Not found
                    Console.WriteLine($"Could not scrape company, because { company.LinkedIn } page was not found");

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("profile-unavailable")); // Not available
                    Console.WriteLine($"Could not scrape company, because this profile { company.LinkedIn } is not available");

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("error-container")); // Stop searching if incorrect url
                    Console.WriteLine($"Could not scrape company, because the page { company.LinkedIn } could not be loaded");

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    var logoCompanyUrlTemp = driver.FindElement(By.ClassName("org-top-card-primary-content__logo")).GetAttribute("src");

                    company.LogoCompanyUrl = logoCompanyUrlTemp.Contains("https://media.licdn.com") ? logoCompanyUrlTemp : "";
                }
                catch { }

                driver.FindElement(By.CssSelector("a[data-control-name='page_member_main_nav_about_tab']")).Click();

                try
                {
                    if ((new List<string> { "Специализация", "Specialties" }).Contains(driver.FindElement(By.CssSelector(".overflow-hidden dt:last-of-type")).Text))
                    {
                        company.Specialties = driver.FindElement(By.CssSelector(".overflow-hidden dd:last-of-type")).Text;
                    }
                }
                catch { }

                try
                {
                    driver.FindElement(By.CssSelector("div > .link-without-visited-state")).Click(); // Watch all people
                }
                catch
                {
                    Console.WriteLine($"Error: Could not scrape company { company.LinkedIn } because No employees found");
                    company.ExecutionStatus = ExecutionStatuses.Success;
                }

                if (company.ExecutionStatus == ExecutionStatuses.Queued)
                {
                    Thread.Sleep(2000); // Waiting for page to load
                    var paginationUrl = driver.Url;

                    driver.Navigate().GoToUrl($"{ paginationUrl }&page=1");

                    try
                    {
                        driver.FindElement(By.ClassName("not-found")); // Page not found
                        Console.WriteLine($"Page { company.LinkedIn } not found");
                        continue;
                    }
                    catch { }

                    for (int i = 1; ; i++)
                    {
                        driver.Navigate().GoToUrl($"{ paginationUrl }&page={ i }"); // Pagination Employees
                        js.ExecuteScript("window.scrollBy(0,1000)");
                        Thread.Sleep(2000); // Waiting for page to load

                        if (!CheckAuthorization() && !SignIn())
                        {
                            _companyService.UpdateCompany(company);
                            return;
                        }

                        try
                        {
                            driver.FindElement(By.ClassName("search-no-results__container")); // Stop searching if next page is missing
                            Console.WriteLine($"Scrap All pages for company: { company.LinkedIn }");
                            break;
                        }
                        catch { }

                        js.ExecuteScript("window.scrollBy(0,1000)");
                        Thread.Sleep(1000); // Waiting for page to load

                        foreach (var item in driver.FindElements(By.CssSelector(".search-result__info > a"))) // Link selection for employees
                        {
                            string href = item.GetAttribute("href");
                            if (href != $"{ paginationUrl }&page={ i }#")
                            {
                                company.Employees.Add(new ProfileViewModel { ProfileUrl = item.GetAttribute("href"), CompanyID = company.Id });
                            }
                        }

                        Console.WriteLine($"Getting employees for page { i }...");
                    }
                }

                company.ExecutionStatus = ExecutionStatuses.Success;
                _companyService.UpdateCompany(company);
            }
        }

        private void GetEmployeeProfiles(IEnumerable<ProfileViewModel> employees)
        {
            Console.WriteLine($"\nProfiles URLs to scrape: [\n{ string.Join(",\n", employees.Select(x => $"\t{ x.ProfileUrl }")) }\n]");

            foreach (var employee in employees)
            {
                Thread.Sleep(5000); //A break between requests.

                Console.WriteLine($"\n\nOpening profile: { employee.ProfileUrl }");
                driver.Navigate().GoToUrl(employee.ProfileUrl);

                if (!CheckAuthorization() && !SignIn())
                {
                    return;
                }

                try
                {
                    driver.FindElement(By.ClassName("profile-unavailable")); // Stop searching if incorrect url
                    Console.WriteLine($"Could not scrape profile, because this profile { employee.ProfileUrl } is not available");

                    employee.ExecutionStatus = ExecutionStatuses.Failed;
                    _profilesService.UpdateProfile(employee);

                    continue;
                }
                catch { }

                Console.WriteLine($"Scrolling to load all data of the profile...");

                try
                {
                    employee.FullName = driver.FindElement(By.CssSelector(".pv-top-card-v3--list .break-words")).Text;
                    employee.FirstName = employee.FullName.Split(' ')[0];
                    employee.LastName = employee.FullName.Split(' ')[1];
                }
                catch { }

                try
                {
                    employee.Job = driver.FindElement(By.CssSelector(".flex-1 h2")).Text;
                }
                catch
                {
                    employee.Job = string.Empty;
                }

                employee.AllSkills = string.Empty;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        try
                        {
                            js.ExecuteScript("window.scrollBy(0,500)");
                            Thread.Sleep(1000);

                            driver.FindElement(By.ClassName("pv-skill-category-entity__name-text"));

                            try
                            {
                                driver.FindElement(By.ClassName("pv-skills-section__additional-skills")).Click(); //Find Show more button
                            }
                            catch { }
                        }
                        catch { continue; }

                        Thread.Sleep(500); // Waiting for loading block skills

                        var arrSkills = new List<string>();
                        foreach (var skill in driver.FindElements(By.ClassName("pv-skill-category-entity__name-text")))
                        {
                            arrSkills.Add(skill.Text);
                        }

                        employee.AllSkills = string.Join(",", arrSkills) ?? "";
                        break;
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(employee.AllSkills) || !string.IsNullOrEmpty(employee.Job))
                {
                    Console.WriteLine($"All data loaded");
                }
                else
                {
                    employee.ExecutionStatus = ExecutionStatuses.Failed;

                    Console.WriteLine($"Could not scrape { employee.ProfileUrl } because: Could not load the profile");
                }

                _profilesService.UpdateProfile(employee);
            }
        }

        private bool CheckAuthorization()
        {
            try
            {
                driver.FindElement(By.CssSelector(".join-form-container,.login-form"));

                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Invalid Token.\n{ Line }\nPlease, check the value of <TOKEN> in App.config.");
                Console.WriteLine($"{ Line }\n\n");

                return false;
            }
            catch { }

            return true;
        }

        private bool SignIn()
        {
            driver.Navigate().GoToUrl("https://www.linkedin.com/login?fromSignIn=true&trk=guest_homepage-basic_nav-header-signin");

            Thread.Sleep(2000); //Loading Sign in page

            driver.FindElement(By.Id("username")).SendKeys(Login); //Authorization process
            Thread.Sleep(500);
            driver.FindElement(By.Id("password")).SendKeys(Password);
            Thread.Sleep(500);
            driver.FindElement(By.ClassName("btn__primary--large")).Click();

            try
            {
                Thread.Sleep(2000); //Wait authorization
                driver.FindElement(By.Id("username"));

                return false;
            }
            catch { }

            return true;
        }

        public void Close()
        {
            try
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
            catch { }
        }
    }
}