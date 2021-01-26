// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.BotFramework.Composer.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.BotFramework.Composer.WebAppTemplates
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, builder) =>
            {
                var env = hostingContext.HostingEnvironment;

                // Use Composer bot path adapter
                builder.UseBotPathConverter(env.IsDevelopment());

                var configuration = builder.Build();

                var botRoot = configuration.GetValue<string>("bot");
                var configFile = Path.GetFullPath(Path.Combine(botRoot, @"settings/appsettings.json"));

                builder.AddJsonFile(configFile, optional: true, reloadOnChange: true);

                string password = Environment.GetEnvironmentVariable("MicrosoftAppPassword");
                configuration["MicrosoftAppPassword"] = password;

                // Use Composer luis and qna settings extensions
                builder.UseComposerSettings();

                builder.AddEnvironmentVariables()
                       .AddCommandLine(args);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
