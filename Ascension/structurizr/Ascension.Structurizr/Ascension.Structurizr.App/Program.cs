using Structurizr;
using Structurizr.Api;
using System.Configuration;
using System.Linq;

namespace Ascension.Structurizr.App
{
    public class Program
    {
        private static int CurrentVersion = 2;
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

            switch (CurrentVersion)
            {
                case 1:
                    BuildV1(workspace, model);
                    break;
                case 2:
                    BuildV2(workspace, model);
                    break;
                default:
                    break;
            }

            Upload(workspace);
        }

        private static void BuildV1(Workspace workspace, Model model)
        {
            // Enterprise

            var enterprise = model.Enterprise = new Enterprise("Ascension");

            // Users
            var meProcessorPerson = model.AddPerson("ME Processor", "User that processes match exceptions.");
            var backOfficeUserPerson = model.AddPerson("Back Office Application User", "User of other back office applications.");
            var vendorPerson = model.AddPerson("Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson("Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem("Automation Platform", "Ascension Automation Platform.");

            var platformUserDesktopSoftwareSystem = model.AddSoftwareSystem("Platform User Desktop", "The desktop of an automation platform end user.");
            platformUserDesktopSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            meProcessorPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");
            backOfficeUserPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");

            var matchExceptionTrackerSoftwareSystem = model.AddSoftwareSystem("ME Tracker", "Tracks and manages match exceptions and related work.");
            matchExceptionTrackerSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            platformUserDesktopSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Uses");

            var otherBackOfficeSoftwareSystem = model.AddSoftwareSystem("Other Back Office Application", "Tracks and manages match other back office related work.");
            otherBackOfficeSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            platformUserDesktopSoftwareSystem.Uses(otherBackOfficeSoftwareSystem, "Uses");

            // Containers

            var webBrowserContainer = platformUserDesktopSoftwareSystem.AddContainer("Web Browser", "Web Browser (e.g. Chrome, IE, Firefox).", "TBD");
            var openSpanContainer = platformUserDesktopSoftwareSystem.AddContainer("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "TBD");
            var radiloContainer = platformUserDesktopSoftwareSystem.AddContainer("RADILO", "Unified Desktop.", "TBD");
            openSpanContainer.Uses(webBrowserContainer, "Automates");
            webBrowserContainer.Uses(openSpanContainer, "Triggers RDA and provides data");
            openSpanContainer.Uses(radiloContainer, "Automates");
            radiloContainer.Uses(openSpanContainer, "Collects data, triggers RDA and provides data");

            webBrowserContainer.Uses(matchExceptionTrackerSoftwareSystem, "Uses");
            webBrowserContainer.Uses(otherBackOfficeSoftwareSystem, "Uses");

            var cognitiveScaleCortexContainer = platformSoftwareSystem.AddContainer("Cortex", "Cortex platform provided by CognitiveScale", "TBD");

            var matchExceptionDatabaseContainer = matchExceptionTrackerSoftwareSystem.AddContainer("ME Database", "Match Exception Tracker operational DB.", "TBD");
            platformSoftwareSystem.Uses(matchExceptionDatabaseContainer, "Uses");

            var matchExceptionTrackerWebApplicationContainer = matchExceptionTrackerSoftwareSystem.AddContainer("ME Tracker Web Application", "Match Exception Tracker Web Application.", "TBD");
            matchExceptionTrackerWebApplicationContainer.Uses(matchExceptionDatabaseContainer, "Uses");
            matchExceptionTrackerWebApplicationContainer.Uses(platformSoftwareSystem, "Uses");

            var vendorPortalContainer = platformSoftwareSystem.AddContainer("Vendor Portal", "Provides vendor self service capabilities.", "TBD");
            vendorPerson.Uses(vendorPortalContainer, "Uses");

            var dataProviderServiceContainer = platformSoftwareSystem.AddContainer("Data Provider Service", "Provides data to other services.", "TBD");
            dataProviderServiceContainer.Uses(cognitiveScaleCortexContainer, "Uses");

            var intelligentAutomationServiceContainer = platformSoftwareSystem.AddContainer("Intelligent Automations Service", "Provides automation related services.", "TBD");
            intelligentAutomationServiceContainer.Uses(dataProviderServiceContainer, "Uses");
            matchExceptionTrackerSoftwareSystem.Uses(intelligentAutomationServiceContainer, "Uses");

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
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A5_Landscape;

            var platformSystemContextView = views.CreateSystemContextView(platformSoftwareSystem, "Platform System Context", "System Context Diagram.");
            platformSystemContextView.AddNearestNeighbours(platformSoftwareSystem);

            var matchExceptionTrackerSystemContextView = views.CreateSystemContextView(matchExceptionTrackerSoftwareSystem, "Match Exception Tracker System Context", "System Context Diagram.");
            matchExceptionTrackerSystemContextView.AddNearestNeighbours(matchExceptionTrackerSoftwareSystem);

            // Container Views

            var platformUserDesktopContainerView = views.CreateContainerView(platformUserDesktopSoftwareSystem, "Platform User Desktop Containers", "The container diagram for the platform user desktop.");
            platformUserDesktopContainerView.Add(openSpanContainer);
            platformUserDesktopContainerView.AddNearestNeighbours(openSpanContainer);
            var relationships = platformUserDesktopContainerView.Relationships.ToList();
            foreach (var relationship in relationships)
            {
                platformUserDesktopContainerView.AddNearestNeighbours(relationship.Relationship.Destination);
            }
            platformUserDesktopContainerView.PaperSize = PaperSize.A5_Landscape;

            var platformContainerView = views.CreateContainerView(platformSoftwareSystem, "Platform Containers", "The container diagram for the Ascension Automation Platform.");
            foreach (var container in platformSoftwareSystem.Containers)
            {
                platformContainerView.Add(container);
                platformContainerView.AddNearestNeighbours(container);
            }
            platformContainerView.PaperSize = PaperSize.A5_Landscape;

            var matchExceptionTrackerContainerView = views.CreateContainerView(matchExceptionTrackerSoftwareSystem, "Match Exception Tracker Containers", "The container diagram for the Match Exception Tracker.");
            foreach (var container in matchExceptionTrackerSoftwareSystem.Containers)
            {
                matchExceptionTrackerContainerView.Add(container);
                matchExceptionTrackerContainerView.AddNearestNeighbours(container);
            }
            matchExceptionTrackerContainerView.PaperSize = PaperSize.A5_Landscape;

            // Component Views
            var dataProviderServiceComponentView = views.CreateComponentView(dataProviderServiceContainer, "Data Provider Components", "The component diagram for the Data Provider Service.");
            dataProviderServiceComponentView.Add(automationAdapterComponent);

            // Styles

            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
        }

        private static void BuildV2(Workspace workspace, Model model)
        {
            // Enterprise

            var enterprise = model.Enterprise = new Enterprise("Ascension");

            // Users
            var matchExceptionProcessorPerson = model.AddPerson(Location.Internal, "Match Exception Processor", "User that processes match exceptions.");
            var backOfficeUserPerson = model.AddPerson(Location.Internal, "Back Office Application User", "User of other back office applications.");
            var vendorPerson = model.AddPerson(Location.External, "Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson(Location.Internal, "Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Automation Platform", "Ascension Automation Platform.");

            var platformUserDesktopSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Platform User Desktop", "The desktop of an automation platform end user.");
            matchExceptionProcessorPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");
            backOfficeUserPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");
            vendorPerson.Uses(platformSoftwareSystem, "Uses");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses");
            platformUserDesktopSoftwareSystem.Uses(platformSoftwareSystem, "Uses");

            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Uses");

            var matchExceptionTrackerSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Match Exception Tracker", "Tracks and manages match exceptions and related work.");
            matchExceptionTrackerSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerSoftwareSystem, "Uses");
            platformUserDesktopSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Uses");

            var otherBackOfficeSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Other Back Office Application", "Tracks and manages match other back office related work.");
            backOfficeUserPerson.Uses(otherBackOfficeSoftwareSystem, "Uses");
            otherBackOfficeSoftwareSystem.Uses(platformSoftwareSystem, "Uses");
            platformUserDesktopSoftwareSystem.Uses(otherBackOfficeSoftwareSystem, "Uses");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Uses");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Uses");

            // Containers

            var webBrowserContainer = platformUserDesktopSoftwareSystem.AddContainer("Web Browser", "Web Browser (e.g. Chrome, IE, Firefox).", "TBD");

            var openSpanContainer = platformUserDesktopSoftwareSystem.AddContainer("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "TBD");
            webBrowserContainer.Uses(openSpanContainer, "Triggers RDA and provides data");
            openSpanContainer.Uses(webBrowserContainer, "Automates");

            var radiloContainer = platformUserDesktopSoftwareSystem.AddContainer("RADILO", "Unified Desktop.", "TBD");
            openSpanContainer.Uses(radiloContainer, "Automates");
            radiloContainer.Uses(openSpanContainer, "Collects data, triggers RDA and provides data");

            // Views 

            var views = workspace.Views;

            // Context Views

            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView("Enterprise Context", "The system landscape diagram for Ascension Automation Platform.");
            systemLandscapeEnterpriseContextView.AddAllElements();
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;

            var platformUserDesktopSystemContextView = views.CreateSystemContextView(platformUserDesktopSoftwareSystem, "Platform User Desktop System Context", "The system context for the platform user desktop.");
            platformUserDesktopSystemContextView.AddNearestNeighbours(platformUserDesktopSoftwareSystem);
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;

            // Container Views

            var platformUserDesktopContainerView = views.CreateContainerView(platformUserDesktopSoftwareSystem, "Platform User Desktop Containers", "The container diagram for the platform user desktop.");
            platformUserDesktopContainerView.Add(openSpanContainer);
            platformUserDesktopContainerView.AddNearestNeighbours(openSpanContainer);
            var relationships = platformUserDesktopContainerView.Relationships.ToList();
            foreach (var relationship in relationships)
            {
                platformUserDesktopContainerView.AddNearestNeighbours(relationship.Relationship.Destination);
            }
            platformUserDesktopContainerView.PaperSize = PaperSize.A5_Landscape;

            // Styles

            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
        }

        private static void Upload(Workspace workspace)
        {
            // Build

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
