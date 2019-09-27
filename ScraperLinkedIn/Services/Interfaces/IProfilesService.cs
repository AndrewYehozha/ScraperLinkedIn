using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Services.Interfaces
{
    interface IProfilesService
    {
        IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size);

        int GetCountRawProfiles();

        int GetCountNewProfiles();

        void AddProfiles(IEnumerable<ProfileViewModel> profiles);

        void UpdateProfile(ProfileViewModel profile, IEnumerable<string> rolesSearch, IEnumerable<string> technologiesSearch);

        void UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses executionStatus, int companyID);
    }
}