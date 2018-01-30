using Structurizr;
using Structurizr.Api;
using System.Configuration;
using System.Linq;

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
            var clientApplicationUserPerson = model.AddPerson(Location.Internal, "Client \n Application User", "User of other client applications.");
            var vendorPerson = model.AddPerson(Location.External, "Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson(Location.Internal, "Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Automation Platform", "Ascension Automation Platform.");
            platformSoftwareSystem.AddTags(AdditionalTags.ViewSubject);

            var tableauSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Tableau", "Tableau");
            platformSoftwareSystem.Uses(tableauSoftwareSystem, "Data Visualization");

            matchExceptionProcessorPerson.Uses(platformSoftwareSystem, "Uses Platform Client Desktop");
            clientApplicationUserPerson.Uses(platformSoftwareSystem, "Uses Platform Client Desktop");
            vendorPerson.Uses(platformSoftwareSystem, "Uses Vendor Self Service Application");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses Automation Analytics Application");
            
            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Sends Actions and Embed Analytics Application");

            var clientApplicationsFrontendsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Client Applications (Frontends)", "Front ends of all client applications that use the platform.");
            matchExceptionProcessorPerson.Uses(clientApplicationsFrontendsSoftwareSystem, "Uses Match Exception Tracker");
            clientApplicationUserPerson.Uses(clientApplicationsFrontendsSoftwareSystem, "Uses Other Applications");
            clientApplicationsFrontendsSoftwareSystem.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(clientApplicationsFrontendsSoftwareSystem, "Perform Automation", "OpenSpan");

            var clientApplicationsBackendsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Client Applications (Backends)", "Back ends of all client applications that use the platform.");
            clientApplicationsFrontendsSoftwareSystem.Uses(clientApplicationsBackendsSoftwareSystem, "Application Specific Logic", "REST API");
            clientApplicationsBackendsSoftwareSystem.Uses(platformSoftwareSystem, "Decision Support", "REST API");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Primary Data Source");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Send Data And Get Insights", "REST API");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Potential Data Source").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Application Specific Data", "SSIS Job");
            ssisSoftwareSystem.Uses(clientApplicationsBackendsSoftwareSystem, "Pushes Data To Application DB", "SSIS Job");
            ssisSoftwareSystem.Uses(cortexPlatformSystem, "Push Data For Learning And Processing (Temporarily)", "REST API").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);

            var caApiManagementSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "CA API Management", "CA API Management Suite");
            platformSoftwareSystem.Uses(caApiManagementSoftwareSystem, "Gateway");
            caApiManagementSoftwareSystem.Uses(platformSoftwareSystem, "API Management");

            // Containers

            var matchExceptionTrackerFrontendContainer = clientApplicationsFrontendsSoftwareSystem.AddContainer("Match Exception Tracker Web", "The Match Exception Tracker Web Application.", "Angular");
            matchExceptionTrackerFrontendContainer.Uses(clientApplicationsBackendsSoftwareSystem, "Uses Application Specific Service", "REST API");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerFrontendContainer, "Uses", "Web Browser");
            matchExceptionTrackerFrontendContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(matchExceptionTrackerFrontendContainer, "Perform Automation", "OpenSpan");

            var matchExceptionTrackerServiceContainer = clientApplicationsBackendsSoftwareSystem.AddContainer("Match Exception Tracker App Service", "The Match Exception Tracker Application Specific Service.", "ASP.NET Core Web API");
            clientApplicationsFrontendsSoftwareSystem.Uses(matchExceptionTrackerServiceContainer, "Uses", "REST API");

            var matchExceptionTrackerDatabaseContainer = clientApplicationsBackendsSoftwareSystem.AddContainer("Match Exception Tracker Database", "The Match Exception Tracker Application Specific Database.", "SQL Server");
            matchExceptionTrackerDatabaseContainer.AddTags(AdditionalTags.Database);
            matchExceptionTrackerServiceContainer.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "SQL");
            platformSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "SQL");
            ssisSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Pushes Data To", "SQL");
            clientApplicationsFrontendsSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Query SQL Server Directly (current but not recommended)", "Datagrid SQL Connector").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);
            
            var otherClientApplicationFrontendContainer = clientApplicationsFrontendsSoftwareSystem.AddContainer("Other Client Application", "Other Client Application.", "Angular");
            otherClientApplicationFrontendContainer.Uses(clientApplicationsBackendsSoftwareSystem, "Uses Application Specific Service", "ASP.NET Core Web API");
            clientApplicationUserPerson.Uses(otherClientApplicationFrontendContainer, "Uses", "Web Browser");
            otherClientApplicationFrontendContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(otherClientApplicationFrontendContainer, "Perform Automation", "OpenSpan");

            var otherClientApplicationServiceContainer = clientApplicationsBackendsSoftwareSystem.AddContainer("Other Client Application App Service", "The Other Client Application Specific Service.", "ASP.NET Core Web API?");
            clientApplicationsFrontendsSoftwareSystem.Uses(otherClientApplicationServiceContainer, "Uses", "REST API");

            var otherClientApplicationDatabaseContainer = clientApplicationsBackendsSoftwareSystem.AddContainer("Other Client Application Database", "The Other Client Application Specific Database.", "SQL Server");
            otherClientApplicationDatabaseContainer.AddTags(AdditionalTags.Database);
            otherClientApplicationServiceContainer.Uses(otherClientApplicationDatabaseContainer, "Uses", "SQL");
            platformSoftwareSystem.Uses(otherClientApplicationDatabaseContainer, "Uses", "SQL");
            ssisSoftwareSystem.Uses(otherClientApplicationDatabaseContainer, "Pushes Data To", "SQL");

            var peopleSoftDatabaseContainer = peoplesoftSoftwareSystem.AddContainer("Peoplesoft Database", "Peoplesoft Database", "Peoplesoft");
            peopleSoftDatabaseContainer.AddTags(AdditionalTags.Database);
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Potentially Uses").AddTags(AdditionalTags.PotentiallyUsedRelation);
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(clientApplicationsBackendsSoftwareSystem, "Push Data To Application Specific DBs");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pull Data");
            ssisContainer.Uses(cortexPlatformSystem, "Push Data For Learning And Processing (Temporarily)", "REST API").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);

            var platformClientDesktopContainer = platformSoftwareSystem.AddContainer("Platform Client Desktop", "Automation Platform Client Desktop", "Windows with Pega");
            platformClientDesktopContainer.Uses(clientApplicationsFrontendsSoftwareSystem, "Perform Automation", "OpenSpan");
            clientApplicationsFrontendsSoftwareSystem.Uses(platformClientDesktopContainer, "Initiates Automation", "OpenSpan");
            matchExceptionProcessorPerson.Uses(platformClientDesktopContainer, "Uses");
            clientApplicationUserPerson.Uses(platformClientDesktopContainer, "Uses");

            var platformServicesGatewayContainer = platformSoftwareSystem.AddContainer("Platform Services Gateway", "Automation Platform Services Gateway", "CA API Gateway");
            platformServicesGatewayContainer.Uses(caApiManagementSoftwareSystem, "Gateway Features");
            caApiManagementSoftwareSystem.Uses(platformServicesGatewayContainer, "API Management");
            clientApplicationsBackendsSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses Platform Functionality", "REST API");
            clientApplicationsFrontendsSoftwareSystem.Uses(platformServicesGatewayContainer, "Could Use Platform Directly From Frontend", "REST API").AddTags(AdditionalTags.PotentiallyUsedRelation);
            platformClientDesktopContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            
            var vendorSelfServiceApplicationContainer = platformSoftwareSystem.AddPlatformApplicationContainer("Vendor Self Service Application", technology:"Angular");

            var platformAnalyticsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Analytics Service", technology: "ASP.NET Core Web API");
            platformAnalyticsServiceContainer.Uses(tableauSoftwareSystem, "Data Visualization");

            var platformManagementServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Management Service", technology: "ASP.NET Core Web API");

            var vendorServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Vendor Service", technology: "ASP.NET Core Web API");

            var machineLearningServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Machine Learning Service", technology: "ASP.NET Core Web API");
            machineLearningServiceContainer.Uses(cortexPlatformSystem, "Send Data And Get Insights", "REST API");

            var communicationsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Communications Service", "Omni/Multi Channel Communication Aggregation And Transformation", technology: "ASP.NET Core Web API");

            var dataIntegrationContainer = platformSoftwareSystem.AddContainer("Data Integration Service", "Data Integration Service", "ASP.NET Core Web API");
            dataIntegrationContainer.Uses(enterpriseDataLakeSystem, "Various Data Sources", "Data Connector");
            dataIntegrationContainer.Uses(matchExceptionTrackerDatabaseContainer, "Application specific data", "SQL Data Connector");
            dataIntegrationContainer.Uses(otherClientApplicationDatabaseContainer, "Application specific data", "SQL Data Connector");
            dataIntegrationContainer.Uses(peoplesoftSoftwareSystem, "Peoplesoft Data").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var apiServiceContainers = platformSoftwareSystem.Containers.Where(container => container.Tags.Contains(AdditionalTags.ApiService));
            foreach (var apiServiceContainer in apiServiceContainers)
            {
                platformServicesGatewayContainer.Uses(apiServiceContainer, "Exposes", "REST API");
                apiServiceContainer.Uses(dataIntegrationContainer, "Uses", "REST API");
            }

            var platformApplicationContainers = platformSoftwareSystem.Containers.Where(container => container.Tags.Contains(AdditionalTags.PlatformApplication));
            foreach (var platformApplicationContainer in platformApplicationContainers)
            {
                platformApplicationContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            }

            var pegaWorkforceIntelligenceAnalyticsApplicationContainer = pegaWorkforceIntelligenceSoftwareSystem.AddContainer("WFI Analytics Application", "WFI Analytics Application", "Web Application");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceAnalyticsApplicationContainer, "Embeds");

            var pegaWorkforceIntelligenceActionLoggerServiceContainer = pegaWorkforceIntelligenceSoftwareSystem.AddContainer("WFI Action Logging Service", "WFI Action Logging Service", "REST API");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceActionLoggerServiceContainer, "Client Desktop Action Recording");

            var pegaWorkforceIntelligenceAnalyticsDatabaseContainer = pegaWorkforceIntelligenceSoftwareSystem.AddContainer("WFI Analytics Database", "WFI Analytics Database", "DB");
            pegaWorkforceIntelligenceAnalyticsDatabaseContainer.AddTags(AdditionalTags.Database);
            pegaWorkforceIntelligenceAnalyticsApplicationContainer.Uses(pegaWorkforceIntelligenceAnalyticsDatabaseContainer, "Process Improvement and Automation Analytics");
            pegaWorkforceIntelligenceActionLoggerServiceContainer.Uses(pegaWorkforceIntelligenceAnalyticsDatabaseContainer, "Client Desktop Actions");

            var embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer = platformSoftwareSystem.AddContainer("Embedded WFI Analytics Application", "Embedded WFI Analytics Application", "Embedded Web Application");
            embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Embeds");
            automationAnalyst.Uses(embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer, "Uses");

            // Components

            var webBrowserComponent = platformClientDesktopContainer.AddComponent("Web Browser", "Web Browser", "Chrome, IE, Firefox");
            webBrowserComponent.Uses(clientApplicationsFrontendsSoftwareSystem, "Run Client Web Applications");

            var openSpanComponent = platformClientDesktopContainer.AddComponent("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "Pega Platform");
            webBrowserComponent.Uses(openSpanComponent, "Causes RDA to start and provides data", "Pega OpenSpan");
            openSpanComponent.Uses(webBrowserComponent, "Automates and sends data", "Pega OpenSpan");

            var radiloComponent = platformClientDesktopContainer.AddComponent("RADILO", "Unified Desktop.", ".NET Desktop Application");
            openSpanComponent.Uses(radiloComponent, "Automates and sends data", "Pega OpenSpan");
            radiloComponent.Uses(openSpanComponent, "Collects data, causes RDA to start and provides data", "Pega OpenSpan");

            var matchExceptionTrackerAdminViewComponent = matchExceptionTrackerFrontendContainer.AddComponent("Admin View", "Used by administrators of the Match Exception Tracker.", "Angular View Template");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerAdminViewComponent, "Uses", "Based On URL Path And Parameters");
            platformSoftwareSystem.Uses(matchExceptionTrackerAdminViewComponent, "Performs Automation", "Pega OpenSpan");

            var matchExceptionTrackerAdminControllerComponent = matchExceptionTrackerFrontendContainer.AddComponent("Admin Controller", "Used by administrators of the Match Exception Tracker.", "Angular Controller");
            matchExceptionTrackerAdminViewComponent.Uses(matchExceptionTrackerAdminControllerComponent, "Respond to actions and manage state");

            var matchExceptionTrackerAnalyticsViewComponent = matchExceptionTrackerFrontendContainer.AddComponent("Analytics View", "Used to view analytics for the Match Exception Tracker.", "Angular View Template");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerAnalyticsViewComponent, "Uses", "Based On URL Path And Parameters");
            platformSoftwareSystem.Uses(matchExceptionTrackerAnalyticsViewComponent, "Performs Automation", "Pega OpenSpan");

            var matchExceptionTrackerAnalyticsControllerComponent = matchExceptionTrackerFrontendContainer.AddComponent("Analytics Controller", "Used to view analytics for the Match Exception Tracker.", "Angular Controller");
            matchExceptionTrackerAnalyticsViewComponent.Uses(matchExceptionTrackerAnalyticsControllerComponent, "Respond to actions and manage state");

            var httpComponent = matchExceptionTrackerFrontendContainer.AddComponent("HTTP Component", "Used to communicate over HTTP.", "Angular Component");
            matchExceptionTrackerAdminControllerComponent.Uses(httpComponent, "Uses");
            matchExceptionTrackerAnalyticsControllerComponent.Uses(httpComponent, "Uses");
            httpComponent.Uses(matchExceptionTrackerServiceContainer, "Send and Receive Data", "HTTPS");
            httpComponent.Uses(platformSoftwareSystem, "Use Platform Services Directly (not recommended)", "HTTPS").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var matchExceptionTrackerServiceWorkManagementControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Work Management Controller", "Manage the work assigned to match exception processors.", "Web API Controller");
            matchExceptionTrackerServiceWorkManagementControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Work management related data and operations.", "SQL (e.g. Entity Framework)");
            matchExceptionTrackerFrontendContainer.Uses(matchExceptionTrackerServiceWorkManagementControllerComponent, "Work Managment", "HTTPS");

            var matchExceptionTrackerServiceExceptionsControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Exceptions Controller", "CRUD operations on match exceptions.", "Web API Controller");
            matchExceptionTrackerServiceExceptionsControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Match exceptions related data and operations.", "SQL (e.g. Entity Framework)");
            matchExceptionTrackerFrontendContainer.Uses(matchExceptionTrackerServiceExceptionsControllerComponent, "Match Exceptions", "HTTPS");

            var matchExceptionTrackerServiceEmailControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Email Data Controller", "Retrieve data, analytics, and visualization components for emails.", "Web API Controller");
            matchExceptionTrackerFrontendContainer.Uses(matchExceptionTrackerServiceEmailControllerComponent, "Email", "HTTPS");
            matchExceptionTrackerServiceEmailControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Email related data and operations.", "SQL (e.g. Entity Framework)");

            var platformClientInterface = matchExceptionTrackerServiceContainer.AddComponent("Platform Client Interface", "Used to interact with the Platform Services Gateway.", ".NET Class");
            matchExceptionTrackerServiceWorkManagementControllerComponent.Uses(platformClientInterface, "Interact With Platform");
            matchExceptionTrackerServiceExceptionsControllerComponent.Uses(platformClientInterface, "Interact With Platform");
            matchExceptionTrackerServiceEmailControllerComponent.Uses(platformClientInterface, "Interact With Platform");
            platformClientInterface.Uses(platformSoftwareSystem, "Decision Support And Other Platform Features", "HTTPS");

            // Views 

            var views = workspace.Views;

            // Context Views

            views.CreateEnterpriseContextLandscapeViewFor(enterprise);

            //views.CreateSystemContextViewFor(platformSoftwareSystem);

            views.CreateSystemContextViewFor(clientApplicationsFrontendsSoftwareSystem, PaperSize.A5_Portrait);

            views.CreateSystemContextViewFor(peoplesoftSoftwareSystem, PaperSize.A6_Landscape);

            // Container Views

            views.CreateContainerViewFor(platformSoftwareSystem, PaperSize.A3_Landscape);

            views.CreateContainerViewFor(clientApplicationsFrontendsSoftwareSystem);

            views.CreateContainerViewFor(clientApplicationsBackendsSoftwareSystem, PaperSize.A4_Landscape, dataIntegrationContainer);

            views.CreateContainerViewFor(pegaWorkforceIntelligenceSoftwareSystem);

            views.CreateContainerViewFor(peoplesoftSoftwareSystem);

            views.CreateContainerViewFor(ssisSoftwareSystem);

            // Component Views

            views.CreateComponentViewFor(platformClientDesktopContainer);

            views.CreateComponentViewFor(matchExceptionTrackerFrontendContainer, PaperSize.A4_Portrait);

            views.CreateComponentViewFor(matchExceptionTrackerServiceContainer, PaperSize.A4_Landscape);

            // Styles

            views.ConfigureStyles();
        }

        private static void Upload(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}
