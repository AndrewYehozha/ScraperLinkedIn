using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
        private LoggerService _loggerService;

        private int CompanyBatchSize;
        private int ProfileBatchSize;
        private string Login;
        private string Password;

        public Scraper()
        {
            _companyService = new CompaniesService();
            _dataService = new DataService();
            _profilesService = new ProfilesService();
            _loggerService = new LoggerService();
        }

        public void Initialize()
        {
            try
            {
                if (driver == null)
                {
                    var options = new ChromeOptions();
                    options.AddArgument("no-sandbox");

                    driver = new ChromeDriver(options);
                    js = (IJavaScriptExecutor)driver;

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    driver.Manage().Window.Maximize();
                }
            }
            catch (Exception ex)
            {
                _loggerService.Add("Error initialize scraper", ex.ToString());
            }
        }

        public void Run()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["COMPANY_BATCH_SIZE"], out CompanyBatchSize))
            {
                _loggerService.Add("Error", $"Company batch size must be an integer. Please, check the value of <COMPANY_BATCH_SIZE> in App.config.");
                return;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["PROFILE_BATCH_SIZE"], out ProfileBatchSize))
            {
                _loggerService.Add("Error", $"Company batch size must be an integer. Please, check the value of <PROFILE_BATCH_SIZE> in App.config.");
                return;
            }

            Login = ConfigurationManager.AppSettings["LOGIN"];
            Password = ConfigurationManager.AppSettings["PASSWORD"];

            _loggerService.Add("Connecting to LinkedIn...", "");

            var cookie = new Cookie("li_at", ConfigurationManager.AppSettings["TOKEN"], ".www.linkedin.com", "/", DateTime.Now.AddDays(7));

            try
            {
                driver.Navigate().GoToUrl("https://www.linkedin.com");
                driver.Manage().Cookies.AddCookie(cookie);
                driver.Navigate().Refresh();

                try // Validation of the entered token
                {
                    var profileName = driver.FindElement(By.ClassName("profile-rail-card__actor-link")).Text;
                    _loggerService.Add($"Connected successfully as", profileName);
                }
                catch
                {
                    _loggerService.Add("Invalid Token. Please, check the value of <TOKEN> in App.config.");

                    if (CheckBrowserErrors() || !SignIn())
                    {
                        Close();
                        return;
                    }
                }

                _loggerService.Add("Start scraped data processing", "");

                //Scraped data processing
                var searchCompanies = _companyService.GetCompaniesForSearch();
                _dataService.SearchSuitableDirectorsCompanies(searchCompanies);

                //Scraper process
                _loggerService.Add("Start scraper process", "");

                var rawProfilesCount = _profilesService.GetCountRawProfiles();

                if (rawProfilesCount < ProfileBatchSize)
                {
                    var newPofilesCount = _profilesService.GetCountNewProfiles();

                    while (newPofilesCount <= ProfileBatchSize * 1.6)
                    {
                        var companies = _companyService.GetCompanies(CompanyBatchSize);
                        GetCompaniesEmployees(companies);

                        newPofilesCount = _profilesService.GetCountNewProfiles();
                    }
                }
                else
                {
                    ProfileBatchSize *= 2;
                }

                var profiles = _profilesService.GetProfiles(ProfileBatchSize);
                GetEmployeeProfiles(profiles);

                _loggerService.Add("End scraper process", "");
                Close();
            }
            catch (Exception ex)
            {
                _loggerService.Add("Error Run scraper", ex.ToString());
            }
        }

        private void GetCompaniesEmployees(IEnumerable<CompanyEmployeesViewModel> companies)
        {
            _loggerService.Add("Companies URLs to scrape", $"[\n{ string.Join(",\n", companies.Select(x => $"\t{ x.LinkedIn }")) }\n]");

            foreach (var company in companies)
            {
                Thread.Sleep(30000); //A break between requests.

                try
                {
                    _loggerService.Add("Getting employees for company with url", company.LinkedIn);
                    driver.Navigate().GoToUrl(company.LinkedIn);
                }
                catch (WebDriverException ex)
                {
                    driver.FindElement(By.TagName("body")).SendKeys("Keys.ESCAPE");
                    _loggerService.Add($"Error opening company page with url: { company.LinkedIn }", ex.ToString());
                }

                if (CheckBrowserErrors() || (!CheckAuthorization() && !SignIn()))
                {
                    return;
                }

                try
                {
                    driver.FindElement(By.ClassName("nav-header__guest-nav"));
                    _loggerService.Add("Find and apply for a job to contact and learn more about this company", company.LinkedIn);

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("not-found__main-heading")); // Not found
                    _loggerService.Add("Could not scrape company, because this page was not found", company.LinkedIn);

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("profile-unavailable")); // Not available
                    _loggerService.Add("Could not scrape company, because this profile is not available", company.LinkedIn);

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("error-container")); // Stop searching if incorrect url
                    _loggerService.Add("Could not scrape company, because the page could not be loaded", company.LinkedIn);

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

                try
                {
                    driver.FindElement(By.CssSelector("a[data-control-name='page_member_main_nav_about_tab']")).Click();
                }
                catch
                {
                    _loggerService.Add("Error: \"This isn't a company\"", company.LinkedIn); //This isn't a company

                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    _companyService.UpdateCompany(company);

                    continue;
                }

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
                    _loggerService.Add("Error: Could not scrape company because No employees found", company.LinkedIn);
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
                        _loggerService.Add("This page not found", company.LinkedIn);

                        continue;
                    }
                    catch { }

                    for (int i = 1; ; i++)
                    {
                        driver.Navigate().GoToUrl($"{ paginationUrl }&page={ i }"); // Pagination Employees
                        js.ExecuteScript("window.scrollBy(0,1000)");
                        Thread.Sleep(2000); // Waiting for page to load

                        if (CheckBrowserErrors() || (!CheckAuthorization() && !SignIn()))
                        {
                            _companyService.UpdateCompany(company);
                            return;
                        }

                        try
                        {
                            driver.FindElement(By.ClassName("search-no-results__container")); // Stop searching if next page is missing
                            _loggerService.Add("Scrap All pages for company", company.LinkedIn);

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

                        _loggerService.Add($"Getting employees for page { i }...");
                    }
                }

                company.ExecutionStatus = ExecutionStatuses.Success;
                _companyService.UpdateCompany(company);
            }
        }

        private void GetEmployeeProfiles(IEnumerable<ProfileViewModel> employees)
        {
            _loggerService.Add("Profiles URLs to scrape", $"[\n{ string.Join(",\n", employees.Select(x => $"\t{ x.ProfileUrl }")) }\n]");

            foreach (var employee in employees)
            {
                Thread.Sleep(5000); //A break between requests.

                try
                {
                    _loggerService.Add("Opening profile", employee.ProfileUrl);
                    driver.Navigate().GoToUrl(employee.ProfileUrl);
                }
                catch (WebDriverException ex)
                {
                    driver.FindElement(By.TagName("body")).SendKeys("Keys.ESCAPE");
                    _loggerService.Add($"Error opening profile page with url: { employee.ProfileUrl }", ex.ToString());
                }

                if (CheckBrowserErrors() || (!CheckAuthorization() && !SignIn()))
                {
                    return;
                }

                try
                {
                    driver.FindElement(By.ClassName("profile-unavailable")); // Stop searching if incorrect url
                    _loggerService.Add("Could not scrape profile, because this profile is not available", employee.ProfileUrl);

                    employee.ExecutionStatus = ExecutionStatuses.Failed;
                    _profilesService.UpdateProfile(employee);

                    continue;
                }
                catch { }

                _loggerService.Add("Scrolling to load all data of the profile...");

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

                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        try
                        {
                            js.ExecuteScript("window.scrollBy(0,750)");
                            driver.FindElement(By.ClassName("pv-skill-category-entity__name-text"));

                            try
                            {
                                js.ExecuteScript("window.scrollBy(0,200)");
                                Thread.Sleep(500);
                                driver.FindElement(By.ClassName("pv-skills-section__additional-skills")).Click(); //Find Show more button
                                js.ExecuteScript("window.scrollBy(0,250)");
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
                    _loggerService.Add("All data loaded");
                }
                else
                {
                    _loggerService.Add("Could not scrape because: Could not load the profile", employee.ProfileUrl);
                    employee.ExecutionStatus = ExecutionStatuses.Failed;
                }

                _profilesService.UpdateProfile(employee);
            }
        }

        private bool CheckBrowserErrors()
        {
            try
            {
                var errorElement = driver.FindElement(By.CssSelector("div .error-code"));
                _loggerService.Add("Error", errorElement.Text);

                return true;
            }
            catch { }

            return false;
        }

        private bool CheckAuthorization()
        {
            try
            {
                driver.FindElement(By.CssSelector(".join-form-container,.login-form"));
                _loggerService.Add("Error", "Invalid Token. Please, check the value of <TOKEN> in App.config.");

                return false;
            }
            catch { }

            return true;
        }

        private bool SignIn()
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.linkedin.com/login?fromSignIn=true&trk=guest_homepage-basic_nav-header-signin");

                Thread.Sleep(3000); //Loading Sign in page

                driver.FindElement(By.Id("username")).Clear();
                driver.FindElement(By.Id("username")).SendKeys(Login); //Authorization process
                Thread.Sleep(500);
                driver.FindElement(By.Id("password")).SendKeys(Password);
                Thread.Sleep(500);
                driver.FindElement(By.ClassName("btn__primary--large")).Click();
            }
            catch (Exception ex)
            {
                _loggerService.Add("Error authorization", ex.ToString());
                return false;
            }

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
            catch (Exception ex)
            {
                _loggerService.Add("Error", ex.ToString());
            }
        }
    }
}