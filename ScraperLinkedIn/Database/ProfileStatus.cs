namespace ScraperLinkedIn.Database
{
    using System.Collections.Generic;
    
    public partial class ProfileStatus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProfileStatus()
        {
            this.Profiles = new HashSet<Profile>();
        }
    
        public int ProfileStatusID { get; set; }
        public string StatusName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}