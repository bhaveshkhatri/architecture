using Structurizr;
using Structurizr.Api;

namespace Ascension.Structurizr.App
{
    class Program
    {
        private const long WorkspaceId = 38101;
        private const string ApiKey = "66fe366b-708d-4d38-b0f0-e45d1620200b";
        private const string ApiSecret = "69b55f1a-01b9-4da5-9577-e6373c542972";

        static void Main(string[] args)
        {
            Workspace workspace = new Workspace("Getting Started", "This is a model of my software system.");
            Model model = workspace.Model;
            ViewSet views = workspace.Views;

            // Enterprise

            Enterprise enterprise = model.Enterprise = new Enterprise("Ascension");

            // Systems and Users

            SoftwareSystem ascensionAutomationPlatform = model.AddSoftwareSystem("Automation Platform", "Ascension Automation Platform.");

            SoftwareSystem matchExceptionTracker = model.AddSoftwareSystem("ME Tracker", "Tracks and manages match exceptions and related work.");
            matchExceptionTracker.Uses(ascensionAutomationPlatform, "Uses");

            SoftwareSystem otherBackOfficeApplication = model.AddSoftwareSystem("Other Back Office Application", "Tracks and manages match other back office related work.");
            otherBackOfficeApplication.Uses(ascensionAutomationPlatform, "Uses");

            // Containers

            Container intelligentAutomationsService = ascensionAutomationPlatform.AddContainer("Intelligent Automations Service", "Provides automation related services.", "TBD");

            Container vendorPortal = ascensionAutomationPlatform.AddContainer("Vendor Portal", "Provides vendor self service capabilities.", "TBD");
            //vendorPortal.Uses(intelligentAutomationsService, "Uses");

            Container dataProviderService = ascensionAutomationPlatform.AddContainer("Data Provider Service", "Provides data to other services.", "TBD");

            // Components
            //Component 

            // Views

            EnterpriseContextView enterpriseContextView = views.CreateEnterpriseContextView("SystemLandscape", "The system landscape diagram for Ascension Automation Platform.");
            enterpriseContextView.AddAllElements();

            SystemContextView systemContextView = views.CreateSystemContextView(ascensionAutomationPlatform, "SystemContext", "System Context Diagram.");
            systemContextView.AddNearestNeighbours(ascensionAutomationPlatform);

            ContainerView containerView = views.CreateContainerView(ascensionAutomationPlatform, "Containers", "The container diagram for the Ascension Automation Platform.");
            containerView.AddAllContainers();

            // Styles

            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });

            // Build

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
