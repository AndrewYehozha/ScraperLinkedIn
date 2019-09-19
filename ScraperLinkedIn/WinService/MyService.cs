using ScraperLinkedIn.Scheduler;
using ScraperLinkedIn.Scrapers;
using ScraperLinkedIn.Services;
using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.WinService
{
    class MyService
    {
        private Scraper _scraper;
        private LoggerService _loggerService;
        private AccountsService _accountsService;

        public MyService()
        {
            _scraper = new Scraper();
            _loggerService = new LoggerService();
            _accountsService = new AccountsService();
        }

        public void OnStart()
        {
            _loggerService.Add("Scheduler service are starting...", "");

            var settings = _accountsService.GetAccountSettings();

            switch (settings.IntervalType)
            {
                case IntervalTypes.Second:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInSeconds(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in seconds");
                        settings = _accountsService.GetAccountSettings();
                        _scraper.Initialize();
                        _scraper.Run(settings);
                    });
                    break;
                case IntervalTypes.Hour:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInHours(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in hours");
                        settings = _accountsService.GetAccountSettings();
                        _scraper.Initialize();
                        _scraper.Run(settings);
                    });
                    break;

                case IntervalTypes.Day:
                    // IntervalInSeconds(start_hour, start_minute, days)
                    MyScheduler.IntervalInDays(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in days");
                        settings = _accountsService.GetAccountSettings();
                        _scraper.Initialize();
                        _scraper.Run(settings);
                    });
                    break;

                case IntervalTypes.Undefined:
                    _loggerService.Add("Error INTERVAL_TYPE", "Invalid IntervalType.Please, check the value of < INTERVAL_TYPE > in App.config.");
                    break;
            }

            _loggerService.Add("Scheduler service started", "");
        }

        public void OnStop()
        {
            _loggerService.Add("Scheduler service is stoping...", "");
            _scraper.Close();
            _loggerService.Add("Scheduler service stopped", "");
        }
    }
}