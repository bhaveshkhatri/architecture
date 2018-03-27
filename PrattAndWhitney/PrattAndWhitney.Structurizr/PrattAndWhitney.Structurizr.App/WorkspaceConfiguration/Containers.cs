using PrattAndWhitney.Structurizr.App.Common;
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
                EventBus = SoftwareSystems.Target.InfrastructureServices.AddContainer("Event Bus", "The ITS Event Bus.", Constants.TechnologyStack.Transport);
                EventBus.AddTags(AdditionalTags.EventBus);

                DataCacheMaster = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Master", "The ITS Data Cache Master.", Constants.TechnologyStack.Transport);
                DataCacheMaster.AddTags(AdditionalTags.Cache);

                DataCacheSlave = SoftwareSystems.Target.InfrastructureServices.AddContainer("Data Cache Slave", "The ITS Data Cache Slave.", Constants.TechnologyStack.Transport);
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
                WebApplication = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Application", "Delivers the ITS Web Client Single-Page Application.", Constants.TechnologyStack.WebAppPlatform);
                Users.InvoiceTeam.Uses(WebApplication, "Uses");
                Users.ShopUser.Uses(WebApplication, "Uses");
                Users.FleetManager.Uses(WebApplication, "Uses");

                WebClient = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("Web Client SPA", "The ITS Web Client Single-Page Application.", Constants.TechnologyStack.FrontendFramework);
                Users.InvoiceTeam.Uses(WebClient, "Uses");
                Users.ShopUser.Uses(WebClient, "Uses");
                Users.FleetManager.Uses(WebClient, "Uses");
                WebApplication.Uses(WebClient, "Delivers");

                ApiService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("API Service", "The ITS API Service.", Constants.TechnologyStack.ApiPlatform);
                ApiService.AddTags(AdditionalTags.Gateway);
                SoftwareSystems.Downstream.FleetManagementDashboard.Uses(ApiService, "Uses", "HTTPS");
                SoftwareSystems.Downstream.WingToCashDownstream.Uses(ApiService, "Uses", "HTTPS");
                WebClient.Uses(ApiService, "Uses", "HTTPS");
                
                var sqlServer = SoftwareSystems.Target.InvoiceTransactionsSystem.AddContainer("SQL Server", "The ITS database server.", Constants.TechnologyStack.DatabaseServer);
                sqlServer.AddTags(AdditionalTags.Database);

                DataManagementService = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Data Management Service", "The Invoice Transactions Data Management Service.");
                DataManagementService.Uses(SoftwareSystems.Upstream.SharePoint, "Uses", " ");
                DataManagementService.Uses(SoftwareSystems.Upstream.FileSystem, "Uses", "OS/NAS");
                DataManagementService.Uses(sqlServer, "Uses", "OLAP/OLTP");
            }
        }

        public static class Microservices
        {
            public static Container Workflow { get; private set; }
            public static Container LoadDocument { get; private set; }

            public static void Configure()
            {
                Workflow = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Workflow Microservice", "The ITS Workflow Service.");
                LoadDocument = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Load Document Microservice", "Loads document files (e.g. Excel invoices, TEEP) into ITS.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Export Microservice", "The ITS Export Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Email Microservice", "The ITS Email Service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Translation Microservice", "Custom function translation service.");
                SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Central Logging Microservice", "Centralizaed logging across ITS.");

                var external = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Upstream Systems Microservices", "Actions and data from upstream systems.");
                external.AddTags(AdditionalTags.Multiple);
                external.Uses(SoftwareSystems.Upstream.EagleData, "Uses");
                external.Uses(SoftwareSystems.Upstream.WorkScoping, "Uses");
                external.Uses(SoftwareSystems.Upstream.Odin, "Uses");
                external.Uses(SoftwareSystems.Upstream.Sap, "Uses");
                external.Uses(SoftwareSystems.Upstream.Fleetcare, "Uses");
                external.Uses(SoftwareSystems.Upstream.Speid, "Uses");

                var identityMicroservice = SoftwareSystems.Target.InvoiceTransactionsSystem.AddMicroserviceContainer("Identity Microservice", "The ITS Identity Service.");
                identityMicroservice.Uses(SoftwareSystems.Upstream.ActiveDirectory, "Uses", "LDAP");
            }
        }
    }
}