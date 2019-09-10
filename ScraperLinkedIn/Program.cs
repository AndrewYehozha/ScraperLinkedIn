using ScraperLinkedIn.Scheduler;
using ScraperLinkedIn.Scrapers;
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

        static void Main(string[] args)
        {
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("Scheduler service are starting...\n");

            if (!DateTime.TryParseExact(ConfigurationManager.AppSettings["TIME_START"], "h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeStart))
            {
                Console.WriteLine("TimeStart must be a DateTime.\nPlease, check the value of <TIME_START> in App.config.\n\n");
            }

            if(!Int32.TryParse(ConfigurationManager.AppSettings["INTERVAL"], out Interval))
            {
                Console.WriteLine("Interval must be an integer.\nPlease, check the value of <INTERVAL> in App.config.\n\n");
            }

            Enum.TryParse(ConfigurationManager.AppSettings["INTERVAL_TYPE"], out IntervalType);


            var scraper = new Scraper();
            scraper.Initialize();
            scraper.Run();

            switch (IntervalType)
            {
                case IntervalTypes.Second:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInSeconds(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        Console.WriteLine("Start a schedule with an interval in seconds");
                        scraper.Run();
                    });
                    break;
                case IntervalTypes.Hour:
                    // IntervalInSeconds(start_hour, start_minute, hours)
                    MyScheduler.IntervalInHours(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        Console.WriteLine("Start a schedule with an interval in hours");
                        scraper.Run();
                    });
                    break;

                case IntervalTypes.Day:
                    // IntervalInSeconds(start_hour, start_minute, days)
                    MyScheduler.IntervalInDays(TimeStart.Hour, TimeStart.Minute, Interval,
                    () =>
                    {
                        Console.WriteLine("Start a schedule with an interval in days");
                        scraper.Run();
                    });
                    break;

                case IntervalTypes.Undefined:
                    Console.WriteLine("Invalid IntervalType.\nPlease, check the value of <INTERVAL_TYPE> in App.config.\n\n");
                    break;
            }

            Console.WriteLine("Scheduler Service are listening...Press <ENTER> to terminate.");
            Console.WriteLine("-----------------------------------------------------------------------\n");
            Console.ReadLine();

            scraper.Close();
        }
    }
}