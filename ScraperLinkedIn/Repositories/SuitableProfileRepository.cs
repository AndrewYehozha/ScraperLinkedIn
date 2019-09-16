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
                    suitableProfile.DateTimeCreation = DateTime.Now;

                    db.SuitableProfiles.Add(suitableProfile);
                }

                db.SaveChanges();
            }
        }
    }
}