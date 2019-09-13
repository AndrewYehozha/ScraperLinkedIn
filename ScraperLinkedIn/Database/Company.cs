namespace ScraperLinkedIn.Database
{
    using System.Collections.Generic;
    
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            this.Profiles = new HashSet<Profile>();
        }
    
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationURL { get; set; }
        public string Founders { get; set; }
        public string HeadquartersLocation { get; set; }
        public string Website { get; set; }
        public string LinkedInURL { get; set; }
        public string LogoUrl { get; set; }
        public string Specialties { get; set; }
        public int ExecutionStatusID { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string PhoneNumber { get; set; }
        public string AmountEmployees { get; set; }
        public string Industry { get; set; }
    
        public virtual ExecutionStatus ExecutionStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}