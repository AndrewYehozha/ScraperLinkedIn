using ScraperLinkedIn.Models;
using System.Collections.Generic;
using System.Linq;
using Youworks.Text;

namespace ScraperLinkedIn.Repositories
{
    class CompanyRepository
    {
        public IEnumerable<CompaniesEmployeesViewModel> GetCompany()
        {
            var companies = new List<CompaniesEmployeesViewModel>() { };
            var sourceCompanies = new CSVSource<CompaniesEmployeesViewModel>(@"D:\CompaniesInfo\companies-23-08-2019.csv");

            while (sourceCompanies.HasMore)
            {
                companies.Add(sourceCompanies.ReadNext());
            }

            return companies.Where(x => !string.IsNullOrEmpty(x.Website));
        }
    }
}