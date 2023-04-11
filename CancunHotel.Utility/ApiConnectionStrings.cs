using Microsoft.Extensions.Configuration;
using System;

namespace CancunHotel.Utility
{
    public static class ApiConnectionStrings
    {
        public static string CancunDB
        {
            get
            {
                var builder = new ConfigurationBuilder();
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{env}.json", optional: true);

                var configuration = builder.Build();
                return configuration.GetConnectionString("CancunDB");
            }
        }
    }
}
