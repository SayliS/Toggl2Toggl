namespace AdoIntegration
{
    public class AdoTicketModel
    {
        public AdoTicketModel(string clientName, string areaPath)
        {
            ClientName = clientName;
            AreaPath = areaPath;
        }

        public string ClientName { get; private set; }

        public string AreaPath { get; private set; }
    }
}
