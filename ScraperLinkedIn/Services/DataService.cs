using OfficeOpenXml;
using ScraperLinkedIn.Email;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.SearchParameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScraperLinkedIn.Services
{
    class DataService
    {
        private EmailGenerator _emailGenerator;

        public DataService()
        {
            _emailGenerator = new EmailGenerator();
        }

        public IEnumerable<ResultViewModel> SearchSuitableDirectorsCompanies(IEnumerable<CompanyEmployeesViewModel> companiesEmployees)
        {
            var result = new List<ResultViewModel>();

            foreach (var companyEmployees in companiesEmployees)
            {
                var technologiesStack = new List<string>();

                var profileSkills = companyEmployees.Employees.Where(x => TechnologiesSearchParameters.Technologies.Any(y => x.AllSkills != null ? x.AllSkills.ToUpper().Contains(y) : false)).Select(z => z.Skills);

                foreach (var skills in profileSkills)
                {
                    foreach (var skill in skills)
                    {
                        if (!technologiesStack.Contains(skill))
                        {
                            technologiesStack.Add(skill);
                        }
                    }
                }

                if (profileSkills == null)
                {
                    continue;
                }

                foreach (var employee in companyEmployees.Employees)
                {
                    if (RolesSearchParameters.Roles.Any(y => employee.Job.ToUpper().Contains(y)))
                    {
                        var emails = !string.IsNullOrEmpty(employee.FirstName) && !string.IsNullOrEmpty(employee.LastName) ? _emailGenerator.GetValidEmails(employee.FirstName, employee.LastName, companyEmployees.Website) : new List<string>();

                        result.Add(
                            new ResultViewModel
                            {
                                Name = employee.FullName,
                                Job = RolesSearchParameters.Roles.Where(x => employee.Job.ToUpper().Contains(x)).FirstOrDefault(),
                                PersonLinkedIn = employee.ProfileUrl,
                                Company = companyEmployees.OrganizationName,
                                Website = companyEmployees.Website,
                                CompanyLogoUrl = companyEmployees.LogoCompanyUrl,
                                CrunchUrl = companyEmployees.OrganizationNameURL,
                                Email = emails.FirstOrDefault(),
                                EmailStatus = emails.Count < 4 ? "OK" : "",
                                CompanySpecialties = companyEmployees.Specialties,
                                TechStack = string.Join(", ", profileSkills)
                            }
                        );
                    }
                }

                foreach (var fullName in companyEmployees.Founders.Split(','))
                {
                    var founderProfile = companyEmployees.Employees.Where(x => x.FullName == fullName).FirstOrDefault();

                    var emails = !string.IsNullOrEmpty(fullName.Trim()) ? _emailGenerator.GetValidEmails(fullName.Trim().Split(' ')[0], fullName.Trim().Split(' ')[1], companyEmployees.Website)
                        : _emailGenerator.GetValidEmails(founderProfile.FirstName, founderProfile.LastName, companyEmployees.Website);

                    result.Add(
                        new ResultViewModel
                        {
                            Name = fullName.Trim(),
                            Job = "Founder",
                            PersonLinkedIn = founderProfile.ProfileUrl,
                            Company = companyEmployees.OrganizationName,
                            Website = companyEmployees.Website,
                            CompanyLogoUrl = companyEmployees.LogoCompanyUrl,
                            CrunchUrl = companyEmployees.OrganizationNameURL,
                            Email = emails.FirstOrDefault(),
                            EmailStatus = emails.Count < 4 ? "OK" : "",
                            CompanySpecialties = companyEmployees.Specialties,
                            TechStack = string.Join(", ", profileSkills)
                        }
                   );
                }
            }

            return result;
        }

        public void SaveToXLSXFile(IEnumerable<ResultViewModel> profiles)
        {
            using (var excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Profiles");

                string headerRange = "A1:" + Char.ConvertFromUtf32(11 + 64) + "1";
                var worksheet = excel.Workbook.Worksheets["Profiles"];
                worksheet.Cells[headerRange].LoadFromCollection(profiles);
                worksheet.Cells["A1:K20"].AutoFitColumns(10, 25);
                worksheet.Column(11).AutoFit();

                var excelFile = new FileInfo(@"D:\CompaniesInfo\test_1.xlsx");
                excel.SaveAs(excelFile);

                Console.WriteLine($"\n\nFile saved \nPath: { excelFile.FullName }");
            }
        }
    }
}