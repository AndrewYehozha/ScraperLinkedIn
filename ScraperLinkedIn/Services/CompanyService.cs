using ScraperLinkedIn.Models;
using System.Collections.Generic;
using ScraperLinkedIn.Repositories;

namespace ScraperLinkedIn.Services
{
    class CompanyService
    {
        CompanyRepository _companyRepository;

        public CompanyService()
        {
            _companyRepository = new CompanyRepository();
        }

        public IEnumerable<CompaniesEmployeesViewModel> GetCompany()
        {
            return _companyRepository.GetCompany();
        }
    }
}