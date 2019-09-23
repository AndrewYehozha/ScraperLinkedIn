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

        public async void OnStart()
        {
            _loggerService.Add("Scheduler service are starting...", "");

            var settings = await _accountsService.GetAccountSettingsAsync();
            _scraper.Initialize();

            switch (settings.IntervalType)
            {
                case IntervalTypes.Second:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    _loggerService.Add("Start a schedule", "With an interval in seconds");

                    MyScheduler.IntervalInSeconds(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _scraper.Run(settings);
                    });
                    break;
                case IntervalTypes.Hour:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    _loggerService.Add("Start a schedule", "With an interval in hours");

                    MyScheduler.IntervalInHours(settings.TimeStart.Hours, settings.TimeStart.Minutes, settings.IntervalValue,
                    () =>
                    {
                        _scraper.Run(settings);
                    });
                    break;

                case IntervalTypes.Day:
                    // IntervalInSeconds(start_hour, start_minute, days)
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

        public void OnStop()
        {
            _loggerService.Add("Scheduler service is stoping...", "");
            _scraper.Close();
            _loggerService.Add("Scheduler service stopped", "");
        }
    }
}