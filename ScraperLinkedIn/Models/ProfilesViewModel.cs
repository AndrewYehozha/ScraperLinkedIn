using System.Collections.Generic;

namespace ScraperLinkedIn.Models
{
    class ProfilesViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Job { get; set; }
        public string ProfileUrl { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public string AllSkills { get; set; }
        public bool IsRead { get; set; }
    }
}