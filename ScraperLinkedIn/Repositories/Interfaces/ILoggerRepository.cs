using System.Threading.Tasks;

namespace ScraperLinkedIn.Repositories.Interfaces
{
    interface ILoggerRepository
    {
        Task AddAsync(string message);

        Task AddAsync(string remarks, string message);
    }
}