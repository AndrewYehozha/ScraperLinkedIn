using ScraperLinkedIn.Database.ObjectMappers;
using ScraperLinkedIn.Scheduler;
using ScraperLinkedIn.Scrapers;
using ScraperLinkedIn.Services;
using ScraperLinkedIn.Types;
using System;
using System.Configuration;
using System.Globalization;

namespace ScraperLinkedIn
{
    class Program
    {
        private static DateTime TimeStart;
        private static int Interval;
        private static IntervalTypes IntervalType;
        private static Scraper scraper = new Scraper();
        private static LoggerService _loggerService = new LoggerService();

        static void Main(string[] args)
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

            scraper.Initialize();
            scraper.Run();

            switch (IntervalType)
            {
                case IntervalTypes.Second:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInSeconds(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule with an interval in seconds");
                        scraper.Run();
                    });
                    break;
                case IntervalTypes.Hour:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInHours(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule with an interval in hours");
                        scraper.Run();
                    });
                    break;

                case IntervalTypes.Day:
                    // IntervalInSeconds(start_hour, start_minute, days)
                    MyScheduler.IntervalInDays(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        _loggerService.Add("Start a schedule with an interval in days");
                        scraper.Run();
                    });
                    break;

                case IntervalTypes.Undefined:
                    _loggerService.Add("Error INTERVAL_TYPE", "Invalid IntervalType.Please, check the value of < INTERVAL_TYPE > in App.config.");
                    break;
            }

            _loggerService.Add("Scheduler service is stoping...", "");
            scraper.Close();
        }
    }
}