using System;
using System.IO;
using log4net;
using log4net.Config;
using MongoDB.Driver;
using ProjectsHandler;

namespace ReadModelTool
{
    class Program
    {
        static void Main()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            var log = LogManager.GetLogger("ReadModelTool");
            var applicationSettings = new ApplicationSettings();

            CleanUpReadModel(applicationSettings);
            var replayer = new Replayer(applicationSettings, log);

            Console.WriteLine("Starting to regenerate the read model...");

            replayer.ProcessEvents().Wait();

            Console.WriteLine("Done!");
            Console.Write("\r\r\rHit enter to exit...");
            Console.ReadLine();
        }

        private static void CleanUpReadModel(IApplicationSettings applicationSettings)
        {
            var client = new MongoClient(applicationSettings.MongoDbConnectionString);
            client.DropDatabaseAsync(applicationSettings.MongoDbName).Wait();
        }
    }
}
