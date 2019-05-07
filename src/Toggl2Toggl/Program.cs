using System;
using AdoIntegration;

namespace Toggl2Toggl
{
    class Program
    {
        static void Main(string[] args)
        {

            var toggleApiKey = "";
            var startDate = DateTime.UtcNow.AddMonths(-1).StartOfDay();
            var endDate = DateTime.UtcNow.EndOfDay();
            var orgUrl = new Uri("https://marketvision.visualstudio.com");
            var adoPersonalAccessToken = "";

            var toggl2ToggleIntegration = new Toggl2TogglIntegration(toggleApiKey);
            toggl2ToggleIntegration.AddClientResolver(new MarketvisionAdoTimeEntryBasedResolver(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
            toggl2ToggleIntegration.AddClientResolver(new GenericTimeEntryBasedResolver<IClient>(new MarketVisionClientPhraseMapping(), 50));

            toggl2ToggleIntegration.AddProjectResolver(new MarketvisionAdoTimeEntryBasedResolver(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
            toggl2ToggleIntegration.AddProjectResolver(new GenericTimeEntryBasedResolver<IProject>(new MarketVisionProjectPhraseMapping(), 50));

            toggl2ToggleIntegration.Show(startDate, endDate, "Elders workspace");
            //toggl2ToggleIntegration.Sync(startDate, endDate, "Elders workspace", "MarketVision's workspace");

            Console.ReadKey();
        }
    }
}
