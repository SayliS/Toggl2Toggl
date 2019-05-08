using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Toggl
{
    public class MvProject : IProject
    {
        static Dictionary<string, Func<MvProject>> MarketVisionProjects = new Dictionary<string, Func<MvProject>>();

        MvProject() { }

        MvProject(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            ProjectName = value;
        }
        public string ProjectName { get; private set; }

        public static MvProject NoProject = new MvProject("No Project");
        public static MvProject NewGenProject = new MvProject("NewGen Projects");
        public static MvProject NewGenSupport = new MvProject("NewGen Support");
        public static MvProject Arbor = new MvProject("Arbor");
        public static MvProject DevOps = new MvProject("DevOps");
        public static MvProject Vapt = new MvProject("Vapt");
        public static MvProject LinkPro = new MvProject("LinkPro");

        public static implicit operator string(MvProject current)
        {
            return current.ProjectName;
        }

        public override string ToString()
        {
            return ProjectName;
        }

        public static MvProject Parse(string client)
        {
            client = client?.ToLower();
            SetupClinets();

            if (string.IsNullOrWhiteSpace(client) == true)
                client = string.Empty;

            Func<MvProject> foundClinet = null;

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
                var properties = typeof(MvProject).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.FieldType == typeof(MvProject));
                var projects = properties.Select(x => x.GetValue(null) as MvProject).Where(x => x != NoProject).ToList();

                foreach (var project in projects)
                {
                    MarketVisionProjects.Add(project.ProjectName.ToLower(), () => project);
                }
            }
        }
    }
}
