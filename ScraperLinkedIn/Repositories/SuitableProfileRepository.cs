using ScraperLinkedIn.Database;
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
                    db.SuitableProfiles.Add(suitableProfile);
                }

                db.SaveChanges();
            }
        }
    }
}