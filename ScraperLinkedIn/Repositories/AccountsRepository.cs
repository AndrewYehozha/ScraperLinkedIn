using ScraperLinkedIn.Database;
using System.Linq;

namespace ScraperLinkedIn.Repositories
{
    class AccountsRepository
    {
        public Setting GetAccountSettings()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                return db.Settings.FirstOrDefault();
            }
        }

        public void UpdateScraperStatus(int scraperStatusID)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var settings = db.Settings.FirstOrDefault();
                settings.ScraperStatusID = scraperStatusID;

                db.SaveChanges();
            }
        }
    }
}