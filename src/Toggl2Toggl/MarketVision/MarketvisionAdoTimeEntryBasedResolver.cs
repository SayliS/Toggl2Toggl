using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MarketvisionAdoTimeEntryBasedResolver : AdoTimeEntryBasedResolver, ITimeEntryBasedResolver<IClient>, ITimeEntryBasedResolver<IProject>
    {
        public MarketvisionAdoTimeEntryBasedResolver(AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
        }

        public override bool TryResolve(TimeEntry entry, out IClient client)
        {
            client = default(IClient);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                client = MarketVisionClient.Parse(adoTicket.ClientName);
                return true;
            }

            return false;
        }

        public override bool TryResolve(TimeEntry entry, out IProject project)
        {
            project = default(IProject);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                project = MarketVisionProject.Parse(adoTicket.AreaPath);
                return true;
            }

            return false;
        }
    }
}
