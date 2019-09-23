using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profile = ScraperLinkedIn.Database.Profile;

namespace ScraperLinkedIn.Services
{
    class ProfilesService
    {
        private ProfilesRepository _profilesRepository;

        public ProfilesService()
        {
            _profilesRepository = new ProfilesRepository();
        }

        public async Task<IEnumerable<ProfileViewModel>> GetProfilesAsync(int profile_batch_size)
        {
            var profiles = await _profilesRepository.GetProfilesAsync(profile_batch_size);

            return MapperConfigurationModel.Instance.Map<IEnumerable<Profile>, IEnumerable<ProfileViewModel>>(profiles);
        }

        public async Task<int> GetCountRawProfilesAsync()
        {
            return await _profilesRepository.GetCountRawProfilesAsync();
        }

        public async Task<int> GetCountNewProfilesAsync()
        {
            return await _profilesRepository.GetCountNewProfilesAsync();
        }

        public async void AddProfiles(IEnumerable<ProfileViewModel> profiles)
        {
            if (profiles.Count() > 0)
            {
                await _profilesRepository.AddProfilesAsync(MapperConfigurationModel.Instance.Map<IEnumerable<ProfileViewModel>, IEnumerable<Profile>>(profiles));
            }
        }

        public async void UpdateProfile(ProfileViewModel profile, IEnumerable<string> rolesSearch, IEnumerable<string> technologiesSearch)
        {
            profile.ProfileStatus = GetProfileStatus(profile, rolesSearch, technologiesSearch);

            await _profilesRepository.UpdateProfileAsync(MapperConfigurationModel.Instance.Map<ProfileViewModel, Profile>(profile));
        }

        private ProfileStatuses GetProfileStatus(ProfileViewModel profile, IEnumerable<string> rolesSearch, IEnumerable<string> technologiesSearch)
        {
            if (rolesSearch.Any(x => profile.Job.ToUpper().Split(' ').Contains(x.Trim())))
            {
                return ProfileStatuses.Chief;
            }

            if (technologiesSearch.Any(y => profile.AllSkills.ToUpper().Contains(y.Trim())))
            {
                return ProfileStatuses.Developer;
            }

            return ProfileStatuses.Unsuited;
        }

        public async void UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses executionStatus, int companyID)
        {
            await _profilesRepository.UpdateProfilesExecutionStatusByCompanyIDAsync(executionStatus, companyID);
        }
    }
}