using ScraperLinkedIn.Database;
using ScraperLinkedIn.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Profile = ScraperLinkedIn.Database.Profile;

namespace ScraperLinkedIn.Repositories
{
    class ProfilesRepository
    {
        public async Task<IEnumerable<Profile>> GetProfilesAsync(int profile_batch_size)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Profiles.Where(x => (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(profile_batch_size);

                (await result.Where(x => x.ExecutionStatusID == (int)ExecutionStatuses.Created).ToListAsync()).ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                await db.SaveChangesAsync();

                return result;
            }
        }

        public async Task<int> GetCountRawProfilesAsync()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var count = await db.Profiles.Where(x => (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).CountAsync();

                return count;
            }
        }

        public async Task<int> GetCountNewProfilesAsync()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = await db.Profiles.Where(x => (DbFunctions.TruncateTime(x.DateСreation) == DbFunctions.TruncateTime(DateTime.Now)) && (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created)).CountAsync();

                return result;
            }
        }

        public async Task AddProfilesAsync(IEnumerable<Profile> profiles)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var companyID = profiles.FirstOrDefault().CompanyID;
                var addProfiles = db.Profiles.Where(x => x.CompanyID == companyID).Select(x => x.ProfileUrl);

                foreach (var profile in profiles.Where(x => !addProfiles.Contains(x.ProfileUrl)))
                {
                    profile.DateСreation = DateTime.Now;
                    db.Profiles.Add(profile);
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateProfileAsync(Profile profile)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var profileDB = await db.Profiles.Where(x => x.Id == profile.Id).FirstOrDefaultAsync();

                profileDB.FirstName = profile.FirstName ?? "";
                profileDB.LastName = profile.LastName ?? "";
                profileDB.FullName = profile.FullName ?? "";
                profileDB.Job = profile.Job ?? "";
                profileDB.AllSkills = profile.AllSkills ?? "";
                profileDB.ExecutionStatusID = profile.ExecutionStatusID;
                profileDB.ProfileStatusID = profile.ProfileStatusID;
                profileDB.DateСreation = profile.DateСreation;

                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateProfilesExecutionStatusByCompanyIDAsync(ExecutionStatuses executionStatus, int companyID)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                (await db.Profiles.Where(x => x.CompanyID == companyID).ToListAsync()).ForEach(y => y.ExecutionStatusID = (int)executionStatus);
                await db.SaveChangesAsync();
            }
        }
    }
}