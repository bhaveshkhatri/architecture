using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Deployments
    {
        public static void Configure(Model model)
        {
            model.AddDeploymentNode("ITS Client Desktop", "The desktop environment of an ITS user", "TBD - Windows 7")
                .AddDeploymentNode("Desktop Browser", "Client Desktop Browser", "TBD - Internet Explorer")
                .AddDeploymentNode("ITS SPA", "ITS Single Page Application", "TBD - Angular")
                .Add(Containers.TargetSystem.WebClient);

            model.AddDeploymentNode("ITS Client Mobile", "The mobile environment of an ITS user", "TBD - iPhone 6+ iOS 10+")
                .AddDeploymentNode("Mobile Browser", "Client Mobile Browser", "TBD - Safari")
                .AddDeploymentNode("ITS SPA", "ITS Single Page Application", "TBD - Angular")
                .Add(Containers.TargetSystem.WebClient);

            var datacenter = model.AddDeploymentNode("Pratt & Whitney Datacenter", "Pratt & Whitney Datacenter", "TBD - Azure");

            datacenter.AddDeploymentNode("WebServer***", "A web server VM residing in Azure.", "Windows Server 2016", 2)
                .AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.WebApplication);

            datacenter.AddDeploymentNode("APIServer***", "An API server VM residing in Azure.", "Windows Server 2016", 2)
                .AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.ApiGatewayService);
        }
    }
}