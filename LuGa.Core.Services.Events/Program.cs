using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LuGa.Core.Services.Events
{
    class Program
    {
        static void Main(string[] args)
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
        }
    }
}
