namespace Toggl2Toggl
{
    public class WorkspaceClientProjectModel
    {
        public WorkspaceClientProjectModel(long workspaceId, string workspaceName, long clientId, string clientName, long projectId, string projectName)
        {
            WorkspaceId = workspaceId;
            WorkspaceName = workspaceName;
            ClientId = clientId;
            ClientName = clientName;
            ProjectId = projectId;
            ProjectName = projectName;
        }

        public long WorkspaceId { get; private set; }

        public string WorkspaceName { get; private set; }

        public long ClientId { get; private set; }

        public string ClientName { get; private set; }

        public long ProjectId { get; private set; }

        public string ProjectName { get; private set; }

        public override string ToString()
        {
            return $"W: {WorkspaceName} - C: {ClientName} - P: {ProjectName}";
        }
    }
}