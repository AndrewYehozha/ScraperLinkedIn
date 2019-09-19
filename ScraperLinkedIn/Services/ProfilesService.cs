using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Types;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size)
        {
            return _profilesRepository.GetProfiles(profile_batch_size);
        }

        public int GetCountRawProfiles()
        {
            return _profilesRepository.GetCountRawProfiles();
        }

        public int GetCountNewProfiles()
        {
            return _profilesRepository.GetCountNewProfiles();
        }

        public void AddProfiles(IEnumerable<ProfileViewModel> profiles)
        {
            if (profiles.Count() > 0)
            {
                _profilesRepository.AddProfiles(MapperConfigurationModel.Instance.Map<IEnumerable<ProfileViewModel>, IEnumerable<Profile>>(profiles));
            }
        }

        public void UpdateProfile(ProfileViewModel profile, IEnumerable<string> rolesSearch, IEnumerable<string> technologiesSearch)
        {
            profile.ProfileStatus = GetProfileStatus(profile, rolesSearch, technologiesSearch);

            _profilesRepository.UpdateProfile(MapperConfigurationModel.Instance.Map<ProfileViewModel, Profile>(profile));
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

        public void UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses executionStatus, int companyID)
        {
            _profilesRepository.UpdateProfilesExecutionStatusByCompanyID(executionStatus, companyID);
        }

        public void UpdateProfilesWithStatusFailed(IEnumerable<ProfileViewModel> profiles)
        {
            _profilesRepository.UpdateProfilesWithStatusFailed(MapperConfigurationModel.Instance.Map<IEnumerable<ProfileViewModel>, IEnumerable<Profile>>(profiles));
        }
    }
}