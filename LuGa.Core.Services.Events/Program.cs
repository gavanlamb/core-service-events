using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LuGa.Core.Repository;
using Microsoft.Extensions.Configuration;
using Topshelf;

namespace LuGa.Core.Services.Events
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string environment = Environment.GetEnvironmentVariable(Constants.Environment);

            if (String.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("Environment not found in:" + Constants.Environment);

            Debug.WriteLine("Environment: {0}", environment);

            // all passwords should be stored in 
            // %APPDATA%\microsoft\UserSecrets\luga\secrets.json
            // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            if (environment != null && environment.Contains("Dev"))
            {
                builder.AddUserSecrets<Program>();
            }

            var cfg = builder.Build();

            var connectionString = cfg.GetConnectionString(Constants.ConnectionString);
            
            return (int)HostFactory.Run(x =>
            {
                var mqttConfig = new LuGaMqttConfig
                (
                    cfg[Constants.Username],
                    cfg[Constants.Password],
                    cfg[Constants.ClientId],
                    cfg[Constants.Host],
                    Convert.ToInt32(cfg[Constants.Port]));
                
                var eventsRepository = new EventsRepository(connectionString);

                x.Service(y => new LuGaMqtt(mqttConfig, eventsRepository));

                x.SetServiceName(Constants.ServiceName);
                x.SetDisplayName(Constants.ServiceName);
            });
        }
    }
}
