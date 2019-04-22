using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MarketVisionProject : IProject
    {
        static Dictionary<string, Func<MarketVisionProject>> MarketVisionProjects = new Dictionary<string, Func<MarketVisionProject>>();

        MarketVisionProject() { }

        MarketVisionProject(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            ProjectName = value;
        }
        public string ProjectName { get; private set; }

        public static MarketVisionProject NoProject = new MarketVisionProject("No Project");
        public static MarketVisionProject NewGenProject = new MarketVisionProject("NewGen Projects");
        public static MarketVisionProject NewGenSupport = new MarketVisionProject("NewGen Support");
        public static MarketVisionProject Arbor = new MarketVisionProject("Arbor");
        public static MarketVisionProject DevOps = new MarketVisionProject("DevOps");
        public static MarketVisionProject Vapt = new MarketVisionProject("Vapt");
        public static MarketVisionProject LinkPro = new MarketVisionProject("LinkPro");

        public static implicit operator string(MarketVisionProject current)
        {
            return current.ProjectName;
        }

        public override string ToString()
        {
            return ProjectName;
        }

        public static MarketVisionProject Parse(string client)
        {
            client = client?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(client) == true)
                client = string.Empty;

            Func<MarketVisionProject> foundClinet = null;

            if (MarketVisionProjects.TryGetValue(client, out foundClinet))
            {
                return foundClinet();
            }
            switch (client)
            {
                case @"marketvision\newgen\support":
                    return NewGenSupport;
                case @"marketvision\newgen\projects":
                    return NewGenProject;
                case @"marketvision\devops":
                    return DevOps;
                case @"marketvision\arbor":
                    return Arbor;
                case @"marketvision\linkpro":
                    return LinkPro;
                default:
                    return NoProject;
            }
        }

        static void SetupClinets()
        {
            if (MarketVisionProjects.Count == 0)
            {
                var properties = typeof(MarketVisionProject).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MarketVisionProject));
                var projects = properties.Select(x => x.GetValue(null) as MarketVisionProject).Where(x => x != NoProject).ToList();

                foreach (var project in projects)
                {
                    MarketVisionProjects.Add(project.ProjectName.ToLower(), () => project);
                }
            }
        }
    }
}
