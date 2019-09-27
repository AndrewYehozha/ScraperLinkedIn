using ScraperLinkedIn.Repositories;
using ScraperLinkedIn.Repositories.Interfaces;
using ScraperLinkedIn.Services.Interfaces;

namespace ScraperLinkedIn.Services
{
    class LoggerService : ILoggerService
    {
        private ILoggerRepository _loggerRepository;

        public LoggerService()
        {
            _loggerRepository = new LoggerRepository();
        }

        public async void Add(string message)
        {
            await _loggerRepository.AddAsync(message);
        }

        public async void Add(string remarks, string message)
        {
            await _loggerRepository.AddAsync(remarks, message);
        }
    }
}