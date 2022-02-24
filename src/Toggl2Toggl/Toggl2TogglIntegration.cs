using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Toggl;
using Toggl.Interfaces;
using Toggl.QueryObjects;
using Toggl.Services;

namespace Toggl2Toggl
{
    public class Toggl2TogglIntegration
    {
        readonly ISet<ITimeEntryBasedResolver<IClient>> clientNameResolvers;

        readonly ISet<ITimeEntryBasedResolver<IProject>> projectNameResolvers;

        readonly ISet<ITimeEntryBasedResolver<ITag>> tagResolvers;

        readonly ITimeEntryService timeEntryService;

        readonly List<WorkspaceClientProjectModel> workspacesClientsProjects;

        const int padding = 20;

        public Toggl2TogglIntegration(string toggleApiKey)
        {
            timeEntryService = new TimeEntryService(toggleApiKey);
            var workspaceService = new WorkspaceService(toggleApiKey);
            var clientService = new ClientService(toggleApiKey);
            var projectService = new ProjectService(toggleApiKey);

            clientNameResolvers = new HashSet<ITimeEntryBasedResolver<IClient>>();
            projectNameResolvers = new HashSet<ITimeEntryBasedResolver<IProject>>();
            tagResolvers = new HashSet<ITimeEntryBasedResolver<ITag>>();

            List<Client> clients = clientService.List(false);
            List<Project> projects = projectService.List();
            List<Workspace> workspaces = workspaceService.List();

            workspacesClientsProjects = new List<WorkspaceClientProjectModel>();
            MapClientsAndProjects(workspaces, clients, projects);
        }

        public void Sync(DateTime from, DateTime to, string fromWorkspaceName, string fromProjectName, string toWorkspaceName)
        {
            if (TryGetWorkspaceId(fromWorkspaceName, out long sourceWorkspaceId) == false) return;
            if (TryGetProjectId(fromProjectName, out long sourceProjectId) == false) return;
            if (TryGetWorkspaceId(toWorkspaceName, out long destinationWorkspaceId) == false) return;
            var sourceEntries = GetEntries(from, to, sourceWorkspaceId, sourceProjectId);
            var destinationEntries = GetEntries(from, to, destinationWorkspaceId);

            // clean up all entries
            Print("Cleaninig old entries");
            if (destinationEntries.Any())
                timeEntryService.Delete(destinationEntries.Select(x => x.Id.Value).Distinct().ToArray());

            Print("Starting actual sync");
            foreach (var entry in sourceEntries)
            {
                var found = workspacesClientsProjects.FirstOrDefault(x => x.WorkspaceId == destinationWorkspaceId
                && x.ClientName.Equals(entry.ClientName, StringComparison.OrdinalIgnoreCase)
                && x.ProjectName.Equals(entry.ProjectName, StringComparison.OrdinalIgnoreCase));

                if (found is null) throw new Exception();

                var timeEntry = new TimeEntry
                {
                    CreatedWith = "toggle2toggle",
                    Description = entry.Description,
                    Duration = entry.RoundedDuration,
                    ProjectId = found.ProjectId,
                    Start = DateTimeOffset.Parse(entry.Start).ToString("yyyy-MM-ddTHH:mm:sszzz"),// "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Stop = DateTimeOffset.Parse(entry.Start).AddSeconds(entry.RoundedDuration.GetValueOrDefault()).ToString("yyyy-MM-ddTHH:mm:sszzz"),// "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddSeconds(entry.RoundedDuration.GetValueOrDefault()).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    IsBillable = false,
                    TagNames = entry.TagNames
                };

                Print(entry);
                timeEntryService.Add(timeEntry);
            }

            PrintTotal(sourceEntries);
            Print("Finished actual sync");
        }

        public void Show(DateTime from, DateTime to, string workspaceName, string fromProjectName = null)
        {
            if (TryGetWorkspaceId(workspaceName, out long sourceWorkspaceId) == false) return;
            if (TryGetProjectId(fromProjectName, out long sourceProjectId) == false) return;
            var entries = GetEntries(from, to, sourceWorkspaceId, sourceProjectId);
            Show(entries);
            PrintTotal(entries);
        }

        public void AddClientResolver(ITimeEntryBasedResolver<IClient> resolver)
        {
            clientNameResolvers.Add(resolver);
        }

        public void AddProjectResolver(ITimeEntryBasedResolver<IProject> resolver)
        {
            projectNameResolvers.Add(resolver);
        }

        public void AddTagResolver(ITimeEntryBasedResolver<ITag> resolver)
        {
            tagResolvers.Add(resolver);
        }

        void Show(List<ExtendedTimeEntry> entries)
        {
            Console.Write("Client".PadRight(padding));
            Console.Write("Project".PadRight(padding));
            Console.Write("Tags".PadRight(padding));
            Console.Write("Duration".PadRight(padding));
            Console.Write("Description");
            Console.WriteLine();

            foreach (var entry in entries)
            {
                Print(entry);
            }
        }

        void Print(ExtendedTimeEntry entry)
        {
            var c = entry.ClientName ?? string.Empty;
            var p = entry.ProjectName ?? string.Empty;
            var t = string.Join(",", entry.TagNames ?? new List<string>());
            var d = TimeSpan.FromSeconds(entry.RoundedDuration.GetValueOrDefault()).ToString();
            Console.WriteLine($"{c.PadRight(padding)}{p.PadRight(padding)}{t.PadRight(padding)}{d.PadRight(padding)}{entry.Description}");
        }

        void PrintTotal(IEnumerable<ExtendedTimeEntry> entries)
        {
            var total = TimeSpan.FromSeconds(entries.Sum(x => x.RoundedDuration.GetValueOrDefault()));
            Print($"Total time: {(int)total.TotalHours}:{total.Minutes.ToString("d2")}:{total.Seconds.ToString("d2")}");
        }

        void Print(string text)
        {
            Console.WriteLine($"=========================== {text} ===========================");
        }

        void MapClientsAndProjects(List<Workspace> workspaces, List<Client> clients, List<Project> projects)
        {
            foreach (var project in projects)
            {
                Client c = clients.First(x => x.Id == project.ClientId);
                Workspace w = workspaces.First(x => x.Id == project.WorkspaceId);

                var xx = new WorkspaceClientProjectModel(w.Id.Value, w.Name, c.Id.Value, c.Name, project.Id.Value, project.Name);
                workspacesClientsProjects.Add(xx);
            }
        }

        List<ExtendedTimeEntry> GetEntries(DateTime from, DateTime to, long workspaceId, long? projectId = null)
        {
            var timeEntryParams = new TimeEntryParams
            {
                StartDate = from,
                EndDate = to,
                WorkspaceId = workspaceId,
                ProjectId = projectId
            };

            List<ExtendedTimeEntry> entries = timeEntryService.List(timeEntryParams)
                .Where(x => x.WorkspaceId.Value == workspaceId)
                .Select(x => new ExtendedTimeEntry(x)).ToList();

            foreach (var entry in entries)
            {
                foreach (var clientNameResolver in clientNameResolvers.OrderByDescending(x => x.Weight))
                {
                    if (entry.IsClientNameDefined == true) break;

                    if (clientNameResolver.TryResolve(entry, out IClient client))
                    {
                        entry.DefineClientName(client.Name);
                    }
                }

                foreach (var projectNameResolver in projectNameResolvers.OrderByDescending(x => x.Weight))
                {
                    if (entry.IsProjectNameDefined == true) break;

                    if (projectNameResolver.TryResolve(entry, out IProject project))
                    {
                        entry.DefineProjectName(project.Name);
                    }
                }

                foreach (var tagResolver in tagResolvers.OrderByDescending(x => x.Weight))
                {
                    // maybe break here in case we already have found tag?
                    if (tagResolver.TryResolve(entry, out ITag tag))
                    {
                        entry.AddTagName(tag.Name);
                    }
                }
            }

            return entries;
        }

        bool TryGetWorkspaceId(string workspaceName, out long id)
        {
            id = 1;
            var found = workspacesClientsProjects.FirstOrDefault(x => x.WorkspaceName.Equals(workspaceName, StringComparison.OrdinalIgnoreCase));

            if (found is null == false)
            {
                id = found.WorkspaceId;
                return true;
            }

            return false;
        }

        bool TryGetProjectId(string projectName, out long id)
        {
            id = 1;
            var found = workspacesClientsProjects.FirstOrDefault(x => x.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (found is null == false)
            {
                id = found.ProjectId;
                return true;
            }

            return false;
        }
    }
}
