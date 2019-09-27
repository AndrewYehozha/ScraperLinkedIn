using Topshelf;

namespace ScraperLinkedIn.WinService
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<MyService>(service =>
                {
                    service.ConstructUsing(s => new MyService());
                    service.WhenStarted(s => s.OnStart());
                    service.WhenStopped(s => s.OnStop(false));
                    service.WhenShutdown(s => s.OnShutdown());
                });

                //Setup Account that window service use to run.  
                configure.RunAsLocalService();
                configure.StartManually();
                configure.SetServiceName("ScraperLinkedInConsole");
                configure.SetDisplayName("ScraperLinkedInConsole");
                configure.SetDescription("Console app");
            });
        }
    }
}