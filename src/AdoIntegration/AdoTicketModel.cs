namespace AdoIntegration
{
    public class AdoTicketModel
    {
        public AdoTicketModel(string clientName, string areaPath, WorkItemType workItemType)
        {
            ClientName = clientName;
            AreaPath = areaPath;
            WorkItemType = workItemType;
        }

        public string ClientName { get; private set; }

        public string AreaPath { get; private set; }

        public WorkItemType WorkItemType { get; private set; }
    }
}
