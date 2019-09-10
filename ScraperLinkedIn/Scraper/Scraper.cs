using System;
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
        private int AmountProfiles;

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

            if (!int.TryParse(ConfigurationManager.AppSettings["AMOUNT_PROFILES"], out AmountProfiles))
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Company batch size must be an integer.\n{ Line }\nPlease, check the value of <AMOUNT_PROFILES> in App.config.");
                Console.WriteLine($"{ Line }\n\n");
                return;
            }

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

                Close();
                return;
            }

            var result = new List<ResultViewModel>();

            //Scraper process
            while (AmountProfiles >= 0) //TODO
            {
                var companies = _companyService.GetCompanies(CompanyBatchSize);
                GetCompaniesEmployees(companies);

                var profiles = _profilesService.GetProfiles(ProfileBatchSize);
                GetEmployeeProfiles(profiles);

                foreach (var item in _dataService.SearchSuitableDirectorsCompanies(new List<CompanyEmployeesViewModel>()))
                {
                    result.Add(item);
                    AmountProfiles--;
                }
            }
            _dataService.SaveToXLSXFile(result);
            Close();
        }

        private void GetCompaniesEmployees(IEnumerable<CompanyEmployeesViewModel> companies)
        {
            Console.WriteLine($"\nCompanies:\n[\n{ string.Join(",\n", companies.Select(x => x.LinkedIn)) }\n]");

            foreach (var company in companies)
            {
                Thread.Sleep(30000); //A break between requests.

                Console.WriteLine($"\n\nGetting employees for company with url: { company.LinkedIn }");
                driver.Navigate().GoToUrl(company.LinkedIn);

                try
                {
                    driver.FindElement(By.ClassName("join-form-container"));

                    Console.WriteLine($"\n{ Line }");
                    Console.WriteLine($"Invalid Token.\n{ Line }\nPlease, check the value of <TOKEN> in App.config.");
                    Console.WriteLine($"{ Line }\n\n");

                    _companyService.UpdateCompaniesWithStatusFailed(companies);
                    return;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("not-found__main-heading")); // Not found
                    Console.WriteLine($"Could not scrape company, because { company.LinkedIn } page was not found");
                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("error-container")); // Stop searching if incorrect url
                    Console.WriteLine($"Could not scrape company, because the page { company.LinkedIn } could not be loaded");
                    company.ExecutionStatus = ExecutionStatuses.Failed;
                    continue;
                }
                catch { }

                try
                {
                    var logoCompanyUrlTemp = driver.FindElement(By.ClassName("org-top-card-primary-content__logo")).GetAttribute("src");
                    if (logoCompanyUrlTemp.Contains("https://media.licdn.com"))
                    {
                        company.LogoCompanyUrl = logoCompanyUrlTemp;
                    }
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

                var employees = new List<ProfileViewModel>();

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

                        try
                        {
                            driver.FindElement(By.ClassName("join-form-container"));

                            Console.WriteLine($"\n{ Line }");
                            Console.WriteLine($"Invalid Token.\n{ Line }\nPlease, check the value of <TOKEN> in App.config.");
                            Console.WriteLine($"{ Line }\n\n");

                            company.Employees = employees;
                            _companyService.UpdateCompany(company);
                            return;
                        }
                        catch { }

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
                                employees.Add(new ProfileViewModel { ProfileUrl = item.GetAttribute("href"), CompanyID = company.Id });
                            }
                        }

                        Console.WriteLine($"Getting employees for page { i }...");
                    }
                }

                company.Employees = employees;
                company.ExecutionStatus = ExecutionStatuses.Success;

                _companyService.UpdateCompany(company);
            }
        }

        private void GetEmployeeProfiles(IEnumerable<ProfileViewModel> employees)
        {
            Console.WriteLine($"\nCompanies:\n[\n{ string.Join(",\n", employees.Select(x => x.ProfileUrl)) }\n]");

            foreach (var employee in employees)
            {
                Thread.Sleep(5000); //A break between requests.

                Console.WriteLine($"\n\nOpening profile: { employee.ProfileUrl }");
                driver.Navigate().GoToUrl(employee.ProfileUrl);

                try
                {
                    driver.FindElement(By.ClassName("join-form-container"));

                    Console.WriteLine($"\n{ Line }");
                    Console.WriteLine($"Invalid Token.\n{ Line }\nPlease, check the value of <TOKEN> in App.config.");
                    Console.WriteLine($"{ Line }\n\n");

                    _profilesService.UpdateProfilesWithStatusFailed(employees);
                    return;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("profile-unavailable")); // Stop searching if incorrect url
                    Console.WriteLine($"Could not scrape profile, because this profile { employee.ProfileUrl } is not available");
                    employee.ExecutionStatusID = ExecutionStatuses.Failed;
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
                    employee.Job = driver.FindElement(By.CssSelector(".flex-1 h2")).Text ?? "";
                }
                catch { }

                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        js.ExecuteScript("window.scrollBy(0,500)");
                        Thread.Sleep(1000);

                        try
                        {
                            driver.FindElement(By.ClassName("pv-skills-section__additional-skills")).Click(); //Find Show more button
                        }
                        catch { }

                        Thread.Sleep(750); // Waiting for loading block skills

                        try
                        {
                            driver.FindElement(By.ClassName("pv-skill-category-entity__name-text"));

                            js.ExecuteScript("window.scrollBy(0,500)");
                            Thread.Sleep(1000);

                            try
                            {
                                driver.FindElement(By.ClassName("pv-skills-section__additional-skills")).Click(); //Find Show more button
                            }
                            catch { }
                        }
                        catch { continue; }

                        var arrSkills = new List<string>();
                        foreach (var skill in driver.FindElements(By.ClassName("pv-skill-category-entity__name-text")))
                        {
                            arrSkills.Add(skill.Text);
                        }

                        employee.AllSkills = string.Join(", ", arrSkills) ?? "";
                        break;
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(employee.AllSkills) || !string.IsNullOrEmpty(employee.Job))
                {
                    employee.ExecutionStatusID = ExecutionStatuses.Success;

                    Console.WriteLine($"All data loaded");
                }
                else
                {
                    employee.ExecutionStatusID = ExecutionStatuses.Failed;

                    Console.WriteLine($"Could not scrape { employee.ProfileUrl } because: Could not load the profile");
                }

                _profilesService.UpdateProfile(employee);
            }
        }

        public void Close()
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