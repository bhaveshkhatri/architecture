using PrattAndWhitney.Structurizr.App.Extensions;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Containers
    {
        public static void Configure()
        {
            Infrastructure.Configure();
            TargetSystem.Configure();
            Microservices.Configure();
        }

        public static class Infrastructure
        {
            public static void Configure()
            {
                var messageBroker = SoftwareSystems.Target.InfrastructureServices.AddContainer("Message Broker", "The Invoice Transactions System Message Broker.", "TBD");
                messageBroker.AddTags(AdditionalTags.MessageBroker);

                var dataCache = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache", "The Invoice Transactions System  Data Cache.", "TBD-Redis");
                dataCache.AddTags(AdditionalTags.Cache);

                var notificationHub = SoftwareSystems.Target.InfrastructureServices.AddContainer("Notification Hub", "The Invoice Transactions System Notification Hub.", "TBD-SignalR");
                notificationHub.AddTags(AdditionalTags.Hub);
            }
        }

        public static class TargetSystem
        {
            public static Container WebApplication { get; private set; }
            public static Container WebClient { get; private set; }
            public static Container ApiGatewayService { get; private set; }
            public static Container FileManagementService { get; private set; }

            public static void Configure()
            {
                WebApplication = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Application", "Delivers the Invoice Transactions System Web Client Single-Page Application.", "TBD");
                Users.InvoiceAnalyst.Uses(WebApplication, "Uses");
                Users.InvoiceManager.Uses(WebApplication, "Uses");
                Users.ToolSupport.Uses(WebApplication, "Uses");
                Users.ShopUser.Uses(WebApplication, "Uses");

                WebClient = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Client SPA", "The Invoice Transactions System Web Client Single-Page Application.", "TBD");
                Users.InvoiceAnalyst.Uses(WebClient, "Uses");
                Users.InvoiceManager.Uses(WebClient, "Uses");
                Users.ToolSupport.Uses(WebClient, "Uses");
                Users.ShopUser.Uses(WebClient, "Uses");
                WebApplication.Uses(WebClient, "Delivers");

                ApiGatewayService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("API Gateway Service", "The Invoice Transactions System API Gateway Service.", "TBD");
                ApiGatewayService.AddTags(AdditionalTags.Gateway);
                SoftwareSystems.Other.CostManagementMetrics.Uses(ApiGatewayService, "Uses", "HTTPS");
                SoftwareSystems.Other.Spidrs.Uses(ApiGatewayService, "Uses", "HTTPS");
                SoftwareSystems.Other.FleetManagementDashboard.Uses(ApiGatewayService, "Uses", "HTTPS");
                WebClient.Uses(ApiGatewayService, "Uses", "HTTPS");

                FileManagementService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("File Management Service", "The Invoice Transactions File Management Service.", "TBD");
                FileManagementService.Uses(SoftwareSystems.Other.SharePoint, "Uses", "TBD");
                FileManagementService.Uses(SoftwareSystems.Other.FileSystem, "Uses", "OS/NAS");
                SoftwareSystems.Target.InfrastructureServices.Uses(FileManagementService, "Uses", "TBD");
            }
        }

        public static class Microservices
        {
            public static void Configure()
            {
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Workflow Microservice", "The Invoice Transactions System Workflow Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Load Invoice Microservice", "The Invoice Transactions System Load Invoice Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Export Microservice", "The Invoice Transactions System Export Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Email Microservice", "The Invoice Transactions System Email Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Translation Microservice", "TBD.");
                var identityMicroservice = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Identity Microservice", "The Invoice Transactions System Identity Service.");
                identityMicroservice.Uses(SoftwareSystems.Other.ActiveDirectory, "Uses", "TBD");
            }
        }
    }
}