using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Models
{
    class CompanyEmployeesViewModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationNameURL { get; set; }
        public string Founders { get; set; }
        public string HeadquartersLocation { get; set; }
        public string Website { get; set; }
        public string LinkedIn { get; set; }
        public string Categories { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string PhoneNumber { get; set; }
        public string NumberofEmployees { get; set; }
        public string LogoCompanyUrl { get; set; }
        public string Specialties { get; set; }
        public ExecutionStatuses ExecutionStatus { get; set; }
        public List<ProfileViewModel> Employees { get; set; }
    }
}