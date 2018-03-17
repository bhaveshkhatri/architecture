using Structurizr;
using Structurizr.Api;
using System.Configuration;
using System.Linq;

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
            var shopUser = model.AddPerson(Location.External, "Shop User", "TBD.");

            // Software Systems

            var invoiceTransactionSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ITS", "Invoice Transaction System.");
            invoiceTransactionSoftwareSystem.AddTags(AdditionalTags.TargetSystem);
            invoiceAnalyst.Uses(invoiceTransactionSoftwareSystem, "Uses");

            var infrastructureServicesSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Infrastructure Services", "Message Broker / Cache / Notification Hub.");
            infrastructureServicesSoftwareSystem.AddTags(AdditionalTags.InfrastructureServices);

            var eagleDataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Eagle Data", "TBD.");
            eagleDataSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(eagleDataSoftwareSystem, "TBD");

            var workScopingSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", "TBD.");
            workScopingSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            workScopingSoftwareSystem.AddTags(AdditionalTags.FutureState);
            invoiceTransactionSoftwareSystem.Uses(workScopingSoftwareSystem, "TBD");

            var odinSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
            odinSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(odinSoftwareSystem, "TBD - Uses Access DB");

            var sapSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
            sapSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(sapSoftwareSystem, "TBD");

            var speidSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information. ");
            speidSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(speidSoftwareSystem, "TBD");

            var fleetcareSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices");
            fleetcareSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(fleetcareSoftwareSystem, "TBD");

            var sharePointSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SharePoint", "TBD.");
            sharePointSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            invoiceTransactionSoftwareSystem.Uses(sharePointSoftwareSystem, "TBD");



            var costManagementMetricsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", "TBD.");
            costManagementMetricsSoftwareSystem.AddProperty(Properties.KeyContact, "Dafina Georgievska/Matt Wentworth");
            costManagementMetricsSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var spidrsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "TBD - Financial Accounting.");
            spidrsSoftwareSystem.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
            spidrsSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var quoteErrorToolSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "This is temporary");
            quoteErrorToolSoftwareSystem.AddProperty(Properties.KeyContact, "Kim Rose");
            quoteErrorToolSoftwareSystem.AddTags(AdditionalTags.SunsetPhaseOut);
            quoteErrorToolSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var teradataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Teradata", "TBD.");
            teradataSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            teradataSoftwareSystem.AddTags(AdditionalTags.SunsetPhaseOut);
            teradataSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var w2CDownstreamSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "W2C", "TBD");
            w2CDownstreamSoftwareSystem.AddProperty(Properties.KeyContact, "Chandra Kankanala");
            w2CDownstreamSoftwareSystem.AddTags(AdditionalTags.FutureState);
            w2CDownstreamSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var allocationReportSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", "TBD.");
            allocationReportSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            allocationReportSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var fleetManagementDashboardSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", "TBD.");
            fleetManagementDashboardSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");
            fleetManagementDashboardSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");


            var asoSoftwareSystem = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
            fleetManagementDashboardSoftwareSystem.AddProperty(Properties.KeyContact, "TBD");

            // Containers


            var messageBrokerContainer = infrastructureServicesSoftwareSystem.AddContainer("Message Broker", "The Invoice Transactions System Message Broker.", "TBD");
            messageBrokerContainer.AddTags(AdditionalTags.MessageBroker);

            var dataCacheContainer = infrastructureServicesSoftwareSystem.AddContainer("Data Cache", "The Invoice Transactions System  Data Cache.", "TBD");
            dataCacheContainer.AddTags(AdditionalTags.Cache);


            var webClientContainer = invoiceTransactionSoftwareSystem.AddContainer("Web Client", "The Invoice Transactions System Web Client.", "TBD");
            invoiceAnalyst.Uses(webClientContainer, "Uses", "Web Browser");

            var webBackendContainer = invoiceTransactionSoftwareSystem.AddContainer("Web Backend", "The Invoice Transactions System Web Backend.", "TBD");
            webClientContainer.Uses(webBackendContainer, "Load", "HTTPS");
            invoiceAnalyst.Uses(webBackendContainer, "Uses", "HTTPS");

            var apiServiceContainer = invoiceTransactionSoftwareSystem.AddMicroserviceContainer("API Service", "The Invoice Transactions System API Service.", "TBD");
            webClientContainer.Uses(apiServiceContainer, "Uses", "HTTPS");
            webBackendContainer.Uses(apiServiceContainer, "Uses", "HTTPS");
            
            var workflowMicroserviceContainer = invoiceTransactionSoftwareSystem.AddMicroserviceContainer("Workflow Microservice", "The Invoice Transactions System Workflow Service.");
            var loadInvoiceMicroserviceContainer = invoiceTransactionSoftwareSystem.AddMicroserviceContainer("Load Invoice Microservice", "The Invoice Transactions System Load Invoice Service.");
            var exportMicroserviceContainer = invoiceTransactionSoftwareSystem.AddMicroserviceContainer("Export Microservice", "The Invoice Transactions System Export Service.");

            var dataServiceContainer = invoiceTransactionSoftwareSystem.AddContainer("Data Service", "The Invoice Transactions System Data Service.", "TBD");
            infrastructureServicesSoftwareSystem.Uses(dataServiceContainer, "Uses", "TBD");

            var operationalDatabaseContainer = invoiceTransactionSoftwareSystem.AddContainer("Operational Database", "The Invoice Transactions System Operational Database.", "TBD");
            operationalDatabaseContainer.AddTags(AdditionalTags.Database);
            dataServiceContainer.Uses(operationalDatabaseContainer, "TBD");

            // Components

            var webClientAdminViewComponent = webClientContainer.AddComponent("Admin View", "Used by administrators of the Invoice Transaction System.", "TBD");
            invoiceAnalyst.Uses(webClientAdminViewComponent, "Uses", "TBD.");

            var webClientAdminControllerComponent = webClientContainer.AddComponent("Admin Controller", "Used by administrators of the Invoice Transaction System.", "TBD");
            webClientAdminViewComponent.Uses(webClientAdminControllerComponent, "Respond to actions and manage state");

            // Views 

            var views = workspace.Views;

            // Context Views

            views.CreateEnterpriseContextLandscapeViewFor(enterprise, PaperSize.A3_Landscape);

            views.CreateSystemContextViewFor(invoiceTransactionSoftwareSystem, PaperSize.A3_Landscape);

            // Container Views

            views.CreateContainerViewFor(invoiceTransactionSoftwareSystem, PaperSize.A3_Landscape);

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
