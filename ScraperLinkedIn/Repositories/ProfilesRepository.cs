using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using ScraperLinkedIn.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Profile = ScraperLinkedIn.Database.Profile;

namespace ScraperLinkedIn.Repositories
{
    class ProfilesRepository
    {
        public IEnumerable<ProfileViewModel> GetProfiles(int profile_batch_size)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Profiles.Where(x => (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Take(profile_batch_size);

                result.Where(x => x.ExecutionStatusID == (int)ExecutionStatuses.Created).ToList().ForEach(x => x.ExecutionStatusID = (int)ExecutionStatuses.Queued);
                db.SaveChanges();

                return MapperConfigurationModel.Instance.Map<IEnumerable<Profile>, IEnumerable<ProfileViewModel>>(result);
            }
        }

        public int GetCountRawProfiles()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var count = db.Profiles.Where(x => (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created || x.ExecutionStatusID == (int)ExecutionStatuses.Queued)).Count();

                return count;
            }
        }

        public int GetCountNewProfiles()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Profiles.Where(x => (DbFunctions.TruncateTime(x.DateСreation) == DbFunctions.TruncateTime(DateTime.Now)) && (x.ProfileStatusID == (int)ProfileStatuses.Undefined) && (x.ExecutionStatusID == (int)ExecutionStatuses.Created)).Count();

                return result;
            }
        }

        public void AddProfiles(IEnumerable<Profile> profiles)
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

                db.SaveChanges();
            }
        }

        public void UpdateProfile(Profile profile)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var profileDB = db.Profiles.Where(x => x.Id == profile.Id).FirstOrDefault();

                profileDB.FirstName = profile.FirstName ?? "";
                profileDB.LastName = profile.LastName ?? "";
                profileDB.FullName = profile.FullName ?? "";
                profileDB.Job = profile.Job ?? "";
                profileDB.AllSkills = profile.AllSkills ?? "";
                profileDB.ExecutionStatusID = profile.ExecutionStatusID;
                profileDB.ProfileStatusID = profile.ProfileStatusID;
                profileDB.DateСreation = profile.DateСreation;

                db.SaveChanges();
            }
        }

        public void UpdateProfilesExecutionStatusByCompanyID(ExecutionStatuses executionStatus, int companyID)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                db.Profiles.Where(x => x.CompanyID == companyID).ToList().ForEach(y => y.ExecutionStatusID = (int)executionStatus);
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