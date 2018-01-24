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
            
            switch (CurrentVersion)
            {
                case 1:
                    BuildV1(workspace);
                    break;
                case 2:
                    BuildV2(workspace);
                    break;
                default:
                    break;
            }

            Upload(workspace);
        }

        private static void BuildV1(Workspace workspace)
        {
            var model = workspace.Model;

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

            ConfigureStyles(views);
        }

        private static void BuildV2(Workspace workspace)
        {
            var model = workspace.Model;

            // Enterprise

            var enterprise = model.Enterprise = new Enterprise("Ascension");

            // Users
            var matchExceptionProcessorPerson = model.AddPerson(Location.Internal, "Match Exception Processor", "User that processes match exceptions.");
            var backOfficeUserPerson = model.AddPerson(Location.Internal, "Back Office \n Application User", "User of other back office applications.");
            var vendorPerson = model.AddPerson(Location.External, "Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson(Location.Internal, "Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Automation Platform", "Ascension Automation Platform.");

            var platformUserDesktopSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Platform User Desktop", "The desktop of an automation platform end user.");
            matchExceptionProcessorPerson.Uses(platformUserDesktopSoftwareSystem, "Uses", "TBD");
            backOfficeUserPerson.Uses(platformUserDesktopSoftwareSystem, "Uses", "TBD");
            vendorPerson.Uses(platformSoftwareSystem, "Uses", "TBD");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses", "TBD");
            platformUserDesktopSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "TBD");

            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Uses", "TBD");

            var matchExceptionTrackerSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Match Exception Tracker", "Tracks and manages match exceptions and related work.");
            matchExceptionTrackerSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "TBD");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerSoftwareSystem, "Uses", "TBD");
            platformUserDesktopSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Uses", "TBD");

            var otherBackOfficeSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Other Back Office Application", "Tracks and manages match other back office related work.");
            backOfficeUserPerson.Uses(otherBackOfficeSoftwareSystem, "Uses", "TBD");
            otherBackOfficeSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "TBD");
            platformUserDesktopSoftwareSystem.Uses(otherBackOfficeSoftwareSystem, "Uses", "TBD");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Uses", "TBD");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Uses", "TBD");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Uses", "TBD");

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "TBD");
            ssisSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Pushes Data To", "TBD");

            // Containers

            var webBrowserContainer = platformUserDesktopSoftwareSystem.AddContainer("Web Browser", "Web Browser (e.g. Chrome, IE, Firefox).", "TBD");
            webBrowserContainer.Uses(matchExceptionTrackerSoftwareSystem, "Uses", "TBD");
            webBrowserContainer.Uses(otherBackOfficeSoftwareSystem, "Uses", "TBD");

            var openSpanContainer = platformUserDesktopSoftwareSystem.AddContainer("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "TBD");
            webBrowserContainer.Uses(openSpanContainer, "Triggers RDA and provides data", "TBD");
            openSpanContainer.Uses(webBrowserContainer, "Automates", "TBD");

            var radiloContainer = platformUserDesktopSoftwareSystem.AddContainer("RADILO", "Unified Desktop.", "TBD");
            openSpanContainer.Uses(radiloContainer, "Automates", "TBD");
            radiloContainer.Uses(openSpanContainer, "Collects data, triggers RDA and provides data", "TBD");

            var matchExceptionTrackerWebApplicationContainer = matchExceptionTrackerSoftwareSystem.AddContainer("Match Exception Tracker Web", "The Match Exception Tracker Web Application.", "TBD - Angular?");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerWebApplicationContainer, "Uses", "TBD");
            var matchExceptionTrackerServiceContainer = matchExceptionTrackerSoftwareSystem.AddContainer("Match Exception Tracker App Service", "The Match Exception Tracker Application Specific Service.", "TBD - .NET Core Web API?");
            matchExceptionTrackerWebApplicationContainer.Uses(matchExceptionTrackerServiceContainer, "Uses", "TBD");
            matchExceptionTrackerWebApplicationContainer.Uses(platformSoftwareSystem, "Uses", "TBD");
            var matchExceptionTrackerDatabaseContainer = matchExceptionTrackerSoftwareSystem.AddContainer("Match Exception Tracker Database", "The Match Exception Tracker Application Specific Database.", "SQL Server");
            matchExceptionTrackerDatabaseContainer.AddTags(AdditionalTags.Database);
            matchExceptionTrackerServiceContainer.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "TBD");
            platformSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "TBD");

            var peopleSoftDatabaseContainer = peoplesoftSoftwareSystem.AddContainer("Peoplesoft Database", "Peoplesoft Database", "Peoplesoft");
            peopleSoftDatabaseContainer.AddTags(AdditionalTags.Database);
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Uses", "TBD");
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From", "TBD");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(matchExceptionTrackerSoftwareSystem, "Pushes Data To", "TBD");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "TBD");

            // Views 

            var views = workspace.Views;

            // Context Views

            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView("Enterprise Context", "The system landscape diagram for Ascension Automation Platform.");
            systemLandscapeEnterpriseContextView.AddAllElements();
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;

            var platformSystemContextView = views.CreateSystemContextView(platformSoftwareSystem, "Automation Platform System Context", "The system context for the Automation Platform.");
            platformSystemContextView.AddNearestNeighbours(platformSoftwareSystem);
            platformSystemContextView.PaperSize = PaperSize.A4_Landscape;

            var platformUserDesktopSystemContextView = views.CreateSystemContextView(platformUserDesktopSoftwareSystem, "Platform User Desktop System Context", "The system context for the platform user desktop.");
            platformUserDesktopSystemContextView.AddNearestNeighbours(platformUserDesktopSoftwareSystem);
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;

            var matchExceptionTrackerSystemContextView = views.CreateSystemContextView(matchExceptionTrackerSoftwareSystem, "Match Exception Tracker System Context", "The system context for the Match Exception Tracker");
            matchExceptionTrackerSystemContextView.AddNearestNeighbours(matchExceptionTrackerSoftwareSystem);
            matchExceptionTrackerSystemContextView.PaperSize = PaperSize.A4_Landscape;

            var peoplesoftSystemContextView = views.CreateSystemContextView(peoplesoftSoftwareSystem, "Peoplesoft System Context", "The system context for Peoplesoft.");
            peoplesoftSystemContextView.AddNearestNeighbours(peoplesoftSoftwareSystem);
            peoplesoftSystemContextView.PaperSize = PaperSize.A4_Landscape;

            // Container Views

            var platformContainerView = views.CreateContainerView(platformSoftwareSystem, "Automation Platform Containers", "The container diagram for the Automation Platform.");
            foreach (var container in platformSoftwareSystem.Containers)
            {
                platformContainerView.Add(container);
                platformContainerView.AddNearestNeighbours(container);
            }
            platformContainerView.PaperSize = PaperSize.A5_Landscape;

            var platformUserDesktopContainerView = views.CreateContainerView(platformUserDesktopSoftwareSystem, "Platform User Desktop Containers", "The container diagram for the platform user desktop.");
            platformUserDesktopContainerView.Add(openSpanContainer);
            platformUserDesktopContainerView.AddNearestNeighbours(openSpanContainer);
            var relationships = platformUserDesktopContainerView.Relationships.ToList();
            foreach (var relationship in relationships)
            {
                platformUserDesktopContainerView.AddNearestNeighbours(relationship.Relationship.Destination);
            }
            platformUserDesktopContainerView.PaperSize = PaperSize.A5_Landscape;

            var matchExceptionTrackerContainerView = views.CreateContainerView(matchExceptionTrackerSoftwareSystem, "Match Exception Tracker Containers", "The container diagram for the Match Exception Tracker.");
            foreach (var container in matchExceptionTrackerSoftwareSystem.Containers)
            {
                matchExceptionTrackerContainerView.Add(container);
                matchExceptionTrackerContainerView.AddNearestNeighbours(container);
            }
            matchExceptionTrackerContainerView.PaperSize = PaperSize.A5_Landscape;

            var peoplesoftContainerView = views.CreateContainerView(peoplesoftSoftwareSystem, "Peoplesoft Containers", "The container diagram for Peoplesoft.");
            foreach (var container in peoplesoftSoftwareSystem.Containers)
            {
                peoplesoftContainerView.Add(container);
                peoplesoftContainerView.AddNearestNeighbours(container);
            }
            peoplesoftContainerView.PaperSize = PaperSize.A5_Landscape;

            var ssisContainerView = views.CreateContainerView(ssisSoftwareSystem, "SSIS Containers", "The container diagram for SSIS.");
            foreach (var container in ssisSoftwareSystem.Containers)
            {
                ssisContainerView.Add(container);
                ssisContainerView.AddNearestNeighbours(container);
            }
            ssisContainerView.PaperSize = PaperSize.A5_Landscape;

            // Styles

            ConfigureStyles(views);
        }

        private static void ConfigureStyles(ViewSet views)
        {
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(AdditionalTags.Queue) { Shape = Shape.Pipe });
        }

        private static void Upload(Workspace workspace)
        {
            // Build

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }

        private static class AdditionalTags
        {
            public static string Database
            {
                get
                {
                    return "Database";
                }
            }
            public static string Queue
            {
                get
                {
                    return "Queue";
                }
            }
        }
    }
}
