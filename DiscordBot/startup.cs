using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Startup
    {
        // Puts startup configuration in a new file for cleanliness
        public static string GetConfigValue(string key)
        {
            var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(
                "appsettings.json",
                 optional: false,
                 reloadOnChange: true
            );

            IConfigurationRoot configuration = builder.Build();
            return configuration[key] ?? throw new ArgumentException("Config value not found or failed to load", key);
        }
    }
}
