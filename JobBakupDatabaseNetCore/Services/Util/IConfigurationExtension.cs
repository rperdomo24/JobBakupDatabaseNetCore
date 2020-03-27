using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JobBakupDatabaseNetCore.Services.Util
{
    public static class IConfigurationExtension
    {
        /// <summary>
        /// Configurations from application settings.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IConfiguration ConfigurationFromAppSettings(this ConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .Build();
        }
    }

}
