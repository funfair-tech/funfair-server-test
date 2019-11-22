using FunFair.Test.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Startup
{
    internal static class LoggingStartup
    {
        public static void AddLoggingSupport(IServiceCollection services, ITestOutputHelper output)
        {
            services.AddLogging(configure: b => AddFilters(b, output));
        }

        private static void AddFilters(ILoggingBuilder builder, ITestOutputHelper output)
        {
            builder.ClearProviders();
            builder.AddXUnit(output);
            builder.AddFilter(category: @"Microsoft", LogLevel.Warning)
                .AddFilter(category: @"System.Net.Http.HttpClient", LogLevel.Warning);
        }
    }
}