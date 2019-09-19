using ScraperLinkedIn.WinService;
using System;

namespace ScraperLinkedIn
{
    class Program
    {
        static void Main(string[] args)
        {
#if (DEBUG)
            (new MyService()).Start();
            Console.ReadKey(true);
#elif (!DEBUG)

            ConfigureService.Configure();
#endif
        }
    }
}