using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PinotageTodo.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = BuildConfiguration(args);
            BuildWebHost(args, config).Run();
        }

        public static IConfigurationRoot BuildConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public static IWebHost BuildWebHost(string[] args, IConfigurationRoot config)
        {
            var isDevelopment = config.GetValue<string>("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase);

            if (isDevelopment.HasValue && isDevelopment.Value == true)
            {
				var certificateSettings = config.GetSection("certificateSettings");
				var certificateFileName = certificateSettings.GetValue<string>("filename");
				var certificatePassword = certificateSettings.GetValue<string>("password");
				var cert = new X509Certificate2(certificateFileName, certificatePassword);

                return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Loopback, 14433, listenOptions =>
                        {
                            listenOptions.UseHttps(cert);
                        });
                    })
                    .Build();
            }
            else
            {
				return WebHost.CreateDefaultBuilder(args)
					.UseStartup<Startup>()
					.Build();
            }
        }
    }
}
