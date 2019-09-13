using ScraperLinkedIn.Repositories;

namespace ScraperLinkedIn.Services
{
    class LoggerService
    {
        private LoggerRepository _loggerRepository;

        public LoggerService()
        {
            _loggerRepository = new LoggerRepository();
        }

        public void Add(string message)
        {
            _loggerRepository.Add(message);
        }

        public void Add(string remarks, string message)
        {
            _loggerRepository.Add(remarks, message);
        }
    }
}