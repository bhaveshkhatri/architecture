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
            var prattInvoiceAnalyst = model.AddPerson(Location.Internal, "Invoice Analyst", "TBD.");
            var shopUser = model.AddPerson(Location.External, "Shop User", "TBD.");

            // Software Systems

            var invoiceTransactionSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Invoice Transaction System", "Invoice Transaction System.");
            invoiceTransactionSoftwareSystem.AddTags(AdditionalTags.ViewSubject);
            prattInvoiceAnalyst.Uses(invoiceTransactionSoftwareSystem, "Uses");

            var eagleDataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Eagle Data", "TBD.");
            invoiceTransactionSoftwareSystem.Uses(eagleDataSoftwareSystem, "TBD");

            var workScopingSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", "TBD.");
            workScopingSoftwareSystem.AddTags(AdditionalTags.FutureState);
            invoiceTransactionSoftwareSystem.Uses(workScopingSoftwareSystem, "TBD");

            var odinSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
            invoiceTransactionSoftwareSystem.Uses(odinSoftwareSystem, "TBD - Uses Access DB");

            var sapSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
            invoiceTransactionSoftwareSystem.Uses(sapSoftwareSystem, "TBD");

            var speidSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information. ");
            invoiceTransactionSoftwareSystem.Uses(speidSoftwareSystem, "TBD");

            var fleetcareSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices");
            invoiceTransactionSoftwareSystem.Uses(fleetcareSoftwareSystem, "TBD");

            var sharePointSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SharePoint", "TBD.");
            invoiceTransactionSoftwareSystem.Uses(sharePointSoftwareSystem, "TBD");



            var costManagementMetricsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", "TBD (Dafina Georgievska/Matt Wentworth).");
            costManagementMetricsSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var spidrsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "TBD - Financial Accounting.");
            spidrsSoftwareSystem.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
            spidrsSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var quoteErrorToolSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "This is temporary as these");
            quoteErrorToolSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD - Kim's tool?");

            var teradataSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Teradata", "TBD.");
            teradataSoftwareSystem.AddTags(AdditionalTags.SunsetPhaseOut);
            teradataSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var w2CDownstreamSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "W2C", "TBD (Chandra Kankanala) – future state.");
            w2CDownstreamSoftwareSystem.AddTags(AdditionalTags.FutureState);
            w2CDownstreamSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var allocationReportSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", "TBD.");
            allocationReportSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");

            var fleetManagementDashboardSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", "TBD.");
            fleetManagementDashboardSoftwareSystem.Uses(invoiceTransactionSoftwareSystem, "TBD");


            var asoSoftwareSystem = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
            
            // Containers

            var invoiceTransactionSystemWebClientContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions System Web Client", "The Invoice Transactions System Web Client.", "TBD");
            prattInvoiceAnalyst.Uses(invoiceTransactionSystemWebClientContainer, "Uses", "Web Browser");

            var invoiceTransactionSystemWebBackendContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions System Web Backend", "The Invoice Transactions System Web Backend.", "TBD");
            prattInvoiceAnalyst.Uses(invoiceTransactionSystemWebBackendContainer, "Uses", "HTTPS");

            var invoiceTransactionSystemAppServiceContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions System App Service", "The Invoice Transactions System App Service.", "TBD");
            invoiceTransactionSystemWebClientContainer.Uses(invoiceTransactionSystemAppServiceContainer, "Uses", "HTTPS");
            invoiceTransactionSystemWebBackendContainer.Uses(invoiceTransactionSystemAppServiceContainer, "Uses", "HTTPS");
            
            var invoiceTransactionSystemMessageBrokerContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions System Message Broker", "The Invoice Transactions System Message Broker.", "TBD");
            invoiceTransactionSystemMessageBrokerContainer.AddTags(AdditionalTags.Queue);
            invoiceTransactionSystemAppServiceContainer.Uses(invoiceTransactionSystemMessageBrokerContainer, "Uses", "TBD - HTTP/AMQP");
            
            var invoiceTransactionSystemDataServiceContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions Data Service", "The Invoice Transactions System Data Service.", "TBD");
            invoiceTransactionSystemMessageBrokerContainer.Uses(invoiceTransactionSystemDataServiceContainer, "Uses", "TBD");

            var invoiceTransactionSystemOperationalDatabaseContainer = invoiceTransactionSoftwareSystem.AddContainer("Invoice Transactions System Operational DB", "The Invoice Transactions System Operational Database.", "TBD");
            invoiceTransactionSystemOperationalDatabaseContainer.AddTags(AdditionalTags.Database);
            invoiceTransactionSystemDataServiceContainer.Uses(invoiceTransactionSystemOperationalDatabaseContainer, "TBD");

            // Components

            var invoiceTransactionSystemAdminViewComponent = invoiceTransactionSystemWebClientContainer.AddComponent("Admin View", "Used by administrators of the Invoice Transaction System.", "TBD");
            prattInvoiceAnalyst.Uses(invoiceTransactionSystemAdminViewComponent, "Uses", "TBD.");

            var invoiceTransactionSystemAdminControllerComponent = invoiceTransactionSystemWebClientContainer.AddComponent("Admin Controller", "Used by administrators of the Invoice Transaction System.", "TBD");
            invoiceTransactionSystemAdminViewComponent.Uses(invoiceTransactionSystemAdminControllerComponent, "Respond to actions and manage state");

            // Views 

            var views = workspace.Views;

            // Context Views

            views.CreateEnterpriseContextLandscapeViewFor(enterprise, PaperSize.A3_Landscape);

            views.CreateSystemContextViewFor(invoiceTransactionSoftwareSystem, PaperSize.A3_Landscape);

            // Container Views

            views.CreateContainerViewFor(invoiceTransactionSoftwareSystem, PaperSize.A3_Landscape);

            // Component Views

            views.CreateComponentViewFor(invoiceTransactionSystemWebClientContainer);

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
