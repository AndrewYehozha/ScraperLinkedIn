using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.Services.Interfaces
{
    interface IAccountsService
    {
        SettingsViewModel GetAccountSettings();

        void UpdateScraperStatus(ScraperStatuses scraperStatus);
    }
}