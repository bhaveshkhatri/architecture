using Structurizr;

namespace PrattAndWhitney.Structurizr.App
{
    public static class Users
    {
        public static Person InvoiceAnalyst { get; private set; }
        public static Person InvoiceManager { get; private set; }
        public static Person FleetManager { get; private set; }
        public static Person FinanceUser { get; private set; }
        public static Person CostsUser { get; private set; }
        public static Person ToolSupport { get; private set; }
        public static Person PowerPlantEngineer { get; private set; }
        public static Person OnSiteManager { get; private set; }
        public static Person ShopUser { get; private set; }

        public static void Configure(Model model)
        {
            // Users
            InvoiceAnalyst = model.AddPerson(Location.Internal, "Invoice Analyst", "TBD.");
            InvoiceManager = model.AddPerson(Location.Internal, "Invoice Manager", "TBD.");
            FleetManager = model.AddPerson(Location.Internal, "Fleet Manager", "TBD.");
            FinanceUser = model.AddPerson(Location.Internal, "Finance User", "TBD.");
            CostsUser = model.AddPerson(Location.Internal, "Costs User", "TBD.");
            ToolSupport = model.AddPerson(Location.Internal, "Tool Support", "TBD.");
            PowerPlantEngineer = model.AddPerson(Location.External, "Power Plant Engineer", "TBD.");
            OnSiteManager = model.AddPerson(Location.External, "On-Site Manager", "TBD.");
            ShopUser = model.AddPerson(Location.External, "Shop User", "TBD.");
        }
    }
}