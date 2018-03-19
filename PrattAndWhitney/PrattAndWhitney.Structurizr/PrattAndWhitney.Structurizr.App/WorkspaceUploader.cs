using Structurizr;
using Structurizr.Api;
using System.Configuration;

namespace PrattAndWhitney.Structurizr.App
{
    public static class WorkspaceUploader
    {
        public static void Upload(Workspace workspace)
        {
            var WorkspaceId = long.Parse(ConfigurationManager.AppSettings["WorkspaceId"]);
            var ApiKey = ConfigurationManager.AppSettings["ApiKey"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecret"];

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}