using System.Collections.Generic;

namespace ScraperLinkedIn.Models
{
    class CompaniesEmployeesViewModel
    {
        public string OrganizationName { get; set; }
        public string OrganizationNameURL { get; set; }
        public string Founders { get; set; }
        public string HeadquartersLocation { get; set; }
        public string Website { get; set; }
        public string LinkedIn { get; set; }
        public string LogoCompanyUrl { get; set; }
        public string Specialties { get; set; }
        public IEnumerable<ProfilesViewModel> Employees { get; set; }
    }
}