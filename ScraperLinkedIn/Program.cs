using ScraperLinkedIn.WinService;
using System;

namespace ScraperLinkedIn
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                (new MyService()).Start();
                Console.ReadKey(true);
            }
            else
            {
                ConfigureService.Configure();
            }
        }
    }
}