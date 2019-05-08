using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvTagPhraseMapping : IPhraseMapping<ITag>
    {
        readonly IDictionary<string, HashSet<string>> mapping;

        public MvTagPhraseMapping()
        {
            mapping = new Dictionary<string, HashSet<string>>();

            Map(MvTag.Development, "Grooming");
            Map(MvTag.Development, "Deployment");
            Map(MvTag.Development, "planning");
            Map(MvTag.Development, "General Customer Support");
        }

        public void Map(ITag tag, string phrase)
        {
            if (mapping.ContainsKey(tag.Name) == false)
            {
                mapping.Add(tag.Name, new HashSet<string>());
            }

            mapping[tag.Name].Add(phrase);
        }

        public bool TryGet(string phrase, out ITag project)
        {
            project = default(ITag);

            var xx = mapping.FirstOrDefault(x => x.Value.Any(y => phrase.Contains(y, StringComparison.OrdinalIgnoreCase)));

            if (string.IsNullOrEmpty(xx.Key) == false)
            {
                project = MvTag.Parse(xx.Key);
                return true;
            }

            return false;
        }
    }
}
