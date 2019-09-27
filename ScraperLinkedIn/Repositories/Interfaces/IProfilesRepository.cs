using ScraperLinkedIn.Database;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Repositories.Interfaces
{
    interface IProfilesRepository
    {
        IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size);

        int GetCountRawProfiles();

        int GetCountNewProfiles();

        void AddProfiles(IEnumerable<Profile> profiles);

        void UpdateProfile(Profile profile);

        void UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses executionStatus, int companyID);
    }
}