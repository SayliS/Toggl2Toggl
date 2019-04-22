using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MarketVisionProjectPhraseMapping : IPhraseMapping<IProject>
    {
        readonly IDictionary<string, HashSet<string>> mapping;

        public MarketVisionProjectPhraseMapping()
        {
            mapping = new Dictionary<string, HashSet<string>>();

            Map(MarketVisionProject.NewGenSupport, "Grooming");
            Map(MarketVisionProject.NewGenSupport, "Groom");
            Map(MarketVisionProject.NewGenSupport, "Demo");
            Map(MarketVisionProject.NewGenSupport, "Review");
            Map(MarketVisionProject.NewGenSupport, "Production");
            Map(MarketVisionProject.NewGenSupport, "Deployment");
            Map(MarketVisionProject.NewGenSupport, "Retrospective");
            Map(MarketVisionProject.NewGenSupport, "planning");
            Map(MarketVisionProject.NewGenSupport, "NewGen/Legacy Standup");
            Map(MarketVisionProject.NewGenSupport, "MarketVision State of company");
            Map(MarketVisionProject.NewGenSupport, "MarketVision");
            Map(MarketVisionProject.NewGenSupport, "General Customer Support");
            Map(MarketVisionProject.NewGenProject, "RamoSoft Commission Statements");
            Map(MarketVisionProject.NewGenSupport, "Idle time");


            ProjectsToWords();
        }

        public void Map(IProject project, string word)
        {
            if (mapping.ContainsKey(project.ProjectName) == false)
            {
                mapping.Add(project.ProjectName, new HashSet<string>());
            }

            mapping[project.ProjectName].Add(word);
        }

        public bool TryGet(string phrase, out IProject project)
        {
            project = MarketVisionProject.NoProject;

            var xx = mapping.FirstOrDefault(x => x.Value.Any(y => phrase.Contains(y, StringComparison.OrdinalIgnoreCase)));

            if (string.IsNullOrEmpty(xx.Key) == false)
            {
                project = MarketVisionProject.Parse(xx.Key);
                return true;
            }

            return false;
        }

        void ProjectsToWords()
        {
            var properties = typeof(MarketVisionProject).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MarketVisionProject));
            var projects = properties.Select(x => x.GetValue(null) as MarketVisionProject).Where(x => x != MarketVisionProject.NoProject).ToList();

            foreach (var project in projects)
            {
                Map(project, project.ProjectName);
            }

        }
    }
}
