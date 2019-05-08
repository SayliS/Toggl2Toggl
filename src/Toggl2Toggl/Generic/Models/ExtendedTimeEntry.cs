using System.Collections.Generic;
using Toggl;

namespace Toggl2Toggl
{
    public class ExtendedTimeEntry : TimeEntry
    {
        public ExtendedTimeEntry(TimeEntry timeEntry)
        {
            CreatedWith = timeEntry.CreatedWith;
            Description = timeEntry.Description;
            Duration = timeEntry.Duration;
            Id = timeEntry.Id;
            ProjectId = timeEntry.ProjectId;
            ShowDurationOnly = timeEntry.ShowDurationOnly;
            Start = timeEntry.Start;
            Stop = timeEntry.Stop;
            TagNames = timeEntry.TagNames;
            TaskId = timeEntry.TaskId;
            TaskName = timeEntry.TaskName;
            UpdatedOn = timeEntry.UpdatedOn;
            WorkspaceId = timeEntry.WorkspaceId;
        }

        public string ClientName { get; private set; }

        public void DefineClientName(string clientName)
        {
            ClientName = clientName;
        }

        public bool IsClientNameDefined { get { return string.IsNullOrEmpty(ClientName) == false; } }


        public string ProjectName { get; private set; }

        public void DefineProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public void AddTagName(string tagName)
        {
            if (TagNames is null)
                TagNames = new List<string>();

            if (TagNames.Contains(tagName) == false)
                TagNames.Add(tagName);
        }

        public bool IsProjectNameDefined { get { return string.IsNullOrEmpty(ProjectName) == false; } }
    }
}
