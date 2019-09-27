using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using System.Collections.Generic;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Services.Interfaces;
using ScraperLinkedIn.Repositories.Interfaces;

namespace ScraperLinkedIn.Services
{
    class SuitableProfileService : ISuitableProfileService
    {
        private ISuitableProfileRepository _suitableProfileRepository;

        public SuitableProfileService()
        {
            _suitableProfileRepository = new SuitableProfileRepository();
        }

        public void AddSuitableProfile(IEnumerable<ResultViewModel> results)
        {
            _suitableProfileRepository.AddSuitableProfile(MapperConfigurationModel.Instance.Map<IEnumerable<ResultViewModel>, IEnumerable<SuitableProfile>> (results));
        }
    }
}