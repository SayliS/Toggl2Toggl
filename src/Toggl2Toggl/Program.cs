using System;
using System.IO;
using AdoIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Toggl2Toggl
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args).UseConsoleLifetime();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<Application>()
                .Configure<Toggl2TogglOptions>(x => configuration.Bind("Toggl2Toggl", x))
                .BuildServiceProvider();

            var app = serviceProvider.GetRequiredService<Application>();
            app.Run();
        }
    }

    public class Application
    {
        Toggl2TogglOptions options;

        public Application(IOptions<Toggl2TogglOptions> options)
        {
            this.options = options.Value;
        }

        public void Run()
        {
            var startDate = options.GetStartDate();
            var endDate = DateTime.UtcNow.EndOfDay();

            var toggl2ToggleIntegration = new Toggl2TogglIntegration(options.TogglApiKey);
            var adoClient = new AdoIntegrationClient(options.Ado.OrganizationUrl, options.Ado.AccessToken);
            toggl2ToggleIntegration.AddClientResolver(new MvClientResolverBasedOnAdoClientTag(adoClient, 100));
            toggl2ToggleIntegration.AddClientResolver(new PhraseTimeEntryBasedResolver<IClient>(new MvClientPhraseMapping(), 50));
            toggl2ToggleIntegration.AddClientResolver(new DefaultTimeEntryBasedResolver<IClient>(MvClient.MarketVision, 1));


            //toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoAreadPath(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
            toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoItemType(adoClient, 100));
            toggl2ToggleIntegration.AddProjectResolver(new PhraseTimeEntryBasedResolver<IProject>(new MvProjectPhraseMapping(), 50));
            toggl2ToggleIntegration.AddProjectResolver(new DefaultTimeEntryBasedResolver<IProject>(MvProject.NewGenSupport, 1));


            toggl2ToggleIntegration.AddTagResolver(new MvTagResolveBasedOnAdo(MvTag.Development, adoClient, 50));
            toggl2ToggleIntegration.AddTagResolver(new PhraseTimeEntryBasedResolver<ITag>(new MvTagPhraseMapping(), 40));

            switch (options.Verb)
            {
                case "sync":
                    toggl2ToggleIntegration.Sync(startDate, endDate, "Elders workspace", "NewGen", "MarketVision's workspace");
                    break;
                case "show":
                    toggl2ToggleIntegration.Show(startDate, endDate, "Elders workspace", "NewGen");
                    break;
                default:
                    throw new NotSupportedException($"Not supported verb '{options.Verb}'");
            }
        }
    }

    public class Toggl2TogglOptions
    {
        public string TogglApiKey { get; set; }

        public string Verb { get; set; }

        public DateTime? Since { get; set; }

        public AdoOptions Ado { get; set; }

        public DateTime GetStartDate()
        {
            if (Since.HasValue)
                return Since.Value;

            return DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday).AddDays(-7);
        }

        public class AdoOptions
        {
            public string AccessToken { get; set; }
            public Uri OrganizationUrl { get; set; }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
