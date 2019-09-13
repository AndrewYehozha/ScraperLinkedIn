namespace ScraperLinkedIn.Database
{
    using System;
    
    public partial class DebugLog
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string Logs { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}