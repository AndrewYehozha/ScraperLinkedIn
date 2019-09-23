using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using System.Collections.Generic;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;

namespace ScraperLinkedIn.Services
{
    class SuitableProfileService
    {
        private SuitableProfileRepository _suitableProfileRepository;

        public SuitableProfileService()
        {
            _suitableProfileRepository = new SuitableProfileRepository();
        }

        public async void AddSuitableProfile(IEnumerable<ResultViewModel> results)
        {
            await _suitableProfileRepository.AddSuitableProfileAsync(MapperConfigurationModel.Instance.Map<IEnumerable<ResultViewModel>, IEnumerable<SuitableProfile>> (results));
        }
    }
}