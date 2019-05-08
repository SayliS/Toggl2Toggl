using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvClient : IClient
    {
        static Dictionary<string, Func<MvClient>> MarketVisionClients = new Dictionary<string, Func<MvClient>>();

        MvClient() { }

        MvClient(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            ClientName = value;
        }
        public string ClientName { get; private set; }

        public static MvClient NoClient = new MvClient("No Client");
        public static MvClient BodHd = new MvClient("BodHd");
        public static MvClient Cellements = new MvClient("Cellements");
        public static MvClient Cellis = new MvClient("Cellis");
        public static MvClient Innov8tive = new MvClient("Innov8tive");
        public static MvClient LinkBiosciences = new MvClient("Link Biosciences");
        public static MvClient LivLabs = new MvClient("LivLabs");
        public static MvClient MarketVision = new MvClient("MarketVision");
        public static MvClient Pruvit = new MvClient("Pruvit");
        public static MvClient Uforia = new MvClient("Uforia");

        public static implicit operator string(MvClient current)
        {
            return current.ClientName;
        }

        public override string ToString()
        {
            return ClientName;
        }

        public static MvClient Parse(string client)
        {
            client = client?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(client) == true)
                client = string.Empty;

            Func<MvClient> foundClinet = null;

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
                var properties = typeof(MvClient).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvClient));
                var clients = properties.Select(x => x.GetValue(null) as MvClient).Where(x => x != NoClient).ToList();

                foreach (var client in clients)
                {
                    MarketVisionClients.Add(client.ClientName.ToLower(), () => client);
                }
            }
        }
    }
}
