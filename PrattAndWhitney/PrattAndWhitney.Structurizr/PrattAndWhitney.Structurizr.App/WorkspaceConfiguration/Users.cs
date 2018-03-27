using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Users
    {
        public static Person InvoiceTeam { get; private set; }
        public static Person InvoiceAnalyst { get; private set; }
        public static Person InvoiceManager { get; private set; }
        public static Person FleetManager { get; private set; }
        public static Person FinanceUser { get; private set; }
        public static Person CostsUser { get; private set; }
        public static Person ToolSupport { get; private set; }
        public static Person PowerPlantEngineer { get; private set; }
        public static Person OnSiteManager { get; private set; }
        public static Person ShopTeam { get; private set; }
        public static Person ShopUser { get; private set; }
        public static Person ShopManager { get; private set; }

        public static void Configure(Model model)
        {
            // Users
            InvoiceTeam = model.AddPerson(Location.Internal, "Invoice Team", "Managers and Analysts");
            InvoiceAnalyst = model.AddPerson(Location.Internal, "Invoice Analyst", " ");
            InvoiceManager = model.AddPerson(Location.Internal, "Invoice Manager", " ");
            FleetManager = model.AddPerson(Location.Internal, "Fleet Manager", " ");
            FinanceUser = model.AddPerson(Location.Internal, "Finance User", " ");
            CostsUser = model.AddPerson(Location.Internal, "Costs User", " ");
            ToolSupport = model.AddPerson(Location.Internal, "Tool Support", " ");
            PowerPlantEngineer = model.AddPerson(Location.External, "Power Plant Engineer", " ");
            OnSiteManager = model.AddPerson(Location.External, "On-Site Manager", " ");
            ShopTeam = model.AddPerson(Location.External, "Shop Team", "Managers and Users");
            ShopUser = model.AddPerson(Location.External, "Shop User", " ");
            ShopManager = model.AddPerson(Location.External, "Shop Manager", " ");
        }
    }
}