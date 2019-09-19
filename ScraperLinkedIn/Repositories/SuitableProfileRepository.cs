using ScraperLinkedIn.Database;
using System;
using System.Collections.Generic;

namespace ScraperLinkedIn.Repositories
{
    class SuitableProfileRepository
    {
        public void AddSuitableProfile(IEnumerable<SuitableProfile> suitableProfiles)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                foreach (var suitableProfile in suitableProfiles)
                {
                    suitableProfile.FirstName = suitableProfile.FirstName ?? "...";
                    suitableProfile.LastName = suitableProfile.LastName ?? "...";
                    suitableProfile.Job = suitableProfile.Job ?? "...";
                    suitableProfile.PersonLinkedIn = suitableProfile.PersonLinkedIn ?? "...";
                    suitableProfile.Company = suitableProfile.Company ?? "...";
                    suitableProfile.Website = suitableProfile.Website ?? "...";
                    suitableProfile.CompanyLogoUrl = suitableProfile.CompanyLogoUrl ?? "...";
                    suitableProfile.CrunchUrl = suitableProfile.CrunchUrl ?? "...";
                    suitableProfile.Email = suitableProfile.Email ?? "...";
                    suitableProfile.EmailStatus = suitableProfile.EmailStatus ?? "...";
                    suitableProfile.City = suitableProfile.City ?? "...";
                    suitableProfile.State = suitableProfile.State ?? "...";
                    suitableProfile.Country = suitableProfile.Country ?? "...";
                    suitableProfile.PhoneNumber = suitableProfile.PhoneNumber ?? "...";
                    suitableProfile.AmountEmployees = suitableProfile.AmountEmployees ?? "...";
                    suitableProfile.Industry = suitableProfile.Industry ?? "...";
                    suitableProfile.Twitter = suitableProfile.Twitter ?? "...";
                    suitableProfile.Facebook = suitableProfile.Facebook ?? "...";
                    suitableProfile.TechStack = suitableProfile.TechStack ?? "...";
                    suitableProfile.DateTimeCreation = DateTime.Now;

                    db.SuitableProfiles.Add(suitableProfile);
                }

                db.SaveChanges();
            }
        }
    }
}