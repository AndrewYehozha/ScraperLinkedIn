using ScraperLinkedIn.Types;
using System.Collections.Generic;

namespace ScraperLinkedIn.Models
{
    class CompanyEmployeesViewModel : CompanyImportViewModel
    {
        public int Id { get; set; }
        public string LogoCompanyUrl { get; set; }
        public string Specialties { get; set; }
        public ExecutionStatuses ExecutionStatus { get; set; }
        public IEnumerable<ProfileViewModel> Employees { get; set; }
    }
}