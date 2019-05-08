using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MvClientResolverBasedOnAdoClientTag : AdoTimeEntryBasedResolver<IClient>
    {
        public MvClientResolverBasedOnAdoClientTag(AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
        }

        public override bool TryResolve(TimeEntry entry, out IClient result)
        {
            result = default(IClient);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                result = MvClient.Parse(adoTicket.ClientName);
                return true;
            }

            return false;
        }
    }
}
