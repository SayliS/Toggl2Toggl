using System;
using AdoIntegration;

namespace Toggl2Toggl
{
    class Program
    {
        static void Main(string[] args)
        {
            var toggleApiKey = "";
            var adoPersonalAccessToken = "";
            var orgUrl = new Uri("https://marketvision.visualstudio.com");

            var startDate = DateTime.UtcNow.AddMonths(-2).StartOfDay();
            var endDate = DateTime.UtcNow.EndOfDay();

            var toggl2ToggleIntegration = new Toggl2TogglIntegration(toggleApiKey);
            var adoClient = new AdoIntegrationClient(orgUrl, adoPersonalAccessToken);
            toggl2ToggleIntegration.AddClientResolver(new MvClientResolverBasedOnAdoClientTag(adoClient, 100));
            toggl2ToggleIntegration.AddClientResolver(new PhraseTimeEntryBasedResolver<IClient>(new MvClientPhraseMapping(), 50));
            toggl2ToggleIntegration.AddClientResolver(new DefaultTimeEntryBasedResolver<IClient>(MvClient.MarketVision, 1));


            //toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoAreadPath(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
            toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoItemType(adoClient, 100));
            toggl2ToggleIntegration.AddProjectResolver(new PhraseTimeEntryBasedResolver<IProject>(new MvProjectPhraseMapping(), 50));
            toggl2ToggleIntegration.AddProjectResolver(new DefaultTimeEntryBasedResolver<IProject>(MvProject.NewGenSupport, 1));


            toggl2ToggleIntegration.AddTagResolver(new MvTagResolveBasedOnAdo(MvTag.Development, adoClient, 50));
            toggl2ToggleIntegration.AddTagResolver(new PhraseTimeEntryBasedResolver<ITag>(new MvTagPhraseMapping(), 40));

            //toggl2ToggleIntegration.Show(startDate, endDate, "Elders workspace");
            toggl2ToggleIntegration.Sync(startDate, endDate, "Elders workspace", "MarketVision's workspace");

            Console.ReadKey();
        }
    }
}
