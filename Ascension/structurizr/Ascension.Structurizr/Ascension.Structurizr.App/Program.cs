using Structurizr;
using Structurizr.Api;
using System.Configuration;

namespace Ascension.Structurizr.App
{
    public class Program
    {
        private static long WorkspaceId;
        private static string ApiKey;
        private static string ApiSecret;

        static void Main(string[] args)
        {
            WorkspaceId = long.Parse(ConfigurationManager.AppSettings["WorkspaceId"]);
            ApiKey = ConfigurationManager.AppSettings["ApiKey"];
            ApiSecret = ConfigurationManager.AppSettings["ApiSecret"];

            var workspace = new Workspace("Ascension", "Model of the Automation Platform.");
            var model = workspace.Model;

            // Enterprise

            var enterprise = model.Enterprise = new Enterprise("Ascension");

            // Users
            var meProcessorPerson = model.AddPerson("ME Processor", "User that processes match exceptions.");
            var vendorPerson = model.AddPerson("Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson("Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem("Automation Platform", "Ascension Automation Platform.");

            var platformUserDesktopSoftwareSystem = model.AddSoftwareSystem("Platform User Desktop", "The desktop of an automation platform end user.");
            platformUserDesktopSoftwareSystem.Uses(platformSoftwareSystem, "Uses");

            var meTrackerSoftwareSystem = model.AddSoftwareSystem("ME Tracker", "Tracks and manages match exceptions and related work.");
            meTrackerSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            meProcessorPerson.Uses(meTrackerSoftwareSystem, "Uses");

            var otherBackOfficeSoftwareSystem = model.AddSoftwareSystem("Other Back Office Application", "Tracks and manages match other back office related work.");
            otherBackOfficeSoftwareSystem.Uses(platformSoftwareSystem, "Uses");

            // Containers

            var openSpanContainer = platformUserDesktopSoftwareSystem.AddContainer("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "TBD");

            var intelligentAutomationServiceContainer = platformSoftwareSystem.AddContainer("Intelligent Automations Service", "Provides automation related services.", "TBD");

            var vendorPortalContainer = platformSoftwareSystem.AddContainer("Vendor Portal", "Provides vendor self service capabilities.", "TBD");
            vendorPerson.Uses(vendorPortalContainer, "Uses");

            var dataProviderServiceContainer = platformSoftwareSystem.AddContainer("Data Provider Service", "Provides data to other services.", "TBD");

            // Data Provider Service Components
            var automationAdapterComponent = dataProviderServiceContainer.AddComponent("Automation Adapter", "Adapter to read automation information.");

            // Views 

            var views = workspace.Views;

            // Context Views

            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView("Enterprise Context", "The system landscape diagram for Ascension Automation Platform.");
            systemLandscapeEnterpriseContextView.AddAllSoftwareSystems();
            foreach (var softwareSystem in model.SoftwareSystems)
            {
                systemLandscapeEnterpriseContextView.AddNearestNeighbours(softwareSystem);
            }

            var platformSystemContextView = views.CreateSystemContextView(platformSoftwareSystem, "Platform Context", "System Context Diagram.");
            platformSystemContextView.AddNearestNeighbours(platformSoftwareSystem);

            // Container Views

            var platformUserDesktopContainerView = views.CreateContainerView(platformUserDesktopSoftwareSystem, "Platform User Desktop Containers", "The container diagram for the platform user desktop.");
            platformUserDesktopContainerView.Add(openSpanContainer);
            platformUserDesktopContainerView.AddNearestNeighbours(openSpanContainer);

            var platformContainerView = views.CreateContainerView(platformSoftwareSystem, "Platform Containers", "The container diagram for the Ascension Automation Platform.");
            platformContainerView.AddAllContainers();
            platformContainerView.AddNearestNeighbours(vendorPortalContainer);
            platformContainerView.AddNearestNeighbours(intelligentAutomationServiceContainer);


            // Component Views
            var dataProviderServiceComponentView = views.CreateComponentView(dataProviderServiceContainer, "Data Provider Components", "The component diagram for the Data Provider Service.");
            dataProviderServiceComponentView.Add(automationAdapterComponent);

            // Styles

            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });

            Build(workspace);
        }

        private static void Build(Workspace workspace)
        {
            // Build

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
