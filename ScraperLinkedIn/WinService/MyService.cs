using ScraperLinkedIn.Scheduler;
using ScraperLinkedIn.Scrapers;
using ScraperLinkedIn.Services;
using ScraperLinkedIn.Services.Interfaces;
using ScraperLinkedIn.Types;

namespace ScraperLinkedIn.WinService
{
    class MyService
    {
        private Scraper _scraper;
        private ILoggerService _loggerService;
        private IAccountsService _accountsService;

        public MyService()
        {
            _scraper = new Scraper();
            _loggerService = new LoggerService();
            _accountsService = new AccountsService();
        }

        public void OnStart()
        {
            _loggerService.Add("Scheduler service are starting...", "");

            if (!_scraper.Initialize())
            {
                OnStop(true);
            }

            var settings = _accountsService.GetAccountSettings();

            switch (settings.IntervalType)
            {
                case IntervalTypes.Second:

                    _loggerService.Add("Start a schedule", "With an interval in seconds");

                    MyScheduler.IntervalInSeconds(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _scraper.Run(settings);
                    });
                    break;
                case IntervalTypes.Hour:

                    _loggerService.Add("Start a schedule", "With an interval in hours");

                    MyScheduler.IntervalInHours(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _scraper.Run(settings);
                    });
                    break;

                case IntervalTypes.Day:

                    _loggerService.Add("Start a schedule", "With an interval in days");

                    MyScheduler.IntervalInDays(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _scraper.Run(settings);
                    });
                    break;

                case IntervalTypes.Undefined:
                    _loggerService.Add("Error INTERVAL_TYPE", "Invalid IntervalType.Please, check the value of < INTERVAL_TYPE > in App.config.");
                    break;
            }
        }

        public void OnStop(bool isException)
        {
            _loggerService.Add("Scheduler service is stoping...", "");
            _scraper.Close();
            _loggerService.Add("Scheduler service stopped", "");

            if (!isException)
            {
                _accountsService.UpdateScraperStatus(ScraperStatuses.OFF);
            }
        }

        public void OnShutdown()
        {
            _loggerService.Add("Scheduler service is stoping...", "");
            _scraper.Close();
            _loggerService.Add("Scheduler service stopped", "System shutdown");

            _accountsService.UpdateScraperStatus(ScraperStatuses.Exception);
        }
    }
}