using PrattAndWhitney.Structurizr.App.WorkspaceConfiguration;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var workspace = new Workspace("Pratt & Whitney v2", "Model of the Invoice Transaction System.");

            Enterprises.Configure(workspace.Model);

            Users.Configure(workspace.Model);

            SoftwareSystems.Configure(workspace.Model);

            Containers.Configure();

            Components.Configure();

            Deployments.Configure(workspace.Model);

            Views.Configure(workspace);

            WorkspaceUploader.Upload(workspace);
        }
    }
}