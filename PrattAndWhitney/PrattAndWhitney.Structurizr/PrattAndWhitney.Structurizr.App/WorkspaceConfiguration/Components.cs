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
                var admin = Containers.TargetSystem.WebClient.AddComponent("Admin Component", "Used by administrators of the Invoice Transaction System.", "TBD");
                admin.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.InvoiceManager.Uses(admin, "Uses", "TBD.");
                Users.ToolSupport.Uses(admin, "Uses", "TBD.");

                var dashboard = Containers.TargetSystem.WebClient.AddComponent("Dashboard Component", "Used by invoice team members.", "TBD");
                dashboard.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.InvoiceTeam.Uses(dashboard, "Uses", "TBD.");

                var invoiceUpload = Containers.TargetSystem.WebClient.AddComponent("Invoice Upload Component", "Used to upload invoices to the system.", "TBD");
                invoiceUpload.Uses(Containers.TargetSystem.ApiService, "Uses", "HTTPS + Token");
                Users.ShopUser.Uses(invoiceUpload, "Uses", "TBD.");

                var login = Containers.TargetSystem.WebClient.AddComponent("Login Component", "Used by all users.", "TBD");
                Users.InvoiceTeam.Uses(login, "Uses", "TBD.");
                Users.ShopUser.Uses(login, "Uses", "TBD.");

                var securityComponent = Containers.TargetSystem.WebClient.AddComponent("Security Component", "Authentication token client.", "TBD");
                securityComponent.Uses(Containers.TargetSystem.ApiService, "Authenticate", "HTTPS");
                admin.Uses(securityComponent, "Uses");
                dashboard.Uses(securityComponent, "Uses");
                invoiceUpload.Uses(securityComponent, "Uses");
                login.Uses(securityComponent, "Uses");

                var notificationHubProxy = Containers.TargetSystem.WebClient.AddComponent("Notification Hub Proxy", "Receives and processes system notifications.", "TBD-SignalR Hub Proxy");
                notificationHubProxy.Uses(Containers.TargetSystem.ApiService, "Connects to hub", "WebSockets");

                //TODO
                //var moduleLoader = Containers.TargetSystem.WebClient.AddComponent("Module Loader", "Loads the modules available to the user.", "TBD");
                //moduleLoader.Uses(Containers.TargetSystem.WebApplication, "Load available modules.");
            }
        }

        public static class ApiService
        {
            public static void Configure()
            {
                var invoiceApi = Containers.TargetSystem.ApiService.AddApiComponent("Invoice API", "Invoice related API.");
                var securityApi = Containers.TargetSystem.ApiService.AddApiComponent("Security API", "Authentication and authorization related API.");
                var notificationHub = Containers.TargetSystem.ApiService.AddComponent("Notification Hub", "Routes notifications to clients.", "TBD-SignalR Hub");
                var securityComponent = Containers.TargetSystem.ApiService.AddComponent("Security Component", "Security component.", "TBD");
                invoiceApi.Uses(securityComponent, "Check user acccess and permissions.");
                securityApi.Uses(securityComponent, "Check user acccess and permissions.");
                var communicationInterface = Containers.TargetSystem.ApiService.AddComponent("Infrastructure Communication", "Sends and receives messages from Infrastructure Services.", "TBD");
                communicationInterface.Uses(notificationHub, "Notify of system response.");
                communicationInterface.Uses(SoftwareSystems.Target.InfrastructureServices, "Send request messages and handle response messages.");
                invoiceApi.Uses(communicationInterface, "Send commands.");
                securityComponent.Uses(communicationInterface, "Authentication and authorization");
            }
        }
    }
}