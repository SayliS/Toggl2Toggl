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

            List<Client> clients = clientService.List(false);
            List<Project> projects = projectService.List();
            List<Workspace> workspaces = workspaceService.List();

            workspacesClientsProjects = new List<WorkspaceClientProjectModel>();
            MapClientsAndProjects(workspaces, clients, projects);
        }

        public void Sync(DateTime from, DateTime to, string fromWorkspaceName, string toWorkspaceName)
        {
            if (TryGetWorkspaceId(fromWorkspaceName, out long sourceWorkspaceId) == false) return;
            if (TryGetWorkspaceId(toWorkspaceName, out long destinationWorkspaceId) == false) return;
            var sourceEntries = GetEntries(from, to, sourceWorkspaceId);
            var destinationEntries = GetEntries(from, to, destinationWorkspaceId);

            Show(sourceEntries);

            // clean up all entries
            foreach (var entry in destinationEntries)
            {
                timeEntryService.Delete(entry.Id.Value);
            }

            Console.WriteLine("Actually entering:");
            foreach (var entry in sourceEntries)
            {
                var found = workspacesClientsProjects.FirstOrDefault(x => x.WorkspaceId == destinationWorkspaceId
                && x.ClientName.Equals(entry.ClientName, StringComparison.OrdinalIgnoreCase)
                && x.ProjectName.Equals(entry.ProjectName, StringComparison.OrdinalIgnoreCase));

                long duration = (long)(Math.Ceiling((double)entry.Duration / 900d) * 900d);

                if (found is null) throw new Exception();

                var timeEntry = new TimeEntry
                {
                    CreatedWith = "toggle2toggle",
                    Description = entry.Description,
                    Duration = duration,
                    ProjectId = found.ProjectId,
                    Start = DateTime.ParseExact(entry.Start, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Stop = DateTime.ParseExact(entry.Start, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).AddSeconds(duration).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    IsBillable = false
                };

                Print(entry);
                timeEntryService.Add(timeEntry);
            }
        }

        public void Show(DateTime from, DateTime to, string workspaceName)
        {
            if (TryGetWorkspaceId(workspaceName, out long sourceWorkspaceId) == false) return;
            var entries = GetEntries(from, to, sourceWorkspaceId);
            Show(entries);
        }

        public void AddClientResolver(ITimeEntryBasedResolver<IClient> resolver)
        {
            clientNameResolvers.Add(resolver);
        }

        public void AddProjectResolver(ITimeEntryBasedResolver<IProject> resolver)
        {
            projectNameResolvers.Add(resolver);
        }

        void Show(List<ExtendedTimeEntry> entries)
        {
            Console.Write("Client".PadRight(padding));
            Console.Write("Project".PadRight(padding));
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
            Console.WriteLine($"{c.PadRight(padding)}{p.PadRight(padding)}{entry.Description}");
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

        List<ExtendedTimeEntry> GetEntries(DateTime from, DateTime to, long workspaceId)
        {
            var timeEntryParams = new TimeEntryParams
            {
                StartDate = from,
                EndDate = to
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
                        entry.DefineClientName(client.ClientName);
                    }
                }

                foreach (var projectNameResolver in projectNameResolvers.OrderByDescending(x => x.Weight))
                {
                    if (entry.IsProjectNameDefined == true) break;

                    if (projectNameResolver.TryResolve(entry, out IProject project))
                    {
                        entry.DefineProjectName(project.ProjectName);
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
    }
}
