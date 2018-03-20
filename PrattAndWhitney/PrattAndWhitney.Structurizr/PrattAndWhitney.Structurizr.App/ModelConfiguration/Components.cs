namespace PrattAndWhitney.Structurizr.App.ModelConfiguration
{
    public static class Components
    {
        public static void Configure()
        {
            WebClient.Configure();
        }

        public static class WebClient
        {
            public static void Configure()
            {
                var webClientAdminComponent = Containers.TargetSystem.WebClientSpaContainer.AddComponent("Admin Component", "Used by administrators of the Invoice Transaction System.", "TBD");
                webClientAdminComponent.Uses(Containers.TargetSystem.ApiGatewayServiceContainer, "Uses", "HTTPS + Token");
                Users.InvoiceManager.Uses(webClientAdminComponent, "Uses", "TBD.");
                Users.ToolSupport.Uses(webClientAdminComponent, "Uses", "TBD.");

                var webClientDashboardComponent = Containers.TargetSystem.WebClientSpaContainer.AddComponent("Dashboard Component", "Used by invoice team members.", "TBD");
                webClientDashboardComponent.Uses(Containers.TargetSystem.ApiGatewayServiceContainer, "Uses", "HTTPS + Token");
                Users.InvoiceAnalyst.Uses(webClientDashboardComponent, "Uses", "TBD.");
                Users.InvoiceManager.Uses(webClientDashboardComponent, "Uses", "TBD.");

                var webClientLoginComponent = Containers.TargetSystem.WebClientSpaContainer.AddComponent("Login Component", "Used by all users.", "TBD");
                Users.InvoiceAnalyst.Uses(webClientLoginComponent, "Uses", "TBD.");
                Users.InvoiceManager.Uses(webClientLoginComponent, "Uses", "TBD.");
                Users.ToolSupport.Uses(webClientLoginComponent, "Uses", "TBD.");

                var webClientSecurityComponent = Containers.TargetSystem.WebClientSpaContainer.AddComponent("Security Component", "Authentication token client.", "TBD");
                webClientSecurityComponent.Uses(Containers.TargetSystem.ApiGatewayServiceContainer, "Authenticate", "HTTPS");
                webClientAdminComponent.Uses(webClientSecurityComponent, "Uses");
                webClientDashboardComponent.Uses(webClientSecurityComponent, "Uses");
                webClientLoginComponent.Uses(webClientSecurityComponent, "Uses");
            }
        }
    }
}