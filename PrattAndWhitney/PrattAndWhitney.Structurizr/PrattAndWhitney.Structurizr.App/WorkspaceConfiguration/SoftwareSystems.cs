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
                InvoiceTransactionsSystem = model.AddSoftwareSystem(Location.Internal, "ITS", "The Invoice Transaction System");
                InvoiceTransactionsSystem.AddTags(AdditionalTags.TargetSystem);
                Users.InvoiceTeam.Uses(InvoiceTransactionsSystem, "Uses");
                Users.FleetManager.Uses(InvoiceTransactionsSystem, "Uses");
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
            //public static SoftwareSystem Aso { get; private set; }

            public static void Configure(Model model)
            {
                ActiveDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Active Directory");
                Target.InvoiceTransactionsSystem.Uses(ActiveDirectory, "LDAP");

                EagleData = model.AddSoftwareSystem(Location.Internal, "Eagle Data", " ");
                EagleData.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(EagleData, "Uses");

                WorkScoping = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", " ");
                WorkScoping.AddProperty(Properties.KeyContact, "TBD");
                WorkScoping.AddTags(AdditionalTags.FutureState);
                Target.InvoiceTransactionsSystem.Uses(WorkScoping, "Uses");

                Odin = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
                Odin.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Odin, "Uses");

                Sap = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
                Sap.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Sap, "Uses");

                Speid = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information.");
                Speid.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Speid, "Uses");

                Fleetcare = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices.");
                Fleetcare.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(Fleetcare, "Uses");

                SharePoint = model.AddSoftwareSystem(Location.Internal, "SharePoint", " ");
                SharePoint.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSystem.Uses(SharePoint, "Uses");

                FileSystem = model.AddSoftwareSystem(Location.Internal, "Filesystem", " ");
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
                CostManagementMetrics = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", " ");
                CostManagementMetrics.AddProperty(Properties.KeyContact, "Dafina Georgievska/Matt Wentworth");
                CostManagementMetrics.Uses(Target.InvoiceTransactionsSystem, "Uses");

                Spidrs = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "Financial Accounting.");
                Spidrs.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
                Spidrs.Uses(Target.InvoiceTransactionsSystem, "Uses");

                QuoteErrorTool = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "Temporary");
                QuoteErrorTool.AddProperty(Properties.KeyContact, "Kim Rose");
                QuoteErrorTool.AddTags(AdditionalTags.SunsetPhaseOut);
                QuoteErrorTool.Uses(Target.InvoiceTransactionsSystem, "Uses");

                Teradata = model.AddSoftwareSystem(Location.Internal, "Teradata", " ");
                Teradata.AddProperty(Properties.KeyContact, "TBD");
                Teradata.AddTags(AdditionalTags.SunsetPhaseOut);
                Teradata.Uses(Target.InvoiceTransactionsSystem, "Uses");

                WingToCashDownstream = model.AddSoftwareSystem(Location.Internal, "W2C", "Downstream W2C Systems");
                WingToCashDownstream.AddProperty(Properties.KeyContact, "Chandra Kankanala");
                WingToCashDownstream.AddTags(AdditionalTags.FutureState);
                WingToCashDownstream.Uses(Target.InvoiceTransactionsSystem, "Uses");

                AllocationReport = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", " ");
                AllocationReport.AddProperty(Properties.KeyContact, "TBD");
                AllocationReport.Uses(Target.InvoiceTransactionsSystem, "Uses");

                FleetManagementDashboard = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", " ");
                FleetManagementDashboard.AddProperty(Properties.KeyContact, "TBD");
                FleetManagementDashboard.Uses(Target.InvoiceTransactionsSystem, "Uses");
            }
        }

        public static class Other
        {
            public static SoftwareSystem Aso { get; private set; }

            public static void Configure(Model model)
            {
                //Aso = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
                //Aso.AddProperty(Properties.KeyContact, "TBD");
            }
        }
    }
}