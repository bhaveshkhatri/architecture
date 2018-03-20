using PrattAndWhitney.Structurizr.App.ModelConfiguration;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App
{
    public static class WorkspaceBuilder
    {
        public static Workspace Build()
        {
            var workspace = new Workspace("Pratt & Whitney", "Model of the Invoice Transaction System.");
            
            Enterprises.Configure(workspace.Model);

            Users.Configure(workspace.Model);

            SoftwareSystems.Configure(workspace.Model);

            Containers.Configure();

            Components.Configure();

            Views.Configure(workspace);

            return workspace;
        }
    }
}