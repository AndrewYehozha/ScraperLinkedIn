using ScraperLinkedIn.Models;
using System.Collections.Generic;

namespace ScraperLinkedIn.Services.Interfaces
{
    interface IDataService
    {
        void SearchSuitableDirectorsCompanies(IEnumerable<CompanyEmployeesViewModel> companiesEmployees);
    }
}