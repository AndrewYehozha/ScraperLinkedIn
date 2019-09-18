using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;

namespace ScraperLinkedIn.Services
{
    class AccountsService
    {
        private AccountsRepository _accountsRepository;

        public AccountsService()
        {
            _accountsRepository = new AccountsRepository();
        }

        public SettingsViewModel GetAccountSettings()
        {
            return _accountsRepository.GetAccountSettings();
        }

        public void UpdateAccountSettings(SettingsViewModel settingsViewModel)
        {
            _accountsRepository.UpdateAccountSettings(MapperConfigurationModel.Instance.Map<SettingsViewModel, Setting>(settingsViewModel));
        }
    }
}