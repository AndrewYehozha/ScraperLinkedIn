using ScraperLinkedIn.Models;
using System.Collections.Generic;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Database;

namespace ScraperLinkedIn.Services
{
    class CompaniesService
    {
        private ProfilesService _profilesService;
        private CompaniesRepository _companyRepository;

        public CompaniesService()
        {
            _profilesService = new ProfilesService();
            _companyRepository = new CompaniesRepository();
        }

        public IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size)
        {
            return _companyRepository.GetCompanies(company_batch_size);
        }

        public void UpdateCompany(CompanyEmployeesViewModel companyEmployees)
        {
            _companyRepository.UpdateCompany(MapperConfigurationModel.Instance.Map<CompanyEmployeesViewModel, Company>(companyEmployees));

            _profilesService.AddProfiles(companyEmployees.Employees ?? new List<ProfileViewModel>());
        }

        public void UpdateCompaniesWithStatusFailed(IEnumerable<CompanyEmployeesViewModel> companies)
        {
            _companyRepository.UpdateCompaniesWithStatusFailed(MapperConfigurationModel.Instance.Map<IEnumerable<CompanyEmployeesViewModel>, IEnumerable<Company>>(companies));
        }
    }
}