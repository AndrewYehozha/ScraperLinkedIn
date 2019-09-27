namespace ScraperLinkedIn.Services.Interfaces
{
    interface ILoggerService
    {
        void Add(string message);

        void Add(string remarks, string message);
    }
}