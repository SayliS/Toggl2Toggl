using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvClientPhraseMapping : IPhraseMapping<IClient>
    {
        readonly IDictionary<string, HashSet<string>> mapping;

        public MvClientPhraseMapping()
        {
            mapping = new Dictionary<string, HashSet<string>>();

            Map(MvClient.MarketVision, "NewGen");
            Map(MvClient.LinkBiosciences, "LinkPro");
            Map(MvClient.MarketVision, "Grooming");
            Map(MvClient.MarketVision, "Groom");
            Map(MvClient.MarketVision, "Review");
            Map(MvClient.MarketVision, "planning");
            Map(MvClient.MarketVision, "Idle time");
            Map(MvClient.MarketVision, "General Customer Support");
            Map(MvClient.Pruvit, "RamoSoft Commission Statements");

            ClientsToWords();
        }

        public void Map(IClient client, string word)
        {
            if (mapping.ContainsKey(client.Name) == false)
            {
                mapping.Add(client.Name, new HashSet<string>());
            }

            mapping[client.Name].Add(word);
        }

        public bool TryGet(string phrase, out IClient client)
        {
            client = MvClient.NoClient;

            var found = mapping.FirstOrDefault(x => x.Value.Any(y => phrase.Contains(y, StringComparison.OrdinalIgnoreCase)));

            if (string.IsNullOrEmpty(found.Key) == false)
            {
                client = MvClient.Parse(found.Key);
                return true;
            }

            return false;
        }


        void ClientsToWords()
        {
            var properties = typeof(MvClient).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvClient));
            var clients = properties.Select(x => x.GetValue(null) as MvClient).Where(x => x != MvClient.NoClient).ToList();

            foreach (var client in clients)
            {
                Map(client, client.Name);
            }

        }
    }
}
