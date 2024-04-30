using System.Diagnostics;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

using OtherEchoBot;
using OtherEchoBot.Adapters;
using OtherEchoBot.Bots;
using OtherEchoBot.Abstractions;
using OtherEchoBot.Services;
using OtherEchoBot.Options;

/* Load Configuration */

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     ;

var isDevelopment = builder.Environment.IsDevelopment();
var isStaging = builder.Environment.IsStaging(); // Usually, staging environments are used for testing purposes...

/* Logging Configuration */

var applicationInsightsConnectionString = builder.Configuration.GetConnectionString(Constants.ConnectionStrings.ApplicationInsights);

builder.Logging.AddApplicationInsights((telemetryConfiguration) => telemetryConfiguration.ConnectionString = applicationInsightsConnectionString, (_) => { })
               .AddConsole()
;

if (Debugger.IsAttached)
{
    builder.Logging.AddDebug();
}

/* Load Options */

builder.Services.AddOptionsWithValidateOnStart<AzureAISearchOptions>().Bind(builder.Configuration.GetSection(nameof(AzureAISearchOptions))).ValidateDataAnnotations();
builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>().Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions))).ValidateDataAnnotations();

/* Services */

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration)
                .AddLogging(loggingBuilder => loggingBuilder.AddApplicationInsights())
                .AddHttpClient()
                .AddHttpContextAccessor()
                .AddSingleton<IOpenAIService, OpenAIService>()
                .AddControllers(options =>
                {
                    options.SuppressAsyncSuffixInActionNames = true;
                })
                ;

/* Bot Configuration & Services */

builder.Services.AddSingleton<IBotTelemetryClient>(new BotTelemetryClient(new TelemetryClient(new TelemetryConfiguration { ConnectionString = applicationInsightsConnectionString })))
                .AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>()
                .AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>()
                .AddSingleton(sp =>
                {
                    var telemetryClient = sp.GetService<IBotTelemetryClient>();
                    return new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true);
                })
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, TelemetryLoggerMiddleware>(sp =>
                {
                    return sp.GetRequiredService<TelemetryLoggerMiddleware>();
                })
                .AddSingleton(sp =>
                {
                    var loggerMiddleware = sp.GetRequiredService<TelemetryLoggerMiddleware>();
                    var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                    return new TelemetryInitializerMiddleware(contextAccessor, loggerMiddleware, logActivityTelemetry: true);
                })
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, TelemetryInitializerMiddleware>(sp =>
                {
                    return sp.GetRequiredService<TelemetryInitializerMiddleware>();
                })
                .AddSingleton<IStorage, MemoryStorage>()
                .AddSingleton<ConversationState>()
                .AddSingleton<UserState>()
                .AddSingleton<BotState>(sp => sp.GetRequiredService<ConversationState>())
                .AddSingleton<BotState>(sp => sp.GetRequiredService<UserState>())
                .AddSingleton(sp => new AutoSaveStateMiddleware(sp.GetServices<BotState>().ToArray()))
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, AutoSaveStateMiddleware>(sp => sp.GetRequiredService<AutoSaveStateMiddleware>())
                .AddSingleton<ShowTypingMiddleware>()
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, ShowTypingMiddleware>(sp => sp.GetRequiredService<ShowTypingMiddleware>())
                ;

// Add bot services...
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>()
                .AddSingleton<BotCloudAdapterWithErrorHandlerServices>()
                .AddSingleton<IBotFrameworkHttpAdapter, BotCloudAdapterWithErrorHandler>()
                .AddTransient<IBot, EchoBot>()
                ;

/* Application Middleware Configuration */

var app = builder.Build();

if (isDevelopment || isStaging)
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting()
   .UseAuthentication()
   .UseAuthorization()
   .UseEndpoints(endpoints =>
   {
       endpoints.MapControllers();
   })
   ;

app.Run();
