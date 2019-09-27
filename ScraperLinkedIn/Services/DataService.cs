using ScraperLinkedIn.Email;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System;
using System.Collections.Generic;
using ScraperLinkedIn.Services.Interfaces;
using System.Linq;

namespace ScraperLinkedIn.Services
{
    class DataService : IDataService
    {
        private EmailGenerator _emailGenerator;
        private IProfilesService _profilesService;
        private ISuitableProfileService _suitableProfileService;
        private IAccountsService _accountsService;

        public DataService()
        {
            _emailGenerator = new EmailGenerator();
            _profilesService = new ProfilesService();
            _suitableProfileService = new SuitableProfileService();
            _accountsService = new AccountsService();
        }

        public void SearchSuitableDirectorsCompanies(IEnumerable<CompanyEmployeesViewModel> companiesEmployees)
        {
            var settings = _accountsService.GetAccountSettings();
            var rolesSearch = settings != null ? settings.RolesSearch.Split(',') : new string[] { };

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
                            Job = rolesSearch.Where(x => employee.Job.ToUpper().Split(' ').Contains(x.Trim())).FirstOrDefault(),
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

                    var founderProfile = companyEmployees.Employees.Where(x => x.FullName == fullName.Trim() && x.ProfileStatus == ProfileStatuses.Chief).FirstOrDefault();

                    if (founderProfile == null)
                    {
                        var emails = !string.IsNullOrEmpty(fullName.Trim()) ? _emailGenerator.GetValidEmails(fullName.Trim().Split(' ')[0], fullName.Trim().Split(' ')[1], companyEmployees.Website) : new List<string>();

                        result.Add(
                            new ResultViewModel
                            {
                                FirstName = fullName.Trim().Split(' ')[0],
                                LastName = fullName.Trim().Split(' ')[1],
                                Job = "Founder",
                                PersonLinkedIn = "...",
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
                }

                //After company scraped company data processing
                _suitableProfileService.AddSuitableProfile(result);
                _profilesService.UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses.Success, companyEmployees.Id);
            }
        }
    }
}