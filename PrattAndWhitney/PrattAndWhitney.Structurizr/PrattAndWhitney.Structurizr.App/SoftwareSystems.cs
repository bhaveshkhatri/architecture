using Structurizr;

namespace PrattAndWhitney.Structurizr.App
{
    public static class SoftwareSystems
    {
        public static void Configure(Model model)
        {
            Target.Configure(model);

            Other.Configure(model);
        }

        public static class Target
        {
            public static SoftwareSystem InvoiceTransactionsSoftwareSystem { get; private set; }
            public static SoftwareSystem InfrastructureServicesSoftwareSystem { get; private set; }

            public static void Configure(Model model)
            {
                InvoiceTransactionsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "ITS", "Invoice Transaction System.");
                InvoiceTransactionsSoftwareSystem.AddTags(AdditionalTags.TargetSystem);
                Users.InvoiceAnalyst.Uses(InvoiceTransactionsSoftwareSystem, "Uses");
                Users.InvoiceManager.Uses(InvoiceTransactionsSoftwareSystem, "Uses");
                Users.ToolSupport.Uses(InvoiceTransactionsSoftwareSystem, "Uses");
                Users.ShopUser.Uses(InvoiceTransactionsSoftwareSystem, "Uses");

                InfrastructureServicesSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Infrastructure Services", "Message Broker / Cache / Notification Hub.");
                InfrastructureServicesSoftwareSystem.AddTags(AdditionalTags.InfrastructureServices);
                InfrastructureServicesSoftwareSystem.AddTags(AdditionalTags.Subsystem);
            }
        }
        public static class Other
        {
            public static SoftwareSystem CostManagementMetrics { get; private set; }
            public static SoftwareSystem Spidrs { get; private set; }
            public static SoftwareSystem QuoteErrorTool { get; private set; }
            public static SoftwareSystem Teradata { get; private set; }
            public static SoftwareSystem W2CDownstream { get; private set; }
            public static SoftwareSystem AllocationReport { get; private set; }
            public static SoftwareSystem FleetManagementDashboard { get; private set; }
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
                // Consumers/Downstream Software Systems

                CostManagementMetrics = model.AddSoftwareSystem(Location.Internal, "Cost Management Metrics", "TBD.");
                CostManagementMetrics.AddProperty(Properties.KeyContact, "Dafina Georgievska/Matt Wentworth");
                CostManagementMetrics.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                Spidrs = model.AddSoftwareSystem(Location.Internal, "SPIDRS", "TBD - Financial Accounting.");
                Spidrs.AddProperty(Properties.KeyContact, "Mike Faulk – Tsunami Tsolutions");
                Spidrs.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                QuoteErrorTool = model.AddSoftwareSystem(Location.Internal, "Quote Tool / Error Tool", "This is temporary");
                QuoteErrorTool.AddProperty(Properties.KeyContact, "Kim Rose");
                QuoteErrorTool.AddTags(AdditionalTags.SunsetPhaseOut);
                QuoteErrorTool.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                Teradata = model.AddSoftwareSystem(Location.Internal, "Teradata", "TBD.");
                Teradata.AddProperty(Properties.KeyContact, "TBD");
                Teradata.AddTags(AdditionalTags.SunsetPhaseOut);
                Teradata.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                W2CDownstream = model.AddSoftwareSystem(Location.Internal, "W2C", "TBD");
                W2CDownstream.AddProperty(Properties.KeyContact, "Chandra Kankanala");
                W2CDownstream.AddTags(AdditionalTags.FutureState);
                W2CDownstream.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                AllocationReport = model.AddSoftwareSystem(Location.Internal, "Allocation Report ($)", "TBD.");
                AllocationReport.AddProperty(Properties.KeyContact, "TBD");
                AllocationReport.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                FleetManagementDashboard = model.AddSoftwareSystem(Location.Internal, "Fleet Management Dashboard", "TBD.");
                FleetManagementDashboard.AddProperty(Properties.KeyContact, "TBD");
                FleetManagementDashboard.Uses(Target.InvoiceTransactionsSoftwareSystem, "TBD");

                // Producers/Upstream Software Systems

                ActiveDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Active Directory");
                Target.InvoiceTransactionsSoftwareSystem.Uses(ActiveDirectory, "LDAP");

                EagleData = model.AddSoftwareSystem(Location.Internal, "Eagle Data", "TBD.");
                EagleData.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(EagleData, "TBD");

                WorkScoping = model.AddSoftwareSystem(Location.Internal, "New Workscoping Tool", "TBD.");
                WorkScoping.AddProperty(Properties.KeyContact, "TBD");
                WorkScoping.AddTags(AdditionalTags.FutureState);
                Target.InvoiceTransactionsSoftwareSystem.Uses(WorkScoping, "TBD");

                Odin = model.AddSoftwareSystem(Location.Internal, "ODIN", "Allocation tool. ");
                Odin.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(Odin, "TBD - Uses Access DB");

                Sap = model.AddSoftwareSystem(Location.Internal, "SAP", "SAP implementation (master data).");
                Sap.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(Sap, "TBD");

                Speid = model.AddSoftwareSystem(Location.Internal, "SPEID", "Contract information. ");
                Speid.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(Speid, "TBD");

                Fleetcare = model.AddSoftwareSystem(Location.Internal, "Fleetcare", "Customer Invoices");
                Fleetcare.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(Fleetcare, "TBD");

                SharePoint = model.AddSoftwareSystem(Location.Internal, "SharePoint", "TBD.");
                SharePoint.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(SharePoint, "TBD");

                FileSystem = model.AddSoftwareSystem(Location.Internal, "Filesystem", "TBD.");
                FileSystem.AddTags(AdditionalTags.Subsystem);
                FileSystem.AddTags(AdditionalTags.Files);
                FileSystem.AddProperty(Properties.KeyContact, "TBD");
                Target.InvoiceTransactionsSoftwareSystem.Uses(FileSystem, "OS/NAS");

                // Other Software Systems

                Aso = model.AddSoftwareSystem(Location.Unspecified, "ASO", "TBD - Davinia says it's sort of a replacement for AIM.");
                FleetManagementDashboard.AddProperty(Properties.KeyContact, "TBD");
            }
        }
    }
}