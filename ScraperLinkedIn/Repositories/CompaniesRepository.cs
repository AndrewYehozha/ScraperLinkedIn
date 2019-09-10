using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System.Collections.Generic;
using System.Linq;
using Youworks.Text;

namespace ScraperLinkedIn.Repositories
{
    class CompaniesRepository
    {
        public IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size)
        {
            var result = new List<Company>();

            using (var db = new ScraperLinkedInDBEntities())
            {
                result = db.Companies.Where(x => !string.IsNullOrEmpty(x.Website) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(company_batch_size).ToList();

                result.ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                db.SaveChanges();
            }

            return MapperConfigurationModel.Instance.Map<IEnumerable<Company>, IEnumerable<CompanyEmployeesViewModel>>(result);
        }

        public void UpdateCompany(Company company)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var companyDB = db.Companies.Where(x => x.Id == company.Id).FirstOrDefault();

                companyDB.LogoUrl = company.LogoUrl;
                companyDB.Specialties = company.Specialties;
                companyDB.ExecutionStatusID = company.ExecutionStatusID;

                db.SaveChanges();
            }
        }

        public void UpdateCompaniesWithStatusFailed(IEnumerable<Company> companies)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var failedCompaniesIDs = companies.Where(y => y.ExecutionStatusID == (int)ExecutionStatuses.Failed).Select(y => y.Id);

                var companiesDB = db.Companies.Where(x => failedCompaniesIDs.Contains(x.Id)).ToList();
                companiesDB.ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Failed);

                db.SaveChanges();
            }
        }

        public void ImpotCompaniesCSVFile(string filePath = @"D:\CompaniesInfo\companies-23-08-2019.csv")
        {
            var companies = new List<CompanyImportViewModel>() { };
            var sourceCompanies = new CSVSource<CompanyImportViewModel>(filePath);

            while (sourceCompanies.HasMore)
            {
                companies.Add(sourceCompanies.ReadNext());
            }

            var companiesDB = MapperConfigurationModel.Instance.Map<IEnumerable<CompanyImportViewModel>, IEnumerable<Company>>(companies);

            using (var db = new ScraperLinkedInDBEntities())
            {
                foreach (var item in companiesDB)
                {
                    db.Companies.Add(item);
                }
                db.SaveChanges();
            }
        }
    }
}