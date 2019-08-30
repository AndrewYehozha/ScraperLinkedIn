using System;

namespace ScraperLinkedIn
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new Scraper();
            scraper.Initialize();
            Environment.Exit(0);
        }
    }
}
