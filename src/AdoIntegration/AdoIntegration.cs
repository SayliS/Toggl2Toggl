using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoIntegration
{
    public class AdoIntegrationClient
    {
        readonly Uri organizationUrl;
        readonly string personalAccessToken;
        readonly VssConnection connection;
        readonly IMemoryCache cache;

        public AdoIntegrationClient(Uri organizationUrl, string personalAccessToken)
        {
            this.cache = new MemoryCache(new MemoryCacheOptions());
            this.organizationUrl = organizationUrl;
            this.personalAccessToken = personalAccessToken;
            this.connection = new VssConnection(organizationUrl, new VssBasicCredential(string.Empty, personalAccessToken));
        }

        public bool TryGetAdoData(int byAdoTicketId, out AdoTicketModel adoTicket)
        {
            adoTicket = default(AdoTicketModel);
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            if (cache.TryGetValue(byAdoTicketId, out WorkItem workitem) == false)
            {

                workitem = witClient.GetWorkItemAsync(byAdoTicketId, expand: WorkItemExpand.All).Result;
                cache.Set(byAdoTicketId, workitem);
            }

            workitem.Fields.TryGetValue("Custom.Client", out string clientName);

            workitem.Fields.TryGetValue("System.WorkItemType", out string workItemType);

            // TODO: Work with item parents in case client is not found.

            workitem.Fields.TryGetValue("System.AreaPath", out string areaPath);

            if (string.IsNullOrEmpty(areaPath) == false || string.IsNullOrEmpty(clientName) == false)
            {
                adoTicket = new AdoTicketModel(clientName, areaPath, WorkItemType.Parse(workItemType));
                return true;
            }

            return false;
        }
    }
}
