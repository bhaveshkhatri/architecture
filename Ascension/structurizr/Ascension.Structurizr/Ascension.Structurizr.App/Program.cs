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
            var backOfficeUserPerson = model.AddPerson(Location.Internal, "Back Office \n Application User", "User of other back office applications.");
            var vendorPerson = model.AddPerson(Location.External, "Vendor", "A vendor of Ascension.");
            var automationAnalyst = model.AddPerson(Location.Internal, "Automation Analyst", "An individual that analyzes areas for automation.");

            // Software Systems

            var platformSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Automation Platform", "Ascension Automation Platform.");
            platformSoftwareSystem.AddTags(AdditionalTags.ViewSubject);

            var tableauSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Tableau", "Tableau");
            platformSoftwareSystem.Uses(tableauSoftwareSystem, "Data Visualization");

            matchExceptionProcessorPerson.Uses(platformSoftwareSystem, "Uses Platform Client Desktop");
            backOfficeUserPerson.Uses(platformSoftwareSystem, "Uses Platform Client Desktop");
            vendorPerson.Uses(platformSoftwareSystem, "Uses Vendor Self Service Application");
            automationAnalyst.Uses(platformSoftwareSystem, "Uses Automation Analytics Application");
            
            var pegaWorkforceIntelligenceSoftwareSystem = model.AddSoftwareSystem(Location.External, "Pega Workforce Intelligence", "The cloud hosted desktop activity analytics software.");
            platformSoftwareSystem.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Sends Actions and Embed Analytics Application");

            var backOfficeApplicationsFrontEndsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Back Office Applications (Front Ends)", "Front ends of all back office applications that use the platform.");
            matchExceptionProcessorPerson.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses Match Exception Tracker");
            backOfficeUserPerson.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Uses Other Applications");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Perform Automation", "OpenSpan");

            var backOfficeApplicationsBackEndsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Back Office Applications (Back Ends)", "Back ends of all back office applications that use the platform.");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Application Specific Logic", "REST API");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformSoftwareSystem, "Decision Support", "REST API");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Primary Data Source");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Send Data And Get Insights", "REST API");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Potential Data Source").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Application Specific Data", "SSIS Job");
            ssisSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Pushes Data To Application DB", "SSIS Job");
            ssisSoftwareSystem.Uses(cortexPlatformSystem, "Push Data For Learning And Processing (Temporarily)", "REST API").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);

            // Containers

            var matchExceptionTrackerFrontEndContainer = backOfficeApplicationsFrontEndsSoftwareSystem.AddContainer("Match Exception Tracker Web", "The Match Exception Tracker Web Application.", "Angular");
            matchExceptionTrackerFrontEndContainer.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Uses Application Specific Service", "REST API");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerFrontEndContainer, "Uses", "Web Browser");
            matchExceptionTrackerFrontEndContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(matchExceptionTrackerFrontEndContainer, "Perform Automation", "OpenSpan");

            var matchExceptionTrackerServiceContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Match Exception Tracker App Service", "The Match Exception Tracker Application Specific Service.", "ASP.NET Core Web API");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(matchExceptionTrackerServiceContainer, "Uses", "REST API");

            var matchExceptionTrackerDatabaseContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Match Exception Tracker Database", "The Match Exception Tracker Application Specific Database.", "SQL Server");
            matchExceptionTrackerDatabaseContainer.AddTags(AdditionalTags.Database);
            matchExceptionTrackerServiceContainer.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "SQL");
            platformSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Uses", "SQL");
            ssisSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Pushes Data To", "SQL");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Query SQL Server Directly (current but not recommended)", "Datagrid SQL Connector").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);
            
            var otherBackOfficeApplicationFrontEndContainer = backOfficeApplicationsFrontEndsSoftwareSystem.AddContainer("Other Back Office Application", "Other Back Office Application.", "Angular");
            otherBackOfficeApplicationFrontEndContainer.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Uses Application Specific Service", "ASP.NET Core Web API");
            backOfficeUserPerson.Uses(otherBackOfficeApplicationFrontEndContainer, "Uses", "Web Browser");
            otherBackOfficeApplicationFrontEndContainer.Uses(platformSoftwareSystem, "Initiate Automation", "OpenSpan");
            platformSoftwareSystem.Uses(otherBackOfficeApplicationFrontEndContainer, "Perform Automation", "OpenSpan");

            var otherBackOfficeApplicationServiceContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Other Back Office Application App Service", "The Other Back Office Application Specific Service.", "ASP.NET Core Web API?");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(otherBackOfficeApplicationServiceContainer, "Uses", "REST API");

            var otherBackOfficeApplicationDatabaseContainer = backOfficeApplicationsBackEndsSoftwareSystem.AddContainer("Other Back Office Application Database", "The Other Back Office Application Specific Database.", "SQL Server");
            otherBackOfficeApplicationDatabaseContainer.AddTags(AdditionalTags.Database);
            otherBackOfficeApplicationServiceContainer.Uses(otherBackOfficeApplicationDatabaseContainer, "Uses", "SQL");
            platformSoftwareSystem.Uses(otherBackOfficeApplicationDatabaseContainer, "Uses", "SQL");
            ssisSoftwareSystem.Uses(otherBackOfficeApplicationDatabaseContainer, "Pushes Data To", "SQL");

            var peopleSoftDatabaseContainer = peoplesoftSoftwareSystem.AddContainer("Peoplesoft Database", "Peoplesoft Database", "Peoplesoft");
            peopleSoftDatabaseContainer.AddTags(AdditionalTags.Database);
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Potentially Uses").AddTags(AdditionalTags.PotentiallyUsedRelation);
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Push Data To Application Specific DBs");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pull Data");
            ssisContainer.Uses(cortexPlatformSystem, "Push Data For Learning And Processing (Temporarily)", "REST API").AddTags(AdditionalTags.CurrentButNotRecommendedRelation);

            var platformClientDesktopContainer = platformSoftwareSystem.AddContainer("Platform Client Desktop", "Automation Platform Client Desktop", "Windows with Pega");
            platformClientDesktopContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Perform Automation", "OpenSpan");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(platformClientDesktopContainer, "Initiates Automation", "OpenSpan");
            matchExceptionProcessorPerson.Uses(platformClientDesktopContainer, "Uses");
            backOfficeUserPerson.Uses(platformClientDesktopContainer, "Uses");

            var platformServicesGatewayContainer = platformSoftwareSystem.AddContainer("Services Gateway", "Client-Specific Automation Platform Services Gateway", "CA API Gateway");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses Platform Functionality", "REST API");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(platformServicesGatewayContainer, "Could Use Platform Directly From Front End", "REST API").AddTags(AdditionalTags.PotentiallyUsedRelation);
            platformClientDesktopContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");
            
            var vendorSelfServiceApplicationContainer = platformSoftwareSystem.AddPlatformApplicationContainer("Vendor Self Service Application", technology:"Angular");

            var platformAnalyticsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Analytics Service", technology: "ASP.NET Core Web API");
            platformAnalyticsServiceContainer.Uses(tableauSoftwareSystem, "Data Visualization");

            var platformManagementServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Management Service", technology: "ASP.NET Core Web API");

            var vendorServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Vendor Service", technology: "ASP.NET Core Web API");

            var machineLearningServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Machine Learning Service", technology: "ASP.NET Core Web API");
            machineLearningServiceContainer.Uses(cortexPlatformSystem, "Send Data And Get Insights", "REST API");

            var communicationsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Communications Service", "Omni Channel Vendor (Genesys)", technology: "ASP.NET Core Web API");

            var dataIntegrationContainer = platformSoftwareSystem.AddContainer("Data Integration Service", "Data Integration Service", "ASP.NET Core Web API");
            dataIntegrationContainer.Uses(enterpriseDataLakeSystem, "Various Data Sources", "Data Connector");
            dataIntegrationContainer.Uses(matchExceptionTrackerDatabaseContainer, "Application specific data", "SQL Data Connector");
            dataIntegrationContainer.Uses(otherBackOfficeApplicationDatabaseContainer, "Application specific data", "SQL Data Connector");
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

            var pegaWorkforceIntelligenceAnalyticsDatabaseContainer = pegaWorkforceIntelligenceSoftwareSystem.AddContainer("WFI Analytics Database", "WFI Analytics Database", "DB");
            pegaWorkforceIntelligenceAnalyticsDatabaseContainer.AddTags(AdditionalTags.Database);
            pegaWorkforceIntelligenceAnalyticsApplicationContainer.Uses(pegaWorkforceIntelligenceAnalyticsDatabaseContainer, "Process Improvement and Automation Analytics");
            pegaWorkforceIntelligenceActionLoggerServiceContainer.Uses(pegaWorkforceIntelligenceAnalyticsDatabaseContainer, "Client Desktop Actions");

            var embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer = platformSoftwareSystem.AddContainer("Embedded WFI Analytics Application", "Embedded WFI Analytics Application", "Embedded Web Application");
            embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer.Uses(pegaWorkforceIntelligenceSoftwareSystem, "Embeds");
            automationAnalyst.Uses(embeddedPegaWorkforceIntelligenceAnalyticsApplicationContainer, "Uses");

            // Components

            var webBrowserComponent = platformClientDesktopContainer.AddComponent("Web Browser", "Web Browser", "Chrome, IE, Firefox");
            webBrowserComponent.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Run Client Web Applications");

            var openSpanComponent = platformClientDesktopContainer.AddComponent("OpenSpan Runtime", "Runtime environment for Pega OpenSpan.", "Pega Platform");
            webBrowserComponent.Uses(openSpanComponent, "Causes RDA to start and provides data", "Pega OpenSpan");
            openSpanComponent.Uses(webBrowserComponent, "Automates and sends data", "Pega OpenSpan");

            var radiloComponent = platformClientDesktopContainer.AddComponent("RADILO", "Unified Desktop.", ".NET Desktop Application");
            openSpanComponent.Uses(radiloComponent, "Automates and sends data", "Pega OpenSpan");
            radiloComponent.Uses(openSpanComponent, "Collects data, causes RDA to start and provides data", "Pega OpenSpan");

            var matchExceptionTrackerAdminViewComponent = matchExceptionTrackerFrontEndContainer.AddComponent("Admin View", "Used by administrators of the Match Exception Tracker.", "Angular View Template");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerAdminViewComponent, "Uses", "Based On URL Path And Parameters");
            platformSoftwareSystem.Uses(matchExceptionTrackerAdminViewComponent, "Performs Automation", "Pega OpenSpan");

            var matchExceptionTrackerAdminControllerComponent = matchExceptionTrackerFrontEndContainer.AddComponent("Admin Controller", "Used by administrators of the Match Exception Tracker.", "Angular Controller");
            matchExceptionTrackerAdminViewComponent.Uses(matchExceptionTrackerAdminControllerComponent, "Respond to actions and manage state");

            var matchExceptionTrackerAnalyticsViewComponent = matchExceptionTrackerFrontEndContainer.AddComponent("Analytics View", "Used to view analytics for the Match Exception Tracker.", "Angular View Template");
            matchExceptionProcessorPerson.Uses(matchExceptionTrackerAnalyticsViewComponent, "Uses", "Based On URL Path And Parameters");
            platformSoftwareSystem.Uses(matchExceptionTrackerAnalyticsViewComponent, "Performs Automation", "Pega OpenSpan");

            var matchExceptionTrackerAnalyticsControllerComponent = matchExceptionTrackerFrontEndContainer.AddComponent("Analytics Controller", "Used to view analytics for the Match Exception Tracker.", "Angular Controller");
            matchExceptionTrackerAnalyticsViewComponent.Uses(matchExceptionTrackerAnalyticsControllerComponent, "Respond to actions and manage state");

            var httpComponent = matchExceptionTrackerFrontEndContainer.AddComponent("HTTP Component", "Used to communicate over HTTP.", "Angular Component");
            matchExceptionTrackerAdminControllerComponent.Uses(httpComponent, "Uses");
            matchExceptionTrackerAnalyticsControllerComponent.Uses(httpComponent, "Uses");
            httpComponent.Uses(matchExceptionTrackerServiceContainer, "Send and Receive Data", "HTTPS");

            var matchExceptionTrackerServiceWorkManagementControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Work Management Controller", "Manage the work assigned to match exception processors.", "Web API Controller");
            matchExceptionTrackerServiceWorkManagementControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Work management related data and operations.", "SQL (e.g. Entity Framework)");
            matchExceptionTrackerFrontEndContainer.Uses(matchExceptionTrackerServiceWorkManagementControllerComponent, "Work Managment", "HTTPS");

            var matchExceptionTrackerServiceExceptionsControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Exceptions Controller", "CRUD operations on match exceptions.", "Web API Controller");
            matchExceptionTrackerServiceExceptionsControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Match exceptions related data and operations.", "SQL (e.g. Entity Framework)");
            matchExceptionTrackerServiceExceptionsControllerComponent.Uses(platformSoftwareSystem, "Decision Support", "HTTPS");
            matchExceptionTrackerFrontEndContainer.Uses(matchExceptionTrackerServiceExceptionsControllerComponent, "Match Exceptions", "HTTPS");

            var matchExceptionTrackerServiceEmailControllerComponent = matchExceptionTrackerServiceContainer.AddComponent("Email Data Controller", "Retrieve data, analytics, and visualization components for emails.", "Web API Controller");
            matchExceptionTrackerFrontEndContainer.Uses(matchExceptionTrackerServiceEmailControllerComponent, "Email", "HTTPS");
            matchExceptionTrackerServiceEmailControllerComponent.Uses(matchExceptionTrackerDatabaseContainer, "Email related data and operations.", "SQL (e.g. Entity Framework)");

            // Views 

            var views = workspace.Views;

            // Context Views

            views.CreateEnterpriseContextLandscapeViewFor(enterprise);

            //views.CreateSystemContextViewFor(platformSoftwareSystem);

            views.CreateSystemContextViewFor(backOfficeApplicationsFrontEndsSoftwareSystem);

            views.CreateSystemContextViewFor(peoplesoftSoftwareSystem);

            // Container Views

            views.CreateContainerViewFor(platformSoftwareSystem, PaperSize.A3_Landscape);

            views.CreateContainerViewFor(backOfficeApplicationsFrontEndsSoftwareSystem);

            views.CreateContainerViewFor(backOfficeApplicationsBackEndsSoftwareSystem, PaperSize.A4_Landscape, dataIntegrationContainer);

            views.CreateContainerViewFor(pegaWorkforceIntelligenceSoftwareSystem);

            views.CreateContainerViewFor(peoplesoftSoftwareSystem);

            views.CreateContainerViewFor(ssisSoftwareSystem);

            // Component Views

            views.CreateComponentViewFor(platformClientDesktopContainer);

            views.CreateComponentViewFor(matchExceptionTrackerFrontEndContainer, PaperSize.A3_Landscape);

            views.CreateComponentViewFor(matchExceptionTrackerServiceContainer);

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
