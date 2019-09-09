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

namespace ScraperLinkedIn.Scrapers
{
    class Scraper
    {
        private EmailGenerator EmailGenerator { get; set; }

        //https://sites.google.com/a/chromium.org/chromedriver/downloads
        private IWebDriver driver;
        private IJavaScriptExecutor js;
        private CompanyService _companyService;
        private DataService _dataService;

        private int CompanyBatchSize;
        private int AmountProfiles;
        private int CountCompaniesEmployees = 0;
        private int CountCompanies = 1;
        private List<CompaniesEmployeesViewModel> companiesEmployees;

        private const string Line = "-----------------------------------------------------------------------";

        public void Initialize()
        {
            try
            {
                EmailGenerator = new EmailGenerator();
                _companyService = new CompanyService();
                _dataService = new DataService();

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
            catch(Exception ex)
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Error initialize scraper:\n{ Line }\n{ ex }");
                Console.WriteLine($"{ Line }\n\n");
            }
        }

        public void Run()
        {
            if (!Int32.TryParse(ConfigurationManager.AppSettings["COMPANY_BATCH_SIZE"], out CompanyBatchSize))
            {
                Console.WriteLine($"\n{ Line }");
                Console.WriteLine($"Company batch size must be an integer.\n{ Line }\nPlease, check the value of <COMPANY_BATCH_SIZE> in App.config.");
                Console.WriteLine($"{ Line }\n\n");
                return;
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["AMOUNT_PROFILES"], out AmountProfiles))
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
            companiesEmployees = new List<CompaniesEmployeesViewModel>();

            while (AmountProfiles >= 0) //TODO
            {
                var companies = _companyService.GetCompany();

                //Scraper process
                GetCompaniesEmployees(companies);
                GetEmployeeProfiles();

                foreach (var item in _dataService.SearchSuitableDirectorsCompanies(companiesEmployees))
                {
                    result.Add(item);
                    AmountProfiles--;
                }
            }
            _dataService.SaveToXLSXFile(result);
            Close();
        }

        private void GetCompaniesEmployees(IEnumerable<CompaniesEmployeesViewModel> companies)
        {
            Console.WriteLine($"\nCompanies:\n[\n{ string.Join(",\n", companies.Skip(CountCompanies).Take(CompanyBatchSize).Select(x => x.LinkedIn)) }\n]");

            foreach (var company in companies.Skip(CountCompanies).Take(CompanyBatchSize))
            {
                Thread.Sleep(30000); //A break between requests.

                Console.WriteLine($"\n\nGetting employees for company with url: { company.LinkedIn }");
                driver.Navigate().GoToUrl(company.LinkedIn);

                try
                {
                    driver.FindElement(By.ClassName("not-found__main-heading")); // Not found
                    Console.WriteLine($"Could not scrape company, because { company.LinkedIn } page was not found");
                    continue;
                }
                catch { }

                try
                {
                    driver.FindElement(By.ClassName("error-container")); // Stop searching if incorrect url
                    Console.WriteLine($"Could not scrape company, because the page { company.LinkedIn } could not be loaded");
                    continue;
                }
                catch { }

                string logoCompanyUrl = string.Empty;
                try
                {
                    var logoCompanyUrlTemp = driver.FindElement(By.ClassName("org-top-card-primary-content__logo")).GetAttribute("src");
                    if (logoCompanyUrlTemp.Contains("https://media.licdn.com"))
                    {
                        logoCompanyUrl = logoCompanyUrlTemp;
                    }
                }
                catch { }


                driver.FindElement(By.CssSelector("a[data-control-name='page_member_main_nav_about_tab']")).Click();

                string specialties = string.Empty;
                try
                {
                    if ((new List<string> { "Специализация", "Specialties" }).Contains(driver.FindElement(By.CssSelector(".overflow-hidden dt:last-of-type")).Text))
                    {
                        specialties = driver.FindElement(By.CssSelector(".overflow-hidden dd:last-of-type")).Text;
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
                    continue;
                }

                Thread.Sleep(2000); // Waiting for page to load
                var paginationUrl = driver.Url;
                var employees = new List<ProfilesViewModel>();

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
                            employees.Add(new ProfilesViewModel { ProfileUrl = item.GetAttribute("href") });
                        }
                    }

                    Console.WriteLine($"Getting employees for page { i }...");
                }

                companiesEmployees.Add(
                    new CompaniesEmployeesViewModel {
                        Founders = company.Founders,
                        HeadquartersLocation = company.HeadquartersLocation,
                        LinkedIn = company.LinkedIn,
                        LogoCompanyUrl = logoCompanyUrl,
                        OrganizationName = company.OrganizationName,
                        OrganizationNameURL = company.OrganizationNameURL,
                        Specialties = specialties,
                        Employees = employees,
                        Website = company.Website
                    }
                );

                CountCompanies++;
            }
        }

        private void GetEmployeeProfiles()
        {
            foreach (var companyEmployees in companiesEmployees.Skip(CountCompaniesEmployees))
            {
                foreach (var employee in companyEmployees.Employees)
                {
                    Thread.Sleep(10000); //A break between requests.

                    Console.WriteLine($"\n\nOpening profile: { employee.ProfileUrl }");
                    driver.Navigate().GoToUrl(employee.ProfileUrl);

                    try
                    {
                        driver.FindElement(By.ClassName("profile-unavailable")); // Stop searching if incorrect url
                        Console.WriteLine($"Could not scrape profile, because this profile { employee.ProfileUrl } is not available");
                        continue;
                    }
                    catch { }

                    Console.WriteLine($"Scrolling to load all data of the profile...");

                    string firstname = string.Empty;
                    string lastName = string.Empty;
                    string fullName = string.Empty;
                    try
                    {
                        fullName = driver.FindElement(By.CssSelector(".pv-top-card-v3--list .break-words")).Text;
                        firstname = fullName.Split(' ')[0];
                        lastName = fullName.Split(' ')[1];
                    }
                    catch { }

                    string job = string.Empty;
                    try
                    {
                        job = driver.FindElement(By.CssSelector(".flex-1 h2")).Text;
                    }
                    catch { }

                    string allSkills = string.Empty;
                    var arrSkills = new List<string>();


                    for (int i = 0; i < 12; i++)
                    {
                        try
                        {
                            js.ExecuteScript("window.scrollBy(0,500)");
                            Thread.Sleep(1000);

                            driver.FindElement(By.ClassName("pv-skills-section__additional-skills")).Click(); //Find Show more button
                            Thread.Sleep(750); // Waiting for loading block skills

                            foreach (var skill in driver.FindElements(By.ClassName("pv-skill-category-entity__name-text")))
                            {
                                arrSkills.Add(skill.Text);
                            }

                            allSkills = string.Join(", ", arrSkills);
                            break;
                        }
                        catch { }
                    }

                    if (!string.IsNullOrEmpty(allSkills.Trim()) || !string.IsNullOrEmpty(job.Trim()))
                    {
                        employee.FirstName = firstname;
                        employee.LastName = lastName;
                        employee.FullName = fullName;
                        employee.Job = job;
                        employee.Skills = arrSkills;
                        employee.AllSkills = allSkills;
                        employee.IsRead = true;

                        Console.WriteLine($"All data loaded");
                    }
                    else
                    {
                        Console.WriteLine($"Could not scrape { employee.ProfileUrl } because: Could not load the profile");
                    }
                }

                CountCompaniesEmployees++;
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