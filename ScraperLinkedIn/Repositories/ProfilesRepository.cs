using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScraperLinkedIn.Repositories
{
    class ProfilesRepository
    {
        public IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size)
        {
            var result = new List<Profile>();

            using (var db = new ScraperLinkedInDBEntities())
            {
                result = db.Profiles.Where(x => (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(profile_batch_size).ToList();

                result.ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                db.SaveChanges();
            }

            return MapperConfigurationModel.Instance.Map<IEnumerable<Profile>, IEnumerable<ProfileViewModel>>(result);
        }

        public void AddProfiles(IEnumerable<Profile> profiles)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var companyID = profiles.FirstOrDefault().CompanyID;
                var addProfiles = db.Profiles.Where(x => x.CompanyID == companyID).Select(x => x.ProfileUrl);

                foreach (var profile in profiles.Where(x => !addProfiles.Contains(x.ProfileUrl)))
                {
                    db.Profiles.Add(profile);
                }

                db.SaveChanges();
            }
        }

        public void UpdateProfile(Profile profile)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var profileDB = db.Profiles.Where(x => x.Id == profile.Id).FirstOrDefault();

                profileDB.FirstName = profile.FirstName;
                profileDB.LastName = profile.LastName;
                profileDB.FullName = profile.FullName;
                profileDB.Job = profile.Job;
                profileDB.AllSkills = profile.AllSkills;
                profileDB.ExecutionStatusID = profile.ExecutionStatusID;

                db.SaveChanges();
            }
        }

        public void UpdateProfilesWithStatusFailed(IEnumerable<Profile> profiles)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var failedProfilesIDs = profiles.Where(y => y.ExecutionStatusID == (int)ExecutionStatuses.Failed).Select(y => y.Id);

                var profilesDB = db.Profiles.Where(x => failedProfilesIDs.Contains(x.Id)).ToList();
                profilesDB.ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Failed);

                db.SaveChanges();
            }
        }
    }
}