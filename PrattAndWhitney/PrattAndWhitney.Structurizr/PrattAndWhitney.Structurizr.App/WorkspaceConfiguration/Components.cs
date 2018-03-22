namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Components
    {
        public static void Configure()
        {
            WebClient.Configure();
            ApiService.Configure();
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
                Users.InvoiceAnalyst.Uses(dashboard, "Uses", "TBD.");
                Users.InvoiceManager.Uses(dashboard, "Uses", "TBD.");

                var login = Containers.TargetSystem.WebClient.AddComponent("Login Component", "Used by all users.", "TBD");
                Users.InvoiceAnalyst.Uses(login, "Uses", "TBD.");
                Users.InvoiceManager.Uses(login, "Uses", "TBD.");
                Users.ToolSupport.Uses(login, "Uses", "TBD.");

                var securityComponent = Containers.TargetSystem.WebClient.AddComponent("Security Component", "Authentication token client.", "TBD");
                securityComponent.Uses(Containers.TargetSystem.ApiService, "Authenticate", "HTTPS");
                admin.Uses(securityComponent, "Uses");
                dashboard.Uses(securityComponent, "Uses");
                login.Uses(securityComponent, "Uses");

                var notificationHubProxy = Containers.TargetSystem.WebClient.AddComponent("Notification Hub Proxy", "Receives and processes system notifications.", "TBD-SignalR Hub Proxy");
                notificationHubProxy.Uses(Containers.TargetSystem.ApiService, "Connects to hub", "WebSockets");
            }
        }

        public static class ApiService
        {
            public static void Configure()
            {
                var notificationHub = Containers.TargetSystem.ApiService.AddComponent("Notification Hub", "Routes notifications to clients.", "TBD-SignalR Hub");
                Containers.TargetSystem.ApiService.AddComponent("Security Component", "Authentication token client.", "TBD");
                var communicationInterface = Containers.TargetSystem.ApiService.AddComponent("Infrastructure Communication", "Sends and receives messages from Infrastructure Services.", "TBD");
                communicationInterface.Uses(notificationHub, "Notify of system response.");
                communicationInterface.Uses(SoftwareSystems.Target.InfrastructureServices, "Send request messages and handle response messages.");
            }
        }
    }
}