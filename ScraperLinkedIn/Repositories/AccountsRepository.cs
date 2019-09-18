using ScraperLinkedIn.Database;
using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Models;
using System;
using System.Linq;

namespace ScraperLinkedIn.Repositories
{
    class AccountsRepository
    {
        public SettingsViewModel GetAccountSettings()
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Settings.FirstOrDefault();

                return MapperConfigurationModel.Instance.Map<Setting, SettingsViewModel>(result);
            }
        }

        public void UpdateAccountSettings(Setting setting)
        {
            using (var db = new ScraperLinkedInDBEntities())
            {
                var result = db.Settings.FirstOrDefault();

                result.TimeStart = setting.TimeStart;
                result.IntervalType = setting.IntervalType;
                result.IntervalValue = setting.IntervalValue;
                result.Token = setting.Token;
                result.CompanyBatchSize = setting.CompanyBatchSize;
                result.ProfileBatchSize = setting.ProfileBatchSize;
                result.Login = setting.Login;
                result.Password = setting.Password;
                result.DateUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }
    }
}