using ScraperLinkedIn.Database;
using ScraperLinkedIn.Types;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperLinkedIn.Repositories
{
    class CompaniesRepository
    {
        public async Task<IEnumerable<Company>> GetCompaniesAsync(int company_batch_size)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = await db.Companies.Where(x => !string.IsNullOrEmpty(x.LinkedInURL.Trim()) && !string.IsNullOrEmpty(x.Website.Trim()) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(company_batch_size).ToListAsync();

                result.ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                await db.SaveChangesAsync();

                return result;
            }
        }

        public async Task<IEnumerable<Company>> GetCompaniesForSearchAsync()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var lastProcessedCompanyId = await db.Profiles.Where(x => (x.ExecutionStatusID == (int)ExecutionStatuses.Queued) && (x.ProfileStatusID != (int)ProfileStatuses.Undefined)).OrderByDescending(d => d.Id).Select(x => x.CompanyID).FirstOrDefaultAsync();

                var unsuitableCompanies = await db.Companies.Where(x => (x.Id < lastProcessedCompanyId) && (x.ExecutionStatusID == (int)ExecutionStatuses.Success) && x.Profiles.Any(y => (y.ExecutionStatusID != (int)ExecutionStatuses.Success)) && !x.Profiles.Any(y => (y.ProfileStatusID == (int)ProfileStatuses.Developer))).ToListAsync();
                unsuitableCompanies.ForEach(x => x.Profiles.ToList().ForEach(y => y.ExecutionStatusID = (int)ExecutionStatuses.Success));
                await db.SaveChangesAsync();

                return await db.Companies.Where(x => (x.Id < lastProcessedCompanyId) && (x.ExecutionStatusID == (int)ExecutionStatuses.Success) && x.Profiles.Any(y => (y.ProfileStatusID == (int)ProfileStatuses.Developer) && (y.ExecutionStatusID != (int)ExecutionStatuses.Success))).ToListAsync();
            }
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var companyDB = await db.Companies.Where(x => x.Id == company.Id).FirstOrDefaultAsync();

                companyDB.LogoUrl = company.LogoUrl ?? "";
                companyDB.Specialties = company.Specialties ?? "";
                companyDB.ExecutionStatusID = company.ExecutionStatusID;

                await db.SaveChangesAsync();
            }
        }
    }
}