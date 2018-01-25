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
            platformSoftwareSystem.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Execute Automation", "OpenSpan");

            var backOfficeApplicationsBackEndsSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Back Office Applications (Back Ends)", "Back ends of all back office applications that use the platform.");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Application Specific Logic", "REST API");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformSoftwareSystem, "Decision Support", "REST API");

            var enterpriseDataLakeSystem = model.AddSoftwareSystem(Location.Internal, "Enterprise Data Lake", "Enterprise Data Lake.");
            platformSoftwareSystem.Uses(enterpriseDataLakeSystem, "Uses");

            var cortexPlatformSystem = model.AddSoftwareSystem(Location.External, "Cortex V5", "Cognitive Scale Cortex Platform.");
            platformSoftwareSystem.Uses(cortexPlatformSystem, "Send Data And Get Insights", "REST API");

            var peoplesoftSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "Peoplesoft", "Peoplesoft");
            platformSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Potential Data Source").AddTags(AdditionalTags.PotentiallyUsedRelation);

            var ssisSoftwareSystem = model.AddSoftwareSystem(Location.Internal, "SSIS", "SQL Server Integration Services");
            ssisSoftwareSystem.Uses(peoplesoftSoftwareSystem, "Pulls Application Specific Data", "SSIS Job");
            ssisSoftwareSystem.Uses(backOfficeApplicationsBackEndsSoftwareSystem, "Pushes Data To Application DB", "SSIS Job");
            ssisSoftwareSystem.Uses(cortexPlatformSystem, "Push Data For Learning And Processing", "REST API");

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
            platformSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Potentially Uses", "TBD").AddTags(AdditionalTags.PotentiallyUsedRelation);
            ssisSoftwareSystem.Uses(peopleSoftDatabaseContainer, "Pulls Data From", "TBD");

            var ssisContainer = ssisSoftwareSystem.AddContainer("SSIS", "SQL Server Integration Services", "SSIS");
            ssisContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Push Data To Application Specific DBs");
            ssisContainer.Uses(peoplesoftSoftwareSystem, "Pull Data");
            ssisContainer.Uses(cortexPlatformSystem, "Push Data For Learning And Processing", "REST API");

            var platformClientDesktopContainer = platformSoftwareSystem.AddContainer("Platform Client Desktop", "Automation Platform Client Desktop", "Windows with Pega");
            platformClientDesktopContainer.Uses(backOfficeApplicationsFrontEndsSoftwareSystem, "Execute Automation", "OpenSpan");
            backOfficeApplicationsFrontEndsSoftwareSystem.Uses(platformClientDesktopContainer, "Initiates Automation", "OpenSpan");

            var platformServicesGatewayContainer = platformSoftwareSystem.AddContainer("Services Gateway", "Automation Platform Services Gateway", "ASP.NET Core Web API");
            backOfficeApplicationsBackEndsSoftwareSystem.Uses(platformServicesGatewayContainer, "Uses Platform Functionality", "REST API");
            platformClientDesktopContainer.Uses(platformServicesGatewayContainer, "Uses", "REST API");

            var vendorSelfServiceApplicationContainer = platformSoftwareSystem.AddPlatformApplicationContainer("Vendor Self Service Application", technology:"Angular");
            var platformAnalyticsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Analytics Service", technology: "ASP.NET Core Web API");
            var platformManagementServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Platform Management Service", technology: "ASP.NET Core Web API");
            var vendorServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Vendor Service", technology: "ASP.NET Core Web API");
            var machineLearningServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Machine Learning Service", technology: "ASP.NET Core Web API");
            var communicationsServiceContainer = platformSoftwareSystem.AddApiServiceContainer("Communications Service", "Omni Channel Vendor (Genesys)", technology: "ASP.NET Core Web API");

            var dataIntegrationContainer = platformSoftwareSystem.AddContainer("Data Integration Service", "Data Integration Service", "ASP.NET Core Web API");
            dataIntegrationContainer.Uses(enterpriseDataLakeSystem, "Various Data Sources", "Data Connector");
            dataIntegrationContainer.Uses(matchExceptionTrackerDatabaseContainer, "Application specific data", "SQL Data Connector");
            dataIntegrationContainer.Uses(otherBackOfficeApplicationDatabaseContainer, "Application specific data", "SQL Data Connector");
            dataIntegrationContainer.Uses(peoplesoftSoftwareSystem, "Peoplesoft Data").AddTags(AdditionalTags.PotentiallyUsedRelation);

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
            webBrowserComponent.Uses(openSpanComponent, "Triggers RDA and provides data", "Pega OpenSpan");
            openSpanComponent.Uses(webBrowserComponent, "Automates", "Pega OpenSpan");

            var radiloComponent = platformClientDesktopContainer.AddComponent("RADILO", "Unified Desktop.", ".NET Desktop Application");
            openSpanComponent.Uses(radiloComponent, "Automates", "Pega OpenSpan");
            radiloComponent.Uses(openSpanComponent, "Collects data, triggers RDA and provides data", "Pega OpenSpan");

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
