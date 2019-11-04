using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvProjectPhraseMapping : IPhraseMapping<IProject>
    {
        readonly IDictionary<string, HashSet<string>> mapping;

        public MvProjectPhraseMapping()
        {
            mapping = new Dictionary<string, HashSet<string>>();

            Map(MvProject.NewGenSupport, "Grooming");
            Map(MvProject.NewGenSupport, "Groom");
            Map(MvProject.NewGenSupport, "Demo");
            Map(MvProject.NewGenSupport, "Review");
            Map(MvProject.NewGenSupport, "Production");
            Map(MvProject.NewGenSupport, "Deployment");
            Map(MvProject.NewGenSupport, "Retrospective");
            Map(MvProject.NewGenSupport, "planning");
            Map(MvProject.NewGenSupport, "NewGen/Legacy Standup");
            Map(MvProject.NewGenSupport, "NewGen/Genesis - Standup");
            Map(MvProject.NewGenSupport, "MarketVision State of company");
            Map(MvProject.NewGenSupport, "MarketVision");
            Map(MvProject.NewGenSupport, "General Customer Support");
            Map(MvProject.NewGenProject, "RamoSoft Commission Statements");
            Map(MvProject.NewGenSupport, "Idle time");
            Map(MvProject.NewGenSupport, "Overview of Dev Process with VAPT team");
            Map(MvProject.NewGenSupport, "maintanence");
            Map(MvProject.Genesis, "Genesis");

            ProjectsToWords();
        }

        public void Map(IProject project, string word)
        {
            if (mapping.ContainsKey(project.Name) == false)
            {
                mapping.Add(project.Name, new HashSet<string>());
            }

            mapping[project.Name].Add(word);
        }

        public bool TryGet(string phrase, out IProject project)
        {
            project = MvProject.NoProject;

            var xx = mapping.FirstOrDefault(x => x.Value.Any(y => phrase.Contains(y, StringComparison.OrdinalIgnoreCase)));

            if (string.IsNullOrEmpty(xx.Key) == false)
            {
                project = MvProject.Parse(xx.Key);
                return true;
            }

            return false;
        }

        void ProjectsToWords()
        {
            var properties = typeof(MvProject).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvProject));
            var projects = properties.Select(x => x.GetValue(null) as MvProject).Where(x => x != MvProject.NoProject).ToList();

            foreach (var project in projects)
            {
                Map(project, project.Name);
            }

        }
    }
}
