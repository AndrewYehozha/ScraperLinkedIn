namespace ScraperLinkedIn.Database
{
    public partial class Profile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Job { get; set; }
        public string ProfileUrl { get; set; }
        public string AllSkills { get; set; }
        public int ExecutionStatusID { get; set; }
        public int CompanyID { get; set; }
        public int ProfileStatusID { get; set; }
        public System.DateTime DataСreation { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual ExecutionStatus ExecutionStatus { get; set; }
        public virtual ProfileStatus ProfileStatus { get; set; }
    }
}