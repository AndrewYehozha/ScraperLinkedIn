namespace ScraperLinkedIn.Models
{
    class ResultViewModel
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string PersonLinkedIn { get; set; } //url
        public string Company { get; set; } //company name
        public string Website { get; set; } //company website
        public string CompanyLogoUrl { get; set; }
        public string CrunchUrl { get; set; } //Crunch url
        public string Email { get; set; }
        public string EmailStatus { get; set; }
        public string CompanySpecialties { get; set; }
        public string TechStack { get; set; } //Company tech stack
    }
}