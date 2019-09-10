using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ScraperLinkedIn.Services
{
    class ProfilesService
    {
        private ProfilesRepository _profilesRepository;

        public ProfilesService()
        {
            _profilesRepository = new ProfilesRepository();
        }

        public IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size)
        {
            return _profilesRepository.GetProfiles(profile_batch_size);
        }

        public void AddProfiles(IEnumerable<ProfileViewModel> profiles)
        {
            if (profiles.Count() > 0)
            {
                _profilesRepository.AddProfiles(MapperConfigurationModel.Instance.Map<IEnumerable<ProfileViewModel>, IEnumerable<Profile>>(profiles));
            }
        }

        public void UpdateProfile(ProfileViewModel profile)
        {
            _profilesRepository.UpdateProfile(MapperConfigurationModel.Instance.Map<ProfileViewModel, Profile>(profile));
        }

        public void UpdateProfilesWithStatusFailed(IEnumerable<ProfileViewModel> profiles)
        {
            _profilesRepository.UpdateProfilesWithStatusFailed(MapperConfigurationModel.Instance.Map<IEnumerable<ProfileViewModel>, IEnumerable<Profile>>(profiles));
        }
    }
}