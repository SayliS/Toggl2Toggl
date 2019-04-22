using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MarketVisionClientPhraseMapping : IPhraseMapping<IClient>
    {
        readonly IDictionary<string, HashSet<string>> mapping;

        public MarketVisionClientPhraseMapping()
        {
            mapping = new Dictionary<string, HashSet<string>>();

            Map(MarketVisionClient.MarketVision, "NewGen");
            Map(MarketVisionClient.LinkBiosciences, "LinkPro");
            Map(MarketVisionClient.MarketVision, "Grooming");
            Map(MarketVisionClient.MarketVision, "Groom");
            Map(MarketVisionClient.MarketVision, "Review");
            Map(MarketVisionClient.MarketVision, "planning");
            Map(MarketVisionClient.MarketVision, "Idle time");
            Map(MarketVisionClient.MarketVision, "General Customer Support");
            Map(MarketVisionClient.Pruvit, "RamoSoft Commission Statements");

            ClientsToWords();
        }

        public void Map(IClient client, string word)
        {
            if (mapping.ContainsKey(client.ClientName) == false)
            {
                mapping.Add(client.ClientName, new HashSet<string>());
            }

            mapping[client.ClientName].Add(word);
        }

        public bool TryGet(string phrase, out IClient client)
        {
            client = MarketVisionClient.NoClient;

            var found = mapping.FirstOrDefault(x => x.Value.Any(y => phrase.Contains(y, StringComparison.OrdinalIgnoreCase)));

            if (string.IsNullOrEmpty(found.Key) == false)
            {
                client = MarketVisionClient.Parse(found.Key);
                return true;
            }

            return false;
        }


        void ClientsToWords()
        {
            var properties = typeof(MarketVisionClient).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MarketVisionClient));
            var clients = properties.Select(x => x.GetValue(null) as MarketVisionClient).Where(x => x != MarketVisionClient.NoClient).ToList();

            foreach (var client in clients)
            {
                Map(client, client.ClientName);
            }

        }
    }
}
