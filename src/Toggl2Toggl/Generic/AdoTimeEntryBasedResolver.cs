using System;
using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public abstract class AdoTimeEntryBasedResolver : ITimeEntryBasedResolver<IClient>, ITimeEntryBasedResolver<IProject>
    {
        protected readonly AdoIntegrationClient adoIntegrationClient;

        protected AdoTimeEntryBasedResolver(AdoIntegrationClient adoIntegrationClient, int weight)
        {
            this.adoIntegrationClient = adoIntegrationClient;
            Weight = weight;
        }

        public int Weight { get; private set; }

        public abstract bool TryResolve(TimeEntry entry, out IClient client);

        public abstract bool TryResolve(TimeEntry entry, out IProject project);

        protected bool TryExtractAdoTicketNumber(TimeEntry entry, out int adoId)
        {
            adoId = -1;
            var description = entry.Description;
            var firstWord = description.IndexOf(" ", StringComparison.OrdinalIgnoreCase) > -1 ? description.Substring(0, description.IndexOf(" ", StringComparison.OrdinalIgnoreCase)) : description;

            if (int.TryParse(firstWord, out adoId))
            {
                return true;
            }

            return false;
        }
    }
}
