using Structurizr;
using Structurizr.Api;
using System.Configuration;
using System.Linq;
using System;

namespace Ascension.Structurizr.App
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

            var workspace = new Workspace("Ascension", "Model of the Automation Platform.");

            Build(workspace);

            Upload(workspace);
        }

        private static void Build(Workspace workspace)
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
            matchExceptionProcessorPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");
            backOfficeUserPerson.Uses(platformUserDesktopSoftwareSystem, "Uses");
            vendorPerson.Uses(platformSoftwareSystem, "Uses", "Vendor Self Service Application");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses", "Analytics Web Application");
            platformUserDesktopSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "REST API");

            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Uses", "Embeds Application");

            var matchExceptionTrackerSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Match Exception Tracker", "Tracks and manages match exceptions and related work.");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerSoftwareSystem, "Uses");
            matchExceptionTrackerSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "REST API");
            platformUserDesktopSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Uses", "OS");

            var otherBackOfficeSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Other Back Office Application", "Tracks and manages match other back office related work.");
            backOfficeUserPerson.Uses(otherBackOfficeSoftwareSystem, "Uses");
            otherBackOfficeSoftwareSystem.Uses(platformSoftwareSystem, "Uses", "REST API");
            platformUserDesktopSoftwareSystem.Uses(otherBackOfficeSoftwareSystem, "Uses", "OS");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Uses");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Uses", "API Calls");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Potentially Uses", "TBD");

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "Nightly Job");
            ssisSoftwareSystem.Uses(matchExceptionTrackerSoftwareSystem, "Pushes Data To", "Nightly Job");

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
            ssisSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Pushes Data To", "TBD");

            var peopleSoftDatabaseContainer = peoplesoftSoftwareSystem.AddContainer("Peoplesoft Database", "Peoplesoft Database", "Peoplesoft");
            peopleSoftDatabaseContainer.AddTags(AdditionalTags.Database);
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Uses", "TBD");
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From", "TBD");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(matchExceptionTrackerSoftwareSystem, "Pushes Data To", "TBD");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "TBD");

            var platformServicesGatewayContainer = platformSoftwareSystem.AddContainer("Services Gateway", "Automation Platform Services Gateway", "TBD");
            matchExceptionTrackerSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            otherBackOfficeSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            platformUserDesktopSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses", "REST API");

            var vendorSelfServiceApplicationContainer = platformSoftwareSystem.AddPlatformApplicationContainer("Vendor Self Service Application", technology:"TBD - Angular?");
            var platformAnalyticsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Analytics Service");
            var platformManagementServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Management Service");
            var vendorServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Vendor Service");
            var machineLearningServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Machine Learning Service");
            var omniChannelCommunicationsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Omni Channel Communications Service");

            var dataIntegrationContainer = platformSoftwareSystem.AddContainer("Data Integration Service", "Data Integration Service", "TBD");

            var apiServiceContainers = platformSoftwareSystem.Containers.Where(container => container.Tags.Contains(AdditionalTags.ApiService));
            foreach (var apiServiceContainer in apiServiceContainers)
            {
                platformServicesGatewayContainer.Uses(apiServiceContainer, "Exposes", "TBD");
                apiServiceContainer.Uses(dataIntegrationContainer, "Uses", "TBD");
            }

            var platformApplicationContainers = platformSoftwareSystem.Containers.Where(container => container.Tags.Contains(AdditionalTags.PlatformApplication));
            foreach (var platformApplicationContainer in platformApplicationContainers)
            {
                platformApplicationContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            }

            // Views 

            var views = workspace.Views;

            // Context Views

            CreateEnterpriseContextLandscapeViewFor(enterprise, views);

            CreateSystemContextViewFor(platformSoftwareSystem, views);

            CreateSystemContextViewFor(platformUserDesktopSoftwareSystem, views);

            CreateSystemContextViewFor(matchExceptionTrackerSoftwareSystem, views);

            CreateSystemContextViewFor(peoplesoftSoftwareSystem, views);

            // Container Views

            CreateContainerViewFor(platformSoftwareSystem, views, PaperSize.A4_Landscape);

            CreateContainerViewFor(platformUserDesktopSoftwareSystem, views);

            CreateContainerViewFor(matchExceptionTrackerSoftwareSystem, views);

            CreateContainerViewFor(peoplesoftSoftwareSystem, views);

            CreateContainerViewFor(ssisSoftwareSystem, views);

            // Styles

            ConfigureStylesIn(views);
        }

        private static void CreateEnterpriseContextLandscapeViewFor(Enterprise enterprise, ViewSet views)
        {
            var enterpriseName = enterprise.Name;
            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView(string.Format("{0} Enterprise Context", enterpriseName), string.Format("The system landscape diagram for {0}.", enterpriseName));
            systemLandscapeEnterpriseContextView.AddAllElements();
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;
        }

        private static void CreateSystemContextViewFor(SoftwareSystem softwareSystem, ViewSet views)
        {
            var softwareSystemName = softwareSystem.Name;
            var softwareSystemContextView = views.CreateSystemContextView(softwareSystem, string.Format("{0} System Context", softwareSystemName), string.Format("The system context for {0}.", softwareSystemName));
            softwareSystemContextView.AddNearestNeighbours(softwareSystem);
            softwareSystemContextView.PaperSize = PaperSize.A4_Landscape;
        }

        private static void CreateContainerViewFor(SoftwareSystem softwareSystem, ViewSet views, PaperSize paperSize)
        {
            var softwareSystemName = softwareSystem.Name;
            var containerView = views.CreateContainerView(softwareSystem, string.Format("{0} Containers", softwareSystemName), string.Format("The container diagram for {0}.", softwareSystemName));
            foreach (var container in softwareSystem.Containers)
            {
                containerView.Add(container);
                containerView.AddNearestNeighbours(container);
            }
            containerView.PaperSize = paperSize;
        }

        private static void CreateContainerViewFor(SoftwareSystem softwareSystem, ViewSet views)
        {
            CreateContainerViewFor(softwareSystem, views, PaperSize.A5_Landscape);
        }

        private static void ConfigureStylesIn(ViewSet views)
        {
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(AdditionalTags.Queue) { Shape = Shape.Pipe });
        }

        private static void Upload(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
