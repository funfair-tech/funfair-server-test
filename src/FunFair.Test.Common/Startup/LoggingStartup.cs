namespace FunFair.Test.Common.Startup
{
    internal static class LoggingStartup
    {
        public static void AddLoggingSupport(IServiceCollection services, ITestOutputHelper output)
        {
            services.AddLogging(configure: builder => AddFilters(builder: builder, output: output));
        }

        private static void AddFilters(ILoggingBuilder builder, ITestOutputHelper output)
        {
            builder.ClearProviders()
                   .AddXUnit(output)
                   .AddFilter(category: @"Microsoft", level: LogLevel.Warning)
                   .AddFilter(category: @"System.Net.Http.HttpClient", level: LogLevel.Warning)
                   .SetMinimumLevel(LogLevel.Trace);
        }
    }
}