using Structurizr;
using Structurizr.Api;
using System.Configuration;
using System;

namespace Ascension.Structurizr.App
{
    public class Program
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
            ssisSoftwareSystem.Uses(matchExceptionTrackerDatabaseContainer, "Pushes Data To", "TBD");

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

            CreateEnterpriseContextLandscapeViewFor(enterprise, views);

            CreateSystemContextViewFor(platformSoftwareSystem, views);

            CreateSystemContextViewFor(platformUserDesktopSoftwareSystem, views);

            CreateSystemContextViewFor(matchExceptionTrackerSoftwareSystem, views);

            CreateSystemContextViewFor(peoplesoftSoftwareSystem, views);

            // Container Views

            CreateContainerViewFor(platformSoftwareSystem, views);

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

        private static void CreateContainerViewFor(SoftwareSystem softwareSystem, ViewSet views)
        {
            var softwareSystemName = softwareSystem.Name;
            var containerView = views.CreateContainerView(softwareSystem, string.Format("{0} Containers", softwareSystemName), string.Format("The container diagram for {0}.", softwareSystemName));
            foreach (var container in softwareSystem.Containers)
            {
                containerView.Add(container);
                containerView.AddNearestNeighbours(container);
            }
            containerView.PaperSize = PaperSize.A5_Landscape;
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
