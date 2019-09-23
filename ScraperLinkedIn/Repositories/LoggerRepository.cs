using ScraperLinkedIn.Database;
using System;
using System.Threading.Tasks;

namespace ScraperLinkedIn.Repositories
{
    class LoggerRepository
    {
        public async Task AddAsync(string message)
        {
            await AddToDBAsync(message);
        }

        public async Task AddAsync(string remarks, string message)
        {
            await AddToDBAsync(message, remarks);
        }

        private async Task AddToDBAsync(string message, string remarks = "")
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var log = new DebugLog
                {
                    Remarks = remarks,
                    Logs = message,
                    CreatedOn = DateTime.Now
                };

                db.DebugLogs.Add(log);

                await db.SaveChangesAsync();
            }
        }
    }
}