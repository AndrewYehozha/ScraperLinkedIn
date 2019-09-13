using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Models
{
    class ProfileViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Job { get; set; }
        public string ProfileUrl { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public string AllSkills { get; set; }
        public ExecutionStatuses ExecutionStatus { get; set; }
        public int? CompanyID { get; set; }
        public ProfileStatuses ProfileStatus { get; set; }
        public System.DateTime DataСreation { get; set; }
    }
}