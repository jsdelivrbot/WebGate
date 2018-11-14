using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebGateCr
{
    public class Program
    {
        public const string LogSec = @"Logging";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .Build();

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseKestrel()
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   var env = hostingContext.HostingEnvironment;
                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConfiguration(hostingContext.Configuration.GetSection(LogSec));
               })
               .UseStartup<Startup>()
               .Build();
    }
}
