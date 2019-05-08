using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MvTagResolveBasedOnAdo : AdoTimeEntryBasedResolver<ITag>
    {
        readonly ITag defaultTag;

        public MvTagResolveBasedOnAdo(ITag defaultTag, AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
            this.defaultTag = defaultTag;
        }

        public override bool TryResolve(TimeEntry entry, out ITag result)
        {
            result = default(ITag);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                result = defaultTag;
                return true;
            }

            return false;
        }
    }
}
