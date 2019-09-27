using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Repositories.Interfaces;
using ScraperLinkedIn.Services.Interfaces;
using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.Services
{
    class AccountsService : IAccountsService
    {
        private IAccountsRepository _accountsRepository;

        public AccountsService()
        {
            _accountsRepository = new AccountsRepository();
        }

        public SettingsViewModel GetAccountSettings()
        {
            var settings = _accountsRepository.GetAccountSettings();

            return MapperConfigurationModel.Instance.Map<Setting, SettingsViewModel>(settings);
        }

        public void UpdateScraperStatus(ScraperStatuses scraperStatus)
        {
            _accountsRepository.UpdateScraperStatus((int)scraperStatus);
        }
    }
}