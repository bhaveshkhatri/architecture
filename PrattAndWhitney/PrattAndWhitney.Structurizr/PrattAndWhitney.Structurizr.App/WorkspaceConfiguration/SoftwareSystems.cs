using PrattAndWhitney.Structurizr.App.Extensions;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class SoftwareSystems
    {
        public static void Configure(Model model)
        {
            Target.Configure(model);
            Upstream.Configure(model);
            Downstream.Configure(model);
            Other.Configure(model);
        }

        public static class Target
        {
            public static SoftwareSystem InvoiceTransactionsSystem { get; private set; }
            public static SoftwareSystem InfrastructureServices { get; private set; }

            public static void Configure(Model model)
            {
                InvoiceTransactionsSystem = model.AddSoftwareSystem(Location.Internal, "ITS", "Invoice Transaction System.");
                InvoiceTransactionsSystem.AddTags(AdditionalTags.TargetSystem);
                Users.InvoiceAnalyst.Uses(InvoiceTransactionsSystem, "Uses");
                Users.InvoiceManager.Uses(InvoiceTransactionsSystem, "Uses");
                Users.ToolSupport.Uses(InvoiceTransactionsSystem, "Uses");
                Users.ShopUser.Uses(InvoiceTransactionsSystem, "Uses");

                InfrastructureServices = model.AddSoftwareSystem(Location.Internal, "Infrastructure Services", "Event Bus / Cache.");
                InfrastructureServices.AddTags(AdditionalTags.InfrastructureServices);
                InfrastructureServices.AddTags(AdditionalTags.Subsystem);
            }
        }

        public static class Upstream
        {
            public static SoftwareSystem ActiveDirectory { get; private set; }
            public static SoftwareSystem EagleData { get; private set; }
            public static SoftwareSystem WorkScoping { get; private set; }
            public static SoftwareSystem Odin { get; private set; }
            public static SoftwareSystem Sap { get; private set; }
            public static SoftwareSystem Speid { get; private set; }
            public static SoftwareSystem Fleetcare { get; private set; }
            public static SoftwareSystem SharePoint { get; private set; }
            public static SoftwareSystem FileSystem { get; private set; }
            public static SoftwareSystem Aso { get; private set; }

            public static void Configure(Model model)
            {
                ActiveDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Active Directory");
                Target.InvoiceTransactionsSystem.Uses(ActiveDirectory, "LDAP");

                EagleData = model.AddSoftwareSystem(Location.Internal, "Eagle Data", "TBD.");
                EagleData.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(EagleData, "TBD");

                WorkScoping = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", "TBD.");
                WorkScoping.AddProperty(Properties.KeyContact, "TBD");
                WorkScoping.AddTags(AdditionalTags.FutureState);
                Target.InvoiceTransactionsSystem.Uses(WorkScoping, "TBD");

                Odin = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
                Odin.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Odin, "TBD - Uses Access DB");

                Sap = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
                Sap.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Sap, "TBD");

                Speid = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information. ");
                Speid.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Speid, "TBD");

                Fleetcare = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices");
                Fleetcare.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Fleetcare, "TBD");

                SharePoint = model.AddSoftwareSystem(Location.Internal, "SharePoint", "TBD.");
                SharePoint.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(SharePoint, "TBD");

                FileSystem = model.AddSoftwareSystem(Location.Internal, "Filesystem", "TBD.");
                FileSystem.AddTags(AdditionalTags.Subsystem);
                FileSystem.AddTags(AdditionalTags.Files);
                FileSystem.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(FileSystem, "OS/NAS");
            }
        }

        public static class Downstream
        {
            public static SoftwareSystem CostManagementMetrics { get; private set; }
            public static SoftwareSystem Spidrs { get; private set; }
            public static SoftwareSystem QuoteErrorTool { get; private set; }
            public static SoftwareSystem Teradata { get; private set; }
            public static SoftwareSystem WingToCashDownstream { get; private set; }
            public static SoftwareSystem AllocationReport { get; private set; }
            public static SoftwareSystem FleetManagementDashboard { get; private set; }

            public static void Configure(Model model)
            {
                CostManagementMetrics = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", "TBD.");
                CostManagementMetrics.AddProperty(Properties.KeyContact, "Dafina Georgievska/Matt Wentworth");
                CostManagementMetrics.Uses(Target.InvoiceTransactionsSystem, "TBD");

                Spidrs = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "TBD - Financial Accounting.");
                Spidrs.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
                Spidrs.Uses(Target.InvoiceTransactionsSystem, "TBD");

                QuoteErrorTool = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "This is temporary");
                QuoteErrorTool.AddProperty(Properties.KeyContact, "Kim Rose");
                QuoteErrorTool.AddTags(AdditionalTags.SunsetPhaseOut);
                QuoteErrorTool.Uses(Target.InvoiceTransactionsSystem, "TBD");

                Teradata = model.AddSoftwareSystem(Location.Internal, "Teradata", "TBD.");
                Teradata.AddProperty(Properties.KeyContact, "TBD");
                Teradata.AddTags(AdditionalTags.SunsetPhaseOut);
                Teradata.Uses(Target.InvoiceTransactionsSystem, "TBD");

                WingToCashDownstream = model.AddSoftwareSystem(Location.Internal, "W2C", "TBD");
                WingToCashDownstream.AddProperty(Properties.KeyContact, "Chandra Kankanala");
                WingToCashDownstream.AddTags(AdditionalTags.FutureState);
                WingToCashDownstream.Uses(Target.InvoiceTransactionsSystem, "TBD");

                AllocationReport = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", "TBD.");
                AllocationReport.AddProperty(Properties.KeyContact, "TBD");
                AllocationReport.Uses(Target.InvoiceTransactionsSystem, "TBD");

                FleetManagementDashboard = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", "TBD.");
                FleetManagementDashboard.AddProperty(Properties.KeyContact, "TBD");
                FleetManagementDashboard.Uses(Target.InvoiceTransactionsSystem, "TBD");
            }
        }

        public static class Other
        {
            public static SoftwareSystem Aso { get; private set; }

            public static void Configure(Model model)
            {
                Aso = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
                Aso.AddProperty(Properties.KeyContact, "TBD");
            }
        }
    }
}