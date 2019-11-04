using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvProject : IProject
    {
        static readonly Dictionary<string, Func<MvProject>> MarketVisionProjects = new Dictionary<string, Func<MvProject>>();

        MvProject() { }

        MvProject(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            Name = value;
        }
        public string Name { get; private set; }

        public static MvProject NoProject = new MvProject("No Project");
        public static MvProject NewGenProject = new MvProject("NewGen Projects");
        public static MvProject NewGenSupport = new MvProject("NewGen Support");
        public static MvProject Arbor = new MvProject("Arbor");
        public static MvProject DevOps = new MvProject("DevOps");
        public static MvProject Vapt = new MvProject("Vapt");
        public static MvProject LinkPro = new MvProject("LinkPro");
        public static MvProject Genesis = new MvProject("Genesis");
        //public static MvProject LinkProBioScience = new MvProject("Link BioSciences");

        public static implicit operator string(MvProject current)
        {
            return current.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static MvProject Parse(string client)
        {
            client = client?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(client) == true)
                client = string.Empty;

            if (MarketVisionProjects.TryGetValue(client, out Func<MvProject> foundClinet))
            {
                return foundClinet();
            }

            return NoProject;
        }

        static void SetupClinets()
        {
            if (MarketVisionProjects.Count == 0)
            {
                var properties = typeof(MvProject).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvProject));
                var projects = properties.Select(x => x.GetValue(null) as MvProject).Where(x => x != NoProject).ToList();

                foreach (var project in projects)
                {
                    MarketVisionProjects.Add(project.Name.ToLower(), () => project);
                }
            }
        }
    }
}
