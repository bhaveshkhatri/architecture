using Structurizr;

namespace PrattAndWhitney.Structurizr.App
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
                var messageBrokerContainer = SoftwareSystems.Target.InfrastructureServicesSoftwareSystem.AddContainer("Message Broker", "The Invoice Transactions System Message Broker.", "TBD");
                messageBrokerContainer.AddTags(AdditionalTags.MessageBroker);

                var dataCacheContainer = SoftwareSystems.Target.InfrastructureServicesSoftwareSystem.AddContainer("Data Cache", "The Invoice Transactions System  Data Cache.", "TBD");
                dataCacheContainer.AddTags(AdditionalTags.Cache);
            }
        }

        public static class TargetSystem
        {
            public static Container WebApplicationContainer { get; private set; }
            public static Container WebClientSpaContainer { get; private set; }
            public static Container ApiGatewayServiceContainer { get; private set; }
            public static Container DataServiceContainer { get; private set; }

            public static void Configure()
            {
                WebApplicationContainer = SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddContainer("Web Applicatioin", "Delivers the Invoice Transactions System Web Client Single-Page Application.", "TBD");
                Users.InvoiceAnalyst.Uses(WebApplicationContainer, "Uses");
                Users.InvoiceManager.Uses(WebApplicationContainer, "Uses");
                Users.ToolSupport.Uses(WebApplicationContainer, "Uses");
                Users.ShopUser.Uses(WebApplicationContainer, "Uses");

                WebClientSpaContainer = SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddContainer("Web Client SPA", "The Invoice Transactions System Web Client Single-Page Application.", "TBD");
                Users.InvoiceAnalyst.Uses(WebClientSpaContainer, "Uses");
                Users.InvoiceManager.Uses(WebClientSpaContainer, "Uses");
                Users.ToolSupport.Uses(WebClientSpaContainer, "Uses");
                Users.ShopUser.Uses(WebClientSpaContainer, "Uses");
                WebApplicationContainer.Uses(WebClientSpaContainer, "Delivers");

                ApiGatewayServiceContainer = SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("API Gateway Service", "The Invoice Transactions System API Gateway Service.", "TBD");
                ApiGatewayServiceContainer.AddTags(AdditionalTags.Gateway);
                SoftwareSystems.Other.CostManagementMetrics.Uses(ApiGatewayServiceContainer, "Uses", "HTTPS");
                SoftwareSystems.Other.Spidrs.Uses(ApiGatewayServiceContainer, "Uses", "HTTPS");
                SoftwareSystems.Other.FleetManagementDashboard.Uses(ApiGatewayServiceContainer, "Uses", "HTTPS");
                WebClientSpaContainer.Uses(ApiGatewayServiceContainer, "Uses", "HTTPS");

                DataServiceContainer = SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddContainer("Data Service", "The Invoice Transactions System Data Service.", "TBD");
                DataServiceContainer.Uses(SoftwareSystems.Other.SharePoint, "Uses", "TBD");
                DataServiceContainer.Uses(SoftwareSystems.Other.FileSystem, "Uses", "OS/NAS");
                SoftwareSystems.Target.InfrastructureServicesSoftwareSystem.Uses(DataServiceContainer, "Uses", "TBD");
            }
        }

        public static class Microservices
        {
            public static void Configure()
            {
                SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Workflow Microservice", "The Invoice Transactions System Workflow Service.");
                SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Load Invoice Microservice", "The Invoice Transactions System Load Invoice Service.");
                SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Export Microservice", "The Invoice Transactions System Export Service.");
                SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Email Microservice", "The Invoice Transactions System Email Service.");
                var identityMicroserviceContainer = SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Identity Microservice", "The Invoice Transactions System Identity Service.");
                identityMicroserviceContainer.Uses(SoftwareSystems.Other.ActiveDirectory, "Uses", "TBD");
                SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Translation Microservice", "TBD.");
            }
        }
    }
}