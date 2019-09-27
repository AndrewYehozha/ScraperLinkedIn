using ScraperLinkedIn.Models;
using System.Collections.Generic;

namespace ScraperLinkedIn.Services.Interfaces
{
    interface ISuitableProfileService
    {
        void AddSuitableProfile(IEnumerable<ResultViewModel> results);
    }
}