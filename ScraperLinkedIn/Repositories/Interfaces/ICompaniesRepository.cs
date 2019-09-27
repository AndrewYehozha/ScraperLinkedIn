using ScraperLinkedIn.Database;
using ScraperLinkedIn.Models;
using System.Collections.Generic;

namespace ScraperLinkedIn.Repositories.Interfaces
{
    interface ICompaniesRepository
    {
        IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size);

        IEnumerable<CompanyEmployeesViewModel> GetCompaniesForSearch();

        void UpdateCompany(Company company);
    }
}