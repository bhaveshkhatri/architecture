using PrattAndWhitney.Structurizr.App.Extensions;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Components
    {
        public static void Configure()
        {
            WebApplication.Configure();
            WebClient.Configure();
            ApiService.Configure();
        }

        public static class WebApplication
        {
            public static void Configure()
            {
            }
        }

        public static class WebClient
        {
            public static void Configure()
            {
                var admin = Containers.TargetSystem.WebClient.AddComponent("Admin Component", "Used by administrators of ITS.", "Angular Module/Component");
                admin.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.InvoiceManager.Uses(admin, "Uses");
                Users.ToolSupport.Uses(admin, "Uses");

                var dashboard = Containers.TargetSystem.WebClient.AddComponent("Dashboard Component", "Used by invoice team members.", "Angular Module/Component");
                dashboard.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.InvoiceTeam.Uses(dashboard, "Uses");

                var customerContractComponent = Containers.TargetSystem.WebClient.AddComponent("Customer Contract Component", "Used to manage customer contracts in ITS.", "Angular Module/Component");
                customerContractComponent.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.InvoiceTeam.Uses(customerContractComponent, "Uses");
                Users.FleetManager.Uses(customerContractComponent, "Uses");

                var invoiceUpload = Containers.TargetSystem.WebClient.AddComponent("Invoice Upload Component", "Used to upload invoices to ITS.", "Angular Module/Component");
                invoiceUpload.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.ShopUser.Uses(invoiceUpload, "Uses");

                var login = Containers.TargetSystem.WebClient.AddComponent("Login Component", "Used by all users.", "Angular Module/Component");

                var securityComponent = Containers.TargetSystem.WebClient.AddComponent("Security Component", "Authentication token client.", "Angular Module/Component");
                securityComponent.Uses(Containers.TargetSystem.ApiService, "Authenticate", "HTTPS");
                login.Uses(securityComponent, "Uses");

                var routeGuards = Containers.TargetSystem.WebClient.AddComponent("Route Guards", "Enable or disable access to modules/routes based on permissions.", "Angular Route Configuration");
                routeGuards.Uses(securityComponent, "Uses");

                var notificationHubProxy = Containers.TargetSystem.WebClient.AddComponent("Notification Hub Proxy", "Receives and processes system notifications.", "SignalR Hub Proxy");
                notificationHubProxy.Uses(Containers.TargetSystem.ApiService, "Connects to hub", "WebSockets");
            }
        }

        public static class ApiService
        {
            public static void Configure()
            {
                var invoiceApi = Containers.TargetSystem.ApiService.AddApiComponent("Invoice API", "Invoice related API.");
                var contractApi = Containers.TargetSystem.ApiService.AddApiComponent("Contract API", "Contract related API.");
                var securityApi = Containers.TargetSystem.ApiService.AddApiComponent("Security API", "Authentication and authorization related API.");
                var notificationHub = Containers.TargetSystem.ApiService.AddComponent("Notification Hub", "Routes notifications to clients.", "SignalR Hub");
                var securityComponent = Containers.TargetSystem.ApiService.AddComponent("Security Component", "Security component.", "TBD");
                contractApi.Uses(securityComponent, "Check user acccess and permissions.");
                invoiceApi.Uses(securityComponent, "Check user acccess and permissions.");
                securityApi.Uses(securityComponent, "Check user acccess and permissions.");
                var communicationInterface = Containers.TargetSystem.ApiService.AddComponent("Infrastructure Communication", "Sends and receives messages from Infrastructure Services.", "TBD");
                communicationInterface.Uses(notificationHub, "Notify of system response.");
                communicationInterface.Uses(SoftwareSystems.Target.InfrastructureServices, "Send request messages and handle response messages.");
                contractApi.Uses(communicationInterface, "Send commands.");
                invoiceApi.Uses(communicationInterface, "Send commands.");
                securityComponent.Uses(communicationInterface, "Authentication and authorization");
            }
        }
    }
}