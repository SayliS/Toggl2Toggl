using System.Collections.Generic;
using AdoIntegration;
using Toggl;

namespace Toggl2Toggl
{
    public class MvProjectResolveBasedOnAdoAreadPath : AdoTimeEntryBasedResolver<IProject>
    {
        readonly IDictionary<string, IProject> mapping;

        public MvProjectResolveBasedOnAdoAreadPath(AdoIntegrationClient adoIntegrationClient, int weight) : base(adoIntegrationClient, weight)
        {
            mapping = new Dictionary<string, IProject>();
            Map(@"marketvision\newgen\support", MvProject.NewGenSupport);
            Map(@"marketvision\newgen\projects", MvProject.NewGenProject);
            Map(@"marketvision\devops", MvProject.DevOps);
            Map(@"marketvision\arbor", MvProject.Arbor);
            Map(@"marketvision\linkpro", MvProject.LinkPro);
        }

        public void Map(string areaPath, IProject project)
        {
            if (mapping.ContainsKey(areaPath) == true)
                throw new System.InvalidOperationException($"Area Path {areaPath} is already mapped");

            mapping.Add(areaPath.ToLower(), project);
        }

        public override bool TryResolve(TimeEntry entry, out IProject result)
        {
            result = default(IProject);
            if (TryExtractAdoTicketNumber(entry, out int adoId) == false) return false;

            if (adoIntegrationClient.TryGetAdoData(adoId, out AdoTicketModel adoTicket))
            {
                if (mapping.TryGetValue(adoTicket.AreaPath.ToLower(), out result) == true)
                    return true;
            }

            return false;
        }
    }
}
