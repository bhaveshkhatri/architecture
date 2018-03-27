using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Deployments
    {
        public static void Configure(Model model)
        {
            model.AddDeploymentNode("ITS Client Desktop", "The desktop environment of an ITS user", "TBD - Windows 7")
                .AddDeploymentNode("Desktop Browser", "Client Desktop Browser", "TBD - Internet Explorer")
                .Add(Containers.TargetSystem.WebClient);

            model.AddDeploymentNode("ITS Client Mobile", "The mobile environment of an ITS user", "TBD - iPhone 6+ iOS 10+")
                .AddDeploymentNode("Mobile Browser", "Client Mobile Browser", "TBD - Safari")
                .Add(Containers.TargetSystem.WebClient);

            var datacenter = model.AddDeploymentNode("Pratt & Whitney Datacenter", "Pratt & Whitney Datacenter", "TBD - Azure");

            datacenter.AddDeploymentNode("WebServer***", "A web server VM residing in Azure.", "Windows Server 2016", 2)
                .AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.WebApplication);

            datacenter.AddDeploymentNode("APIServer***", "An API server VM residing in Azure.", "Windows Server 2016", 2)
                .AddDeploymentNode("IIS", "A web server from Microsoft.", "IIS 10.0", 1)
                .Add(Containers.TargetSystem.ApiService);

            var serviceHost = datacenter.AddDeploymentNode("MicroserviceHost***", "A server VM residing in Azure.", "Windows Server 2016", 2);
            serviceHost.AddDeploymentNode("WorkflowContainer***", "Service instance container.", "TBD-Docker", 2)
                .Add(Containers.Microservices.Workflow);
            serviceHost.AddDeploymentNode("LoadInvoiceContainer***", "Service instance container.", "TBD-Docker", 2)
                .Add(Containers.Microservices.LoadDocument);

            var dataCacheMaster = datacenter.AddDeploymentNode("DataCacheMaster", "A caching server VM residing in Azure.", "Windows Server 2016", 1)
                .Add(Containers.Infrastructure.DataCacheMaster);

            datacenter.AddDeploymentNode("DataCacheSlave***", "A caching server VM residing in Azure.", "Windows Server 2016", 2)
                .Add(Containers.Infrastructure.DataCacheSlave);
        }
    }
}