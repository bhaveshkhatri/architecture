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
            platformSoftwareSystem.AddTags(AdditionalTags.ViewSubject);

            matchExceptionProcessorPerson.Uses(platformSoftwareSystem, "Uses Client Desktop");
            backOfficeUserPerson.Uses(platformSoftwareSystem, "Uses Client Desktop");
            vendorPerson.Uses(platformSoftwareSystem, "Uses", "Vendor Self Service Application");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses", "Analytics Web Application");
            
            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Uses", "Embeds Application");

            var backOfficeApplicationsFrontEndsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Back Office Applications (Front Ends)", "Front ends of all back office applications that use the platform.");
            matchExceptionProcessorPerson.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses");
            backOfficeUserPerson.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Execute Automation", "OpenSpan");

            var backOfficeApplicationsBackEndsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Back Office Applications (Back Ends)", "Back ends of all back office applications that use the platform.");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Uses", "REST API");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformSoftwareSystem, "Decision Support", "REST API");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Uses");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Uses", "API Calls");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Potentially Uses", "TBD").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "Nightly Job");
            ssisSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Pushes Data To Application DB", "Nightly Job");

            // Containers

            var matchExceptionTrackerFrontEndContainer = backOfficeApplicationsFrontEndsSoftwareSystem.AddContainer("Match Exception Tracker Web", "The Match Exception Tracker Web Application.", "TBD - Angular?");
            matchExceptionTrackerFrontEndContainer.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Uses Application Specific Service", "TBD");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerFrontEndContainer, "Uses", "TBD");
            matchExceptionTrackerFrontEndContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(matchExceptionTrackerFrontEndContainer, "Execute Automation", "OpenSpan");

            var matchExceptionTrackerServiceContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Match Exception Tracker App Service", "The Match Exception Tracker Application Specific Service.", "TBD - .NET Core Web API?");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(matchExceptionTrackerServiceContainer, "Uses", "TBD");

            var matchExceptionTrackerDatabaseContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Match Exception Tracker Database", "The Match Exception Tracker Application Specific Database.", "SQL Server");
            matchExceptionTrackerDatabaseContainer.AddTags(AdditionalTags.Database);
            matchExceptionTrackerServiceContainer.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "TBD");
            platformSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "TBD");
            ssisSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Pushes Data To", "TBD");
            
            var otherBackOfficeApplicationFrontEndContainer = backOfficeApplicationsFrontEndsSoftwareSystem.AddContainer("Other Back Office Application", "Other Back Office Application.", "TBD - Angular?");
            otherBackOfficeApplicationFrontEndContainer.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Uses Application Specific Service", "TBD");
            backOfficeUserPerson.Uses(otherBackOfficeApplicationFrontEndContainer, "Uses", "TBD");
            otherBackOfficeApplicationFrontEndContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(otherBackOfficeApplicationFrontEndContainer, "Execute Automation", "OpenSpan");

            var otherBackOfficeApplicationServiceContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Other Back Office Application App Service", "The Other Back Office Application Specific Service.", "TBD - .NET Core Web API?");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(otherBackOfficeApplicationServiceContainer, "Uses", "TBD");

            var otherBackOfficeApplicationDatabaseContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Other Back Office Application Database", "The Other Back Office Application Specific Database.", "SQL Server");
            otherBackOfficeApplicationDatabaseContainer.AddTags(AdditionalTags.Database);
            otherBackOfficeApplicationServiceContainer.Uses(otherBackOfficeApplicationDatabaseContainer, "Uses", "TBD");
            platformSoftwareSystem.Uses(otherBackOfficeApplicationDatabaseContainer, "Uses", "TBD");
            ssisSoftwareSystem.Uses(otherBackOfficeApplicationDatabaseContainer, "Pushes Data To", "TBD");

            var peopleSoftDatabaseContainer = peoplesoftSoftwareSystem.AddContainer("Peoplesoft Database", "Peoplesoft Database", "Peoplesoft");
            peopleSoftDatabaseContainer.AddTags(AdditionalTags.Database);
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Uses", "TBD");
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From", "TBD");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Pushes Data To Application DB of", "TBD");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pulls Data From", "TBD");

            var platformClientDesktopContainer = platformSoftwareSystem.AddContainer("Platform Client Desktop", "Automation Platform Client Desktop", "TBD");
            platformClientDesktopContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses", "OS");

            var platformServicesGatewayContainer = platformSoftwareSystem.AddContainer("Services Gateway", "Automation Platform Services Gateway", "TBD");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            platformClientDesktopContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");

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

            // Components
            
            var webBrowserComponent = platformClientDesktopContainer.AddComponent("Web Browser", "Web Browser (e.g. Chrome, IE, Firefox).", "TBD");
            webBrowserComponent.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses", "TBD");

            var openSpanComponent = platformClientDesktopContainer.AddComponent("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "TBD");
            webBrowserComponent.Uses(openSpanComponent, "Triggers RDA and provides data", "TBD");
            openSpanComponent.Uses(webBrowserComponent, "Automates");

            var radiloComponent = platformClientDesktopContainer.AddComponent("RADILO", "Unified Desktop.", "TBD");
            openSpanComponent.Uses(radiloComponent, "Automates");
            radiloComponent.Uses(openSpanComponent, "Collects data, triggers RDA and provides data", "TBD");

            // Views 

            var views = workspace.Views;

            // Context Views

            CreateEnterpriseContextLandscapeViewFor(enterprise, views);

            CreateSystemContextViewFor(platformSoftwareSystem, views);

            CreateSystemContextViewFor(backOfficeApplicationsFrontEndsSoftwareSystem, views);

            CreateSystemContextViewFor(peoplesoftSoftwareSystem, views);

            // Container Views

            CreateContainerViewFor(platformSoftwareSystem, views, PaperSize.A4_Landscape);

            CreateContainerViewFor(backOfficeApplicationsFrontEndsSoftwareSystem, views);

            CreateContainerViewFor(backOfficeApplicationsBackEndsSoftwareSystem, views, PaperSize.A4_Landscape);

            CreateContainerViewFor(peoplesoftSoftwareSystem, views);

            CreateContainerViewFor(ssisSoftwareSystem, views);

            // Component Views

            CreateComponentViewFor(platformClientDesktopContainer, views);

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

        private static void CreateComponentViewFor(Container container, ViewSet views, PaperSize paperSize)
        {
            var containerName = container.Name;
            var containerView = views.CreateComponentView(container, string.Format("{0} Components", containerName), string.Format("The component diagram for {0}.", containerName));
            foreach (var component in container.Components)
            {
                containerView.Add(component);
                containerView.AddNearestNeighbours(component);
            }
            containerView.PaperSize = paperSize;
        }

        private static void CreateComponentViewFor(Container container, ViewSet views)
        {
            CreateComponentViewFor(container, views, PaperSize.A5_Landscape);
        }

        private static void ConfigureStylesIn(ViewSet views)
        {
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(AdditionalTags.Queue) { Shape = Shape.Pipe });
            styles.Add(new RelationshipStyle(AdditionalTags.PotentiallyUsedRelation) { Color = "#ffa500" });

            //TODO
            //styles.Add(new ElementStyle(AdditionalTags.ViewSubject) { Background = "#ffa500", Color = "#ffffff", Shape = Shape.RoundedBox });
        }

        private static void Upload(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
