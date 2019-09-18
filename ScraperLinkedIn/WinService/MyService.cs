using ScraperLinkedIn.Scheduler;
using ScraperLinkedIn.Scrapers;
using ScraperLinkedIn.Services;
using ScraperLinkedIn.Types;
using System;
using System.Configuration;
using System.Globalization;

namespace ScraperLinkedIn.WinService
{
    class MyService
    {
        private DateTime TimeStart;
        private int Interval;
        private IntervalTypes IntervalType;
        private Scraper _scraper;
        private LoggerService _loggerService;

        public MyService()
        {
            _scraper = new Scraper();
            _loggerService = new LoggerService();
        }

        public void Start()
        {
            _loggerService.Add("Scheduler service are starting...", "");

            if (!DateTime.TryParseExact(ConfigurationManager.AppSettings["TIME_START"], "h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeStart))
            {
                _loggerService.Add("Error TIME_START", "TimeStart must be a DateTime. Please, check the value of <TIME_START> in App.config.");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["INTERVAL"], out Interval))
            {
                _loggerService.Add("Error INTERVAL", "Interval must be an integer. Please, check the value of <INTERVAL> in App.config.");
            }

            Enum.TryParse(ConfigurationManager.AppSettings["INTERVAL_TYPE"], out IntervalType);

            _scraper.Initialize();

            switch (IntervalType)
            {
                case IntervalTypes.Second:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInSeconds(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in seconds");
                        _scraper.Run();
                    });
                    break;
                case IntervalTypes.Hour:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInHours(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in hours");
                        _scraper.Run();
                    });
                    break;

                case IntervalTypes.Day:
                    // IntervalInSeconds(start_hour, start_minute, days)
                    MyScheduler.IntervalInDays(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule", "With an interval in days");
                        _scraper.Run();
                    });
                    break;

                case IntervalTypes.Undefined:
                    _loggerService.Add("Error INTERVAL_TYPE", "Invalid IntervalType.Please, check the value of < INTERVAL_TYPE > in App.config.");
                    break;
            }

            _loggerService.Add("Scheduler service started", "");
        }
        public void Stop()
        {
            _loggerService.Add("Scheduler service is stoping...", "");
            _scraper.Close();
            _loggerService.Add("Scheduler service stopped", "");
        }
    }
}