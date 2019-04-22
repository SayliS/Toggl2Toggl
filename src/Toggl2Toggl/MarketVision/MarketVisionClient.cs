using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MarketVisionClient : IClient
    {
        static Dictionary<string, Func<MarketVisionClient>> MarketVisionClients = new Dictionary<string, Func<MarketVisionClient>>();

        MarketVisionClient() { }

        MarketVisionClient(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            ClientName = value;
        }
        public string ClientName { get; private set; }

        public static MarketVisionClient NoClient = new MarketVisionClient("No Client");
        public static MarketVisionClient BodHd = new MarketVisionClient("BodHd");
        public static MarketVisionClient Cellements = new MarketVisionClient("Cellements");
        public static MarketVisionClient Cellis = new MarketVisionClient("Cellis");
        public static MarketVisionClient Innov8tive = new MarketVisionClient("Innov8tive");
        public static MarketVisionClient LinkBiosciences = new MarketVisionClient("Link Biosciences");
        public static MarketVisionClient LivLabs = new MarketVisionClient("LivLabs");
        public static MarketVisionClient MarketVision = new MarketVisionClient("MarketVision");
        public static MarketVisionClient Pruvit = new MarketVisionClient("Pruvit");
        public static MarketVisionClient Uforia = new MarketVisionClient("Uforia");

        public static implicit operator string(MarketVisionClient current)
        {
            return current.ClientName;
        }

        public override string ToString()
        {
            return ClientName;
        }

        public static MarketVisionClient Parse(string client)
        {
            client = client?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(client) == true)
                client = string.Empty;

            Func<MarketVisionClient> foundClinet = null;

            if (MarketVisionClients.TryGetValue(client, out foundClinet))
            {
                return foundClinet();
            }

            switch (client)
            {
                case @"all":
                    return MarketVision;
                default:
                    return MarketVision;
            }
        }

        static void SetupClinets()
        {
            if (MarketVisionClients.Count == 0)
            {
                var properties = typeof(MarketVisionClient).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MarketVisionClient));
                var clients = properties.Select(x => x.GetValue(null) as MarketVisionClient).Where(x => x != NoClient).ToList();

                foreach (var client in clients)
                {
                    MarketVisionClients.Add(client.ClientName.ToLower(), () => client);
                }
            }
        }
    }
}
