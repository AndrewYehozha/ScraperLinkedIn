using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.Models
{
    class SettingsViewModel
    {
        public int Id { get; set; }
        public System.TimeSpan TimeStart { get; set; }
        public IntervalTypes IntervalType { get; set; }
        public int IntervalValue { get; set; }
        public string Token { get; set; }
        public int CompanyBatchSize { get; set; }
        public int ProfileBatchSize { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int AccountID { get; set; }
        public System.DateTime DateUpdate { get; set; }
    }
}