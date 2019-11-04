using System.Collections.Generic;
using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MvProjectResolveBasedOnAdoItemType : AdoTimeEntryBasedResolver<IProject>
    {
        readonly IDictionary<string, IProject> mapping;

        public MvProjectResolveBasedOnAdoItemType(AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
            mapping = new Dictionary<string, IProject>();
            Map(WorkItemType.ProductBacklogItem, MvProject.NewGenProject);
            Map(WorkItemType.Feature, MvProject.NewGenProject);
            Map(WorkItemType.Support, MvProject.NewGenSupport);
            Map(WorkItemType.Bug, MvProject.NewGenSupport);
            Map(WorkItemType.Task, MvProject.NewGenSupport);
            Map(WorkItemType.Epic, MvProject.NewGenProject);
        }

        public void Map(WorkItemType workItemType, IProject project)
        {
            if (mapping.ContainsKey(workItemType.Value) == true)
                throw new System.InvalidOperationException($"WorkItemType {workItemType} is already mapped");

            mapping.Add(workItemType.Value, project);
        }

        public override bool TryResolve(TimeEntry entry, out IProject result)
        {
            result = default(IProject);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                if (mapping.TryGetValue(adoTicket.WorkItemType, out result) == true)
                    return true;
            }

            return false;
        }
    }
}
