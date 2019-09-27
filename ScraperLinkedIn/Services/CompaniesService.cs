using ScraperLinkedIn.Models;
using System.Collections.Generic;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Services.Interfaces;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Database;
using ScraperLinkedIn.Repositories.Interfaces;

namespace ScraperLinkedIn.Services
{
    class CompaniesService : ICompaniesService
    {
        private IProfilesService _profilesService;
        private ICompaniesRepository _companyRepository;

        public CompaniesService()
        {
            _profilesService = new ProfilesService();
            _companyRepository = new CompaniesRepository();
        }

        public IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size)
        {
            return _companyRepository.GetCompanies(company_batch_size);
        }

        public IEnumerable<CompanyEmployeesViewModel> GetCompaniesForSearch()
        {
            return _companyRepository.GetCompaniesForSearch();
        }

        public void UpdateCompany(CompanyEmployeesViewModel companyEmployees)
        {
            _companyRepository.UpdateCompany(MapperConfigurationModel.Instance.Map<CompanyEmployeesViewModel, Company>(companyEmployees));

            _profilesService.AddProfiles(companyEmployees.Employees ?? new List<ProfileViewModel>());
        }
    }
}