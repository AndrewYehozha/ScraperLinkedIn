using ScraperLinkedIn.Models;
using System.Collections.Generic;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Database;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<CompanyEmployeesViewModel>> GetCompaniesAsync(int company_batch_size)
        {
            var companies = await _companyRepository.GetCompaniesAsync(company_batch_size);

            return MapperConfigurationModel.Instance.Map<IEnumerable<Company>, IEnumerable<CompanyEmployeesViewModel>>(companies);
        }

        public async Task<IEnumerable<CompanyEmployeesViewModel>> GetCompaniesForSearchAsync()
        {
            var companies = await _companyRepository.GetCompaniesForSearchAsync();

            return MapperConfigurationModel.Instance.Map<IEnumerable<Company>, IEnumerable<CompanyEmployeesViewModel>>(companies);
        }

        public async void UpdateCompany(CompanyEmployeesViewModel companyEmployees)
        {
            await _companyRepository.UpdateCompanyAsync(MapperConfigurationModel.Instance.Map<CompanyEmployeesViewModel, Company>(companyEmployees));

            _profilesService.AddProfiles(companyEmployees.Employees ?? new List<ProfileViewModel>());
        }
    }
}