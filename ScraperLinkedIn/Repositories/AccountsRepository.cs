using ScraperLinkedIn.Database;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ScraperLinkedIn.Repositories
{
    class AccountsRepository
    {
        public async Task<Setting> GetAccountSettingsAsync()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                return await db.Settings.FirstOrDefaultAsync();
            }
        }
    }
}