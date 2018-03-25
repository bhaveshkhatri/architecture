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

            public static void Configure()
            {
                EventBus = SoftwareSystems.Target.InfrastructureServices.AddContainer("Event Bus", "The Invoice Transactions System Event Bus.", "TBD-RabbitMQ");
                EventBus.AddTags(AdditionalTags.EventBus);

                DataCacheMaster = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Master", "The Invoice Transactions System Data Cache Master.", "TBD-Redis");
                DataCacheMaster.AddTags(AdditionalTags.Cache);

                DataCacheSlave = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Slave", "The Invoice Transactions System Data Cache Slave.", "TBD-Redis");
                DataCacheSlave.AddTags(AdditionalTags.Cache);
                DataCacheMaster.Uses(DataCacheSlave, "Replication");
            }
        }

        public static class TargetSystem
        {
            public static Container WebApplication { get; private set; }
            public static Container WebClient { get; private set; }
            public static Container ApiService { get; private set; }
            public static Container DataManagementService { get; private set; }

            public static void Configure()
            {
                WebApplication = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Application", "Delivers the Invoice Transactions System Web Client Single-Page Application.", "TBD-ASP.NET");
                Users.InvoiceTeam.Uses(WebApplication, "Uses");
                Users.ShopUser.Uses(WebApplication, "Uses");

                WebClient = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Client SPA", "The Invoice Transactions System Web Client Single-Page Application.", "Angular 5");
                Users.InvoiceTeam.Uses(WebClient, "Uses");
                Users.ShopUser.Uses(WebClient, "Uses");
                WebApplication.Uses(WebClient, "Delivers");

                ApiService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("API Service", "The Invoice Transactions System API Service.", "TBD-ASP.NET Web API");
                ApiService.AddTags(AdditionalTags.Gateway);
                SoftwareSystems.Downstream.CostManagementMetrics.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Downstream.Spidrs.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Downstream.FleetManagementDashboard.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Downstream.WingToCashDownstream.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Downstream.AllocationReport.Uses(ApiService, "Uses", "HTTPS");
                WebClient.Uses(ApiService, "Uses", "HTTPS");
                
                var sqlServer = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("SQL Server", "The Invoice Transactions SQL Server.", "TBD");

                DataManagementService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Data Management Service", "The Invoice Transactions Data Management Service.");
                DataManagementService.Uses(SoftwareSystems.Upstream.SharePoint, "Uses", "TBD");
                DataManagementService.Uses(SoftwareSystems.Upstream.FileSystem, "Uses", "OS/NAS");
                DataManagementService.Uses(sqlServer, "Uses", "TBD-OLAP/OLTP");
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
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Central Logging Microservice", "TBD.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("External Systems Microservices", "TBD.").AddTags(AdditionalTags.Multiple);
                var identityMicroservice = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Identity Microservice", "The Invoice Transactions System Identity Service.");
                identityMicroservice.Uses(SoftwareSystems.Upstream.ActiveDirectory, "Uses", "TBD");
            }
        }
    }
}