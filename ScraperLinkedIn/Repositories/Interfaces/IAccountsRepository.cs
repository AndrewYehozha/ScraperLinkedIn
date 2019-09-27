using ScraperLinkedIn.Database;

namespace ScraperLinkedIn.Repositories.Interfaces
{
    interface IAccountsRepository
    {
        Setting GetAccountSettings();

        void UpdateScraperStatus(int scraperStatusID);
    }
}