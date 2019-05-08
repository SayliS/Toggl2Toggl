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

            Map(MvTag.Meeting, "Groom");
            Map(MvTag.Meeting, "Planning");
            Map(MvTag.Meeting, "Standup");
            Map(MvTag.Meeting, "Meeting");
            Map(MvTag.Meeting, "Training");
            Map(MvTag.Meeting, "Retrospective");
            Map(MvTag.Meeting, "Project Demo");
            Map(MvTag.Meeting, "State of company");


            Map(MvTag.Development, "Deployment");
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
