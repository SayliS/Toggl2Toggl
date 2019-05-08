using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MvProjectResolveBasedOnAdoAreadPath : AdoTimeEntryBasedResolver<IProject>
    {
        public MvProjectResolveBasedOnAdoAreadPath(AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
        }

        public override bool TryResolve(TimeEntry entry, out IProject result)
        {
            result = default(IProject);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                result = MvProject.Parse(adoTicket.AreaPath);
                return true;
            }

            return false;
        }
    }
}
