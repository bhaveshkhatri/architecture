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
            public static Container EventBus { get; private set; }
            public static Container DataCacheMaster { get; private set; }
            public static Container DataCacheSlave { get; private set; }
            public static Container NotificationHub { get; private set; }

            public static void Configure()
            {
                EventBus = SoftwareSystems.Target.InfrastructureServices.AddContainer("Event Bus", "The Invoice Transactions System Event Bus.", "TBD-RabbitMQ");
                EventBus.AddTags(AdditionalTags.EventBus);

                DataCacheMaster = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Master", "The Invoice Transactions System Data Cache Master.", "TBD-Redis");
                DataCacheMaster.AddTags(AdditionalTags.Cache);

                DataCacheSlave = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Slave", "The Invoice Transactions System Data Cache Slave.", "TBD-Redis");
                DataCacheSlave.AddTags(AdditionalTags.Cache);
                DataCacheMaster.Uses(DataCacheSlave, "Replication");

                NotificationHub = SoftwareSystems.Target.InfrastructureServices.AddContainer("Notification Hub", "The ITS Notification Hub.", "TBD");
                NotificationHub.AddTags(AdditionalTags.Hub);
            }
        }

        public static class TargetSystem
        {
            public static Container WebApplication { get; private set; }
            public static Container WebClient { get; private set; }
            public static Container ApiService { get; private set; }
            public static Container FileManagementService { get; private set; }

            public static void Configure()
            {
                WebApplication = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Application", "Delivers the Invoice Transactions System Web Client Single-Page Application.", "TBD");
                Users.InvoiceAnalyst.Uses(WebApplication, "Uses");
                Users.InvoiceManager.Uses(WebApplication, "Uses");
                Users.ToolSupport.Uses(WebApplication, "Uses");
                Users.ShopUser.Uses(WebApplication, "Uses");

                WebClient = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Client SPA", "The Invoice Transactions System Web Client Single-Page Application.", "TBD-Angular");
                Users.InvoiceAnalyst.Uses(WebClient, "Uses");
                Users.InvoiceManager.Uses(WebClient, "Uses");
                Users.ToolSupport.Uses(WebClient, "Uses");
                Users.ShopUser.Uses(WebClient, "Uses");
                WebApplication.Uses(WebClient, "Delivers");

                ApiService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("API Service", "The Invoice Transactions System API Service.", "TBD");
                ApiService.AddTags(AdditionalTags.Gateway);
                SoftwareSystems.Other.CostManagementMetrics.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Other.Spidrs.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Other.FleetManagementDashboard.Uses(ApiService, "Uses", "HTTPS");
                WebClient.Uses(ApiService, "Uses", "HTTPS");

                FileManagementService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("File Management Service", "The Invoice Transactions File Management Service.", "TBD");
                FileManagementService.Uses(SoftwareSystems.Other.SharePoint, "Uses", "TBD");
                FileManagementService.Uses(SoftwareSystems.Other.FileSystem, "Uses", "OS/NAS");
                SoftwareSystems.Target.InfrastructureServices.Uses(FileManagementService, "Uses", "TBD");
            }
        }

        public static class Microservices
        {
            public static Container Workflow { get; private set; }
            public static Container LoadInvoice { get; private set; }

            public static void Configure()
            {
                Workflow = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Workflow Microservice", "The Invoice Transactions System Workflow Service.");
                LoadInvoice = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Load Invoice Microservice", "The Invoice Transactions System Load Invoice Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Export Microservice", "The Invoice Transactions System Export Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Email Microservice", "The Invoice Transactions System Email Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Translation Microservice", "TBD.");
                var identityMicroservice = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Identity Microservice", "The Invoice Transactions System Identity Service.");
                identityMicroservice.Uses(SoftwareSystems.Other.ActiveDirectory, "Uses", "TBD");
            }
        }
    }
}