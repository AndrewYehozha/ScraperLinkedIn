using ScraperLinkedIn.Database;
using System;

namespace ScraperLinkedIn.Repositories
{
    class LoggerRepository
    {
        public void Add(string message)
        {
            AddToDB(message);
        }

        public void Add(string remarks, string message)
        {
            AddToDB(message, remarks);
        }

        private void AddToDB(string message, string remarks = "")
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
                db.SaveChanges();
            }
        }
    }
}