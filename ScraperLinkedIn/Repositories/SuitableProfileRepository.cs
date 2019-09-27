using ScraperLinkedIn.Database;
using ScraperLinkedIn.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace ScraperLinkedIn.Repositories
{
    class SuitableProfileRepository : ISuitableProfileRepository
    {
        public void AddSuitableProfile(IEnumerable<SuitableProfile> suitableProfiles)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                foreach (var suitableProfile in suitableProfiles)
                {
                    suitableProfile.FirstName = string.IsNullOrEmpty(suitableProfile.FirstName) ? "..." : suitableProfile.FirstName;
                    suitableProfile.LastName = string.IsNullOrEmpty(suitableProfile.LastName) ? "..." : suitableProfile.LastName;
                    suitableProfile.Job = string.IsNullOrEmpty(suitableProfile.Job) ? "..." : suitableProfile.Job;
                    suitableProfile.PersonLinkedIn = string.IsNullOrEmpty(suitableProfile.PersonLinkedIn) ? "..." : suitableProfile.PersonLinkedIn;
                    suitableProfile.Company = string.IsNullOrEmpty(suitableProfile.Company) ? "..." : suitableProfile.Company;
                    suitableProfile.Website = string.IsNullOrEmpty(suitableProfile.Website) ? "..." : suitableProfile.Website;
                    suitableProfile.CompanyLogoUrl = string.IsNullOrEmpty(suitableProfile.CompanyLogoUrl) ? "..." : suitableProfile.CompanyLogoUrl;
                    suitableProfile.CrunchUrl = string.IsNullOrEmpty(suitableProfile.CrunchUrl) ? "..." : suitableProfile.CrunchUrl;
                    suitableProfile.Email = string.IsNullOrEmpty(suitableProfile.Email) ? "..." : suitableProfile.Email;
                    suitableProfile.EmailStatus = string.IsNullOrEmpty(suitableProfile.EmailStatus) ? "..." : suitableProfile.EmailStatus;
                    suitableProfile.City = string.IsNullOrEmpty(suitableProfile.City) ? "..." : suitableProfile.City;
                    suitableProfile.State = string.IsNullOrEmpty(suitableProfile.State) ? "..." : suitableProfile.State;
                    suitableProfile.Country = string.IsNullOrEmpty(suitableProfile.Country) ? "..." : suitableProfile.Country;
                    suitableProfile.PhoneNumber = string.IsNullOrEmpty(suitableProfile.PhoneNumber) ? "..." : suitableProfile.PhoneNumber;
                    suitableProfile.AmountEmployees = string.IsNullOrEmpty(suitableProfile.AmountEmployees) ? "..." : suitableProfile.AmountEmployees;
                    suitableProfile.Industry = string.IsNullOrEmpty(suitableProfile.Industry) ? "..." : suitableProfile.Industry;
                    suitableProfile.Twitter = string.IsNullOrEmpty(suitableProfile.Twitter) ? "..." : suitableProfile.Twitter;
                    suitableProfile.Facebook = string.IsNullOrEmpty(suitableProfile.Facebook) ? "..." : suitableProfile.Facebook;
                    suitableProfile.TechStack = string.IsNullOrEmpty(suitableProfile.TechStack) ? "..." : suitableProfile.TechStack;
                    suitableProfile.DateTimeCreation = DateTime.Now;

                    db.SuitableProfiles.Add(suitableProfile);
                }

                db.SaveChanges();
            }
        }
    }
}