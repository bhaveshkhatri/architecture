using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Deployments
    {
        public static void Configure(Model model)
        {
            var webApplicationServer = model.AddDeploymentNode("WebServer***", "A web server VM residing in Azure.", "Windows Server 2016", 2);
            webApplicationServer.AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.WebApplication);

            var apiServer = model.AddDeploymentNode("APIServer***", "An API server VM residing in Azure.", "Windows Server 2016", 2);
            apiServer.AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.ApiGatewayService);

            //TODO
            //var dataCenter = model.AddDeploymentNode("Data Center", "A Pratt & Whitney data center", "TBD");
            //dataCenter.Children.Add(webApplicationServer);
            //dataCenter.Children.Add(apiServer);
        }
    }
}