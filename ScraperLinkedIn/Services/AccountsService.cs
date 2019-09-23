using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using System.Threading.Tasks;

namespace ScraperLinkedIn.Services
{
    class AccountsService
    {
        private AccountsRepository _accountsRepository;

        public AccountsService()
        {
            _accountsRepository = new AccountsRepository();
        }

        public async Task<SettingsViewModel> GetAccountSettingsAsync()
        {
            var settings = await _accountsRepository.GetAccountSettingsAsync();

            return MapperConfigurationModel.Instance.Map<Setting, SettingsViewModel>(settings);
        }
    }
}