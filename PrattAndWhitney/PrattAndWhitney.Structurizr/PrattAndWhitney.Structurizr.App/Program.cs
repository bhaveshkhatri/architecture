using Structurizr;
using Structurizr.Api;
using System.Configuration;

namespace PrattAndWhitney.Structurizr.App
{
    public partial class Program
    {
        private static long WorkspaceId;
        private static string ApiKey;
        private static string ApiSecret;

        public static void Main(string[] args)
        {
            WorkspaceId = long.Parse(ConfigurationManager.AppSettings["WorkspaceId"]);
            ApiKey = ConfigurationManager.AppSettings["ApiKey"];
            ApiSecret = ConfigurationManager.AppSettings["ApiSecret"];

            var workspace = new Workspace("Pratt & Whitney", "Model of the Wing To Cash System.");

            Build(workspace);

            Upload(workspace);
        }

        private static void Build(Workspace workspace)
        {
            var model = workspace.Model;

            // Enterprise

            var enterprise = model.Enterprise = new Enterprise("Pratt & Whitney");

            // Users
            var invoiceAnalyst = model.AddPerson(Location.Internal, "Invoice Analyst", "TBD.");
            var invoiceManager = model.AddPerson(Location.Internal, "Invoice Manager", "TBD.");
            var fleetManager = model.AddPerson(Location.Internal, "Fleet Manager", "TBD.");
            var financeUser= model.AddPerson(Location.Internal, "Finance User", "TBD.");
            var costsUser = model.AddPerson(Location.Internal, "Costs User", "TBD.");
            var toolSupport = model.AddPerson(Location.Internal, "Tool Support", "TBD.");
            var powerPlantEngineer = model.AddPerson(Location.External, "Power Plant Engineer", "TBD.");
            var onSiteManager = model.AddPerson(Location.External, "On-Site Manager", "TBD.");
            var shopUser = model.AddPerson(Location.External, "Shop User", "TBD.");

            // Target Software Systems

            var invoiceTransactionsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ITS", "Invoice Transaction System.");
            invoiceTransactionsSoftwareSystem.AddTags(AdditionalTags.TargetSystem);
            invoiceAnalyst.Uses(invoiceTransactionsSoftwareSystem, "Uses");
            invoiceManager.Uses(invoiceTransactionsSoftwareSystem, "Uses");
            toolSupport.Uses(invoiceTransactionsSoftwareSystem, "Uses");
            shopUser.Uses(invoiceTransactionsSoftwareSystem, "Uses");

            var infrastructureServicesSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Infrastructure Services", "Message Broker / Cache / Notification Hub.");
            infrastructureServicesSoftwareSystem.AddTags(AdditionalTags.InfrastructureServices);
            infrastructureServicesSoftwareSystem.AddTags(AdditionalTags.Subsystem);

            // Consumers/Downstream Software Systems

            var costManagementMetricsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", "TBD.");
            costManagementMetricsSoftwareSystem.AddProperty(Properties.KeyContact, "Dafina Georgievska/Matt Wentworth");
            costManagementMetricsSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var spidrsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "TBD - Financial Accounting.");
            spidrsSoftwareSystem.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
            spidrsSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var quoteErrorToolSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "This is temporary");
            quoteErrorToolSoftwareSystem.AddProperty(Properties.KeyContact, "Kim Rose");
            quoteErrorToolSoftwareSystem.AddTags(AdditionalTags.SunsetPhaseOut);
            quoteErrorToolSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var teradataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Teradata", "TBD.");
            teradataSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            teradataSoftwareSystem.AddTags(AdditionalTags.SunsetPhaseOut);
            teradataSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var w2CDownstreamSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "W2C", "TBD");
            w2CDownstreamSoftwareSystem.AddProperty(Properties.KeyContact, "Chandra Kankanala");
            w2CDownstreamSoftwareSystem.AddTags(AdditionalTags.FutureState);
            w2CDownstreamSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var allocationReportSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", "TBD.");
            allocationReportSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            allocationReportSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            var fleetManagementDashboardSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", "TBD.");
            fleetManagementDashboardSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            fleetManagementDashboardSoftwareSystem.Uses(invoiceTransactionsSoftwareSystem, "TBD");

            // Producers/Upstream Software Systems

            var activeDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Active Directory");
            invoiceTransactionsSoftwareSystem.Uses(activeDirectory, "LDAP");

            var eagleDataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Eagle Data", "TBD.");
            eagleDataSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(eagleDataSoftwareSystem, "TBD");

            var workScopingSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", "TBD.");
            workScopingSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            workScopingSoftwareSystem.AddTags(AdditionalTags.FutureState);
            invoiceTransactionsSoftwareSystem.Uses(workScopingSoftwareSystem, "TBD");

            var odinSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
            odinSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(odinSoftwareSystem, "TBD - Uses Access DB");

            var sapSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
            sapSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(sapSoftwareSystem, "TBD");

            var speidSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information. ");
            speidSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(speidSoftwareSystem, "TBD");

            var fleetcareSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices");
            fleetcareSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(fleetcareSoftwareSystem, "TBD");

            var sharePointSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SharePoint", "TBD.");
            sharePointSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(sharePointSoftwareSystem, "TBD");

            var fileSystemSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Filesystem", "TBD.");
            fileSystemSoftwareSystem.AddTags(AdditionalTags.Subsystem);
            fileSystemSoftwareSystem.AddTags(AdditionalTags.Files);
            fileSystemSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionsSoftwareSystem.Uses(fileSystemSoftwareSystem, "OS/NAS");

            // Other Software Systems

            var asoSoftwareSystem = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
            fleetManagementDashboardSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");

            // Infrastructure Services Containers
            
            var messageBrokerContainer = infrastructureServicesSoftwareSystem.AddContainer("Message Broker", "The Invoice Transactions System Message Broker.", "TBD");
            messageBrokerContainer.AddTags(AdditionalTags.MessageBroker);

            var dataCacheContainer = infrastructureServicesSoftwareSystem.AddContainer("Data Cache", "The Invoice Transactions System  Data Cache.", "TBD");
            dataCacheContainer.AddTags(AdditionalTags.Cache);

            // Invoice Transactions System Containers

            var webClientContainer = invoiceTransactionsSoftwareSystem.AddContainer("Web Client", "The Invoice Transactions System Web Client.", "TBD");
            invoiceAnalyst.Uses(webClientContainer, "Uses", "Web Browser");
            invoiceManager.Uses(webClientContainer, "Uses", "Web Browser");
            toolSupport.Uses(webClientContainer, "Uses", "Web Browser");
            shopUser.Uses(webClientContainer, "Uses", "Web Browser");

            var webBackendContainer = invoiceTransactionsSoftwareSystem.AddContainer("Web Backend", "The Invoice Transactions System Web Backend.", "TBD");
            webClientContainer.Uses(webBackendContainer, "Load", "HTTPS");
            
            var apiGatewayServiceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("API Gateway Service", "The Invoice Transactions System API Gateway Service.", "TBD");
            apiGatewayServiceContainer.AddTags(AdditionalTags.Gateway);
            costManagementMetricsSoftwareSystem.Uses(apiGatewayServiceContainer, "Uses", "HTTPS");
            spidrsSoftwareSystem.Uses(apiGatewayServiceContainer, "Uses", "HTTPS");
            fleetManagementDashboardSoftwareSystem.Uses(apiGatewayServiceContainer, "Uses", "HTTPS");
            webClientContainer.Uses(apiGatewayServiceContainer, "Uses", "HTTPS");
            webBackendContainer.Uses(apiGatewayServiceContainer, "Uses", "HTTPS");

            var dataServiceContainer = invoiceTransactionsSoftwareSystem.AddContainer("Data Service", "The Invoice Transactions System Data Service.", "TBD");
            dataServiceContainer.Uses(sharePointSoftwareSystem, "Uses", "TBD");
            dataServiceContainer.Uses(fileSystemSoftwareSystem, "Uses", "OS/NAS");
            infrastructureServicesSoftwareSystem.Uses(dataServiceContainer, "Uses", "TBD");

            // Microservice Containers

            var workflowMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Workflow Microservice", "The Invoice Transactions System Workflow Service.");
            var loadInvoiceMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Load Invoice Microservice", "The Invoice Transactions System Load Invoice Service.");
            var exportMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Export Microservice", "The Invoice Transactions System Export Service.");
            var emailMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Email Microservice", "The Invoice Transactions System Email Service.");
            var membershipMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Membership Microservice", "The Invoice Transactions System Membership Service.");
            membershipMicroserviceContainer.Uses(activeDirectory, "Uses", "TBD");
            var translationMicroserviceContainer = invoiceTransactionsSoftwareSystem.AddMicroserviceContainer("Translation Microservice", "TBD.");

            // Components

            var webClientAdminViewComponent = webClientContainer.AddComponent("Admin View", "Used by administrators of the Invoice Transaction System.", "TBD");
            invoiceAnalyst.Uses(webClientAdminViewComponent, "Uses", "TBD.");

            var webClientAdminControllerComponent = webClientContainer.AddComponent("Admin Controller", "Used by administrators of the Invoice Transaction System.", "TBD");
            webClientAdminViewComponent.Uses(webClientAdminControllerComponent, "Respond to actions and manage state");

            // Views 

            var views = workspace.Views;

            // Context Views

            views.CreateEnterpriseContextLandscapeViewFor(enterprise, PaperSize.A3_Landscape);

            views.CreateSystemContextViewFor(invoiceTransactionsSoftwareSystem, PaperSize.A3_Landscape);

            // Container Views

            views.CreateContainerViewFor(invoiceTransactionsSoftwareSystem, PaperSize.A3_Landscape);

            views.CreateContainerViewFor(infrastructureServicesSoftwareSystem, PaperSize.A3_Landscape);

            // Component Views

            views.CreateComponentViewFor(webClientContainer);

            // Styles

            views.ConfigureStyles();
        }

        private static void Upload(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}