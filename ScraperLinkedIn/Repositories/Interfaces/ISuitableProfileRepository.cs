using ScraperLinkedIn.Database;
using System.Collections.Generic;

namespace ScraperLinkedIn.Repositories.Interfaces
{
    interface ISuitableProfileRepository
    {
        void AddSuitableProfile(IEnumerable<SuitableProfile> suitableProfiles);
    }
}