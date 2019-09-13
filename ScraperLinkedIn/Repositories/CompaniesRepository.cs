﻿using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System.Collections.Generic;
using System.Linq;

namespace ScraperLinkedIn.Repositories
{
    class CompaniesRepository
    {
        public IEnumerable<CompanyEmployeesViewModel> GetCompanies(int company_batch_size)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Companies.Where(x => !string.IsNullOrEmpty(x.LinkedInURL.Trim()) && !string.IsNullOrEmpty(x.Website.Trim()) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(company_batch_size);

                result.ToList().ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                db.SaveChanges();

                return MapperConfigurationModel.Instance.Map<IEnumerable<Company>, IEnumerable<CompanyEmployeesViewModel>>(result);
            }
        }

        public IEnumerable<CompanyEmployeesViewModel> GetCompaniesForSearch()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var lastProcessedCompanyId = db.Profiles.Where(x => (x.ExecutionStatusID == (int)ExecutionStatuses.Queued) && (x.ProfileStatusID != (int)ProfileStatuses.Undefined)).OrderByDescending(d => d.Id).Select(x => x.CompanyID).FirstOrDefault();

                var unsuitableCompanies = db.Companies.Where(x => (x.Id < lastProcessedCompanyId) && (x.ExecutionStatusID == (int)ExecutionStatuses.Success) && x.Profiles.Any(y => (y.ExecutionStatusID != (int)ExecutionStatuses.Success)) && !x.Profiles.Any(y => (y.ProfileStatusID == (int)ProfileStatuses.Developer))).ToList();
                unsuitableCompanies.ToList().ForEach(x => x.Profiles.ToList().ForEach(y => y.ExecutionStatusID = (int)ExecutionStatuses.Success));
                db.SaveChanges();

                var result = db.Companies.Where(x => (x.Id < lastProcessedCompanyId) && (x.ExecutionStatusID == (int)ExecutionStatuses.Success) && x.Profiles.Any(y => (y.ProfileStatusID == (int)ProfileStatuses.Developer) && (y.ExecutionStatusID != (int)ExecutionStatuses.Success))).ToList();

                return MapperConfigurationModel.Instance.Map<IEnumerable<Company>, IEnumerable<CompanyEmployeesViewModel>>(result);
            }
        }

        public void UpdateCompany(Company company)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var companyDB = db.Companies.Where(x => x.Id == company.Id).FirstOrDefault();

                companyDB.LogoUrl = company.LogoUrl ?? "";
                companyDB.Specialties = company.Specialties ?? "";
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
    }
}