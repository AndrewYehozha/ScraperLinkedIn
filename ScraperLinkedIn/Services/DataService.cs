using OfficeOpenXml;
using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Email;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.SearchParameters;
using ScraperLinkedIn.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Youworks.Text;

namespace ScraperLinkedIn.Services
{
    class DataService
    {
        private EmailGenerator _emailGenerator;
        private ProfilesService _profilesService;
        private SuitableProfileService _suitableProfileService;

        public DataService()
        {
            _emailGenerator = new EmailGenerator();
            _profilesService = new ProfilesService();
            _suitableProfileService = new SuitableProfileService();
        }

        public void SearchSuitableDirectorsCompanies(IEnumerable<CompanyEmployeesViewModel> companiesEmployees)
        {
            foreach (var companyEmployees in companiesEmployees)
            {
                var result = new List<ResultViewModel>();
                var technologiesStack = new List<string>();

                var location = !string.IsNullOrEmpty(companyEmployees.HeadquartersLocation) ? companyEmployees.HeadquartersLocation.Split(',') : new string[] { };
                var profileSkills = companyEmployees.Employees.Where(x => x.ProfileStatus == ProfileStatuses.Developer).Select(z => z.AllSkills.Split(','));

                foreach (var skills in profileSkills)
                {
                    foreach (var skill in skills)
                    {
                        if (!technologiesStack.Contains(skill.Trim()))
                        {
                            technologiesStack.Add(skill.Trim());
                        }
                    }
                }

                if (profileSkills == null || profileSkills.Count() == 0)
                {
                    continue;
                }

                foreach (var employee in companyEmployees.Employees.Where(x => x.ProfileStatus == ProfileStatuses.Chief))
                {
                    var emails = !string.IsNullOrEmpty(employee.FirstName) && !string.IsNullOrEmpty(employee.LastName) ? _emailGenerator.GetValidEmails(employee.FirstName, employee.LastName, companyEmployees.Website) : new List<string>();

                    result.Add(
                        new ResultViewModel
                        {
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Job = RolesSearchParameters.Roles.Where(x => employee.Job.ToUpper().Split(' ').Contains(x)).FirstOrDefault(),
                            PersonLinkedIn = employee.ProfileUrl,
                            Company = companyEmployees.OrganizationName,
                            Website = companyEmployees.Website,
                            CompanyLogoUrl = companyEmployees.LogoCompanyUrl,
                            CrunchUrl = companyEmployees.OrganizationNameURL,
                            Email = emails.Count > 0 ? emails.FirstOrDefault() : "...",
                            EmailStatus = emails.Count > 0 && emails.Count < 4 ? "OK" : "...",
                            City = location.Count() > 0 ? location[0] : "...",
                            State = location.Count() > 1 ? location[1] : "...",
                            Country = location.Count() > 2 ? location[2] : "...",
                            PhoneNumber = companyEmployees.PhoneNumber,
                            AmountEmployees = companyEmployees.NumberofEmployees,
                            Industry = companyEmployees.Categories,
                            Twitter = companyEmployees.Twitter,
                            Facebook = companyEmployees.Facebook,
                            //CompanySpecialties = companyEmployees.Specialties,
                            TechStack = string.Join(", ", technologiesStack)
                        }
                    );
                }

                foreach (var fullName in companyEmployees.Founders.Split(','))
                {
                    if (string.IsNullOrEmpty(fullName.Trim()))
                        continue;

                    var founderProfile = companyEmployees.Employees.Where(x => x.FullName == fullName).FirstOrDefault();

                    var emails = !string.IsNullOrEmpty(fullName.Trim()) ? _emailGenerator.GetValidEmails(fullName.Trim().Split(' ')[0], fullName.Trim().Split(' ')[1], companyEmployees.Website)
                        : founderProfile != null ? _emailGenerator.GetValidEmails(founderProfile.FirstName, founderProfile.LastName, companyEmployees.Website) : new List<string>();

                    result.Add(
                        new ResultViewModel
                        {
                            FirstName = fullName.Trim().Split(' ')[0],
                            LastName = fullName.Trim().Split(' ')[1],
                            Job = "Founder",
                            PersonLinkedIn = founderProfile != null ? founderProfile.ProfileUrl : "...",
                            Company = companyEmployees.OrganizationName,
                            Website = companyEmployees.Website,
                            CompanyLogoUrl = companyEmployees.LogoCompanyUrl,
                            CrunchUrl = companyEmployees.OrganizationNameURL,
                            Email = emails.Count > 0 ? emails.FirstOrDefault() : "...",
                            EmailStatus = emails.Count > 0 && emails.Count < 4 ? "OK" : "...",
                            City = location.Count() > 0 ? location[0] : "...",
                            State = location.Count() > 1 ? location[1] : "...",
                            Country = location.Count() > 2 ? location[2] : "...",
                            PhoneNumber = companyEmployees.PhoneNumber,
                            AmountEmployees = companyEmployees.NumberofEmployees,
                            Industry = companyEmployees.Categories,
                            Twitter = companyEmployees.Twitter,
                            Facebook = companyEmployees.Facebook,
                            //CompanySpecialties = companyEmployees.Specialties,
                            TechStack = string.Join(", ", technologiesStack)
                        }
                   );
                }

                //After company scraped company data processing
                _suitableProfileService.AddSuitableProfile(result);
                _profilesService.UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses.Success, companyEmployees.Id);
            }
        }

        public void ImpotCompaniesCSVFile(string filePath = @"D:\CompaniesInfo\companies-23-08-2019.csv")
        {
            var companies = new List<CompanyImportViewModel>() { };
            var sourceCompanies = new CSVSource<CompanyImportViewModel>(filePath);

            while (sourceCompanies.HasMore)
            {
                companies.Add(sourceCompanies.ReadNext());
            }

            var companiesDB = MapperConfigurationModel.Instance.Map<IEnumerable<CompanyImportViewModel>, IEnumerable<Company>>(companies);
            companiesDB.Where(x => (x.LinkedInURL == null || x.LinkedInURL.Trim() == "") || (x.Website == null || x.Website.Trim() == "")).ToList().ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Failed);

            using (var db = new ScraperLinkedInDBEntities())
            {
                foreach (var item in companiesDB)
                {
                    db.Companies.Add(item);
                }
                db.SaveChanges();
            }
        }

        public void SaveToXLSXFile(List<ResultViewModel> profiles)
        {
            profiles.Insert(0, 
                new ResultViewModel
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Job = "Job",
                    PersonLinkedIn = "Person LinkedIn",
                    Company = "Company",
                    Website = "Website",
                    //CompanyLogoUrl = "Company Logo",
                    CrunchUrl = "Crunch Url",
                    Email = "Email",
                    EmailStatus = "Email Status",
                    City = "City",
                    State = "State",
                    Country = "Country",
                    PhoneNumber = "Phone number",
                    AmountEmployees = "Amount Employees",
                    Industry = "Industry",
                    Twitter = "Twitter",
                    Facebook = "Facebook",
                    //CompanySpecialties = "CompanySpecialties",
                    TechStack = "TechStack"
                }
            );

            using (var excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Profiles");

                string headerRange = "A1:" + Char.ConvertFromUtf32(18 + 64) + "1";
                var worksheet = excel.Workbook.Worksheets["Profiles"];
                worksheet.Cells[headerRange].LoadFromCollection(profiles);
                worksheet.Cells["A1:Q2000"].AutoFitColumns(10, 25);
                worksheet.Column(11).AutoFit();

                var excelFile = new FileInfo(@"D:\CompaniesInfo\test_1.xlsx");
                excel.SaveAs(excelFile);

                Console.WriteLine($"\n\nFile saved \nPath: { excelFile.FullName }");
            }
        }
    }
}