using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Deployments
    {
        public static void Configure(Model model)
        {
            var clientDesktop = model.AddDeploymentNode("ITS Client Desktop", "The desktop environment of an ITS user", "TBD - Windows 7");

            var desktopBrowser = clientDesktop.AddDeploymentNode("Desktop Browser", "Client Desktop Browser", "TBD - Internet Explorer");
            desktopBrowser.AddDeploymentNode("ITS SPA", "ITS Single Page Application", "TBD - Angular").Add(Containers.TargetSystem.WebClient);

            var clientMobileDevice = model.AddDeploymentNode("ITS Client Mobile", "The mobile environment of an ITS user", "TBD - iPhone 6+ iOS 10+");

            var mobileBrowser = clientMobileDevice.AddDeploymentNode("Mobile Browser", "Client Mobile Browser", "TBD - Safari");
            mobileBrowser.AddDeploymentNode("ITS SPA", "ITS Single Page Application", "TBD - Angular").Add(Containers.TargetSystem.WebClient);

            var datacenter = model.AddDeploymentNode("Pratt & Whitney Datacenter", "Pratt & Whitney Datacenter", "TBD - Azure");

            var webApplicationServer = datacenter.AddDeploymentNode("WebServer***", "A web server VM residing in Azure.", "Windows Server 2016", 2);
            webApplicationServer.AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1).Add(Containers.TargetSystem.WebApplication);

            var apiServer = datacenter.AddDeploymentNode("APIServer***", "An API server VM residing in Azure.", "Windows Server 2016", 2);
            apiServer.AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1).Add(Containers.TargetSystem.ApiGatewayService);
        }
    }
}