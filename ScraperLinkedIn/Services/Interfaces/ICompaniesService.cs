using ScraperLinkedIn.Models;
using System.Collections.Generic;

namespace ScraperLinkedIn.Services.Interfaces
{
    interface ICompaniesService
    {
        IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size);

        IEnumerable<CompanyEmployeesViewModel> GetCompaniesForSearch();

        void UpdateCompany(CompanyEmployeesViewModel companyEmployees);
    }
}