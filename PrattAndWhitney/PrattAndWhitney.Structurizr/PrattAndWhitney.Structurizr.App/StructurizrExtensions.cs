using Structurizr;
using System.Linq;

namespace PrattAndWhitney.Structurizr.App
{
    public static class StructurizrExtensions
    {
        public static Container AddMicroserviceContainer(this SoftwareSystem softwareSystem, string name, string description = "", string technology = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var technologyToUse = string.IsNullOrWhiteSpace(technology) ? "TBD" : technology;
            var container = softwareSystem.AddContainer(name, descriptionToUse, technologyToUse);
            container.AddTags(AdditionalTags.Microservice);

            var infrastructureServices = softwareSystem.Model.SoftwareSystems.Single(x => x.Tags.Contains(AdditionalTags.InfrastructureServices));
            container.Uses(infrastructureServices, "Uses");

            return container;
        }

        public static void CreateEnterpriseContextLandscapeViewFor(this ViewSet views, Enterprise enterprise, PaperSize paperSize)
        {
            var enterpriseName = enterprise.Name;
            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView(string.Format("{0} Enterprise Context", enterpriseName), string.Format("The system landscape diagram for {0}.", enterpriseName));
            systemLandscapeEnterpriseContextView.AddAllSoftwareSystems();
            var subsystems = views.Model.SoftwareSystems.Where(x => x.Tags.Contains(AdditionalTags.Subsystem));
            foreach (var subsystem in subsystems)
            {
                systemLandscapeEnterpriseContextView.Remove(subsystem);
            }
            systemLandscapeEnterpriseContextView.PaperSize = paperSize;
        }

        public static void CreateEnterpriseContextLandscapeViewFor(this ViewSet views, Enterprise enterprise)
        {
            views.CreateEnterpriseContextLandscapeViewFor(enterprise, PaperSize.A4_Landscape);
        }

        public static void CreateSystemContextViewFor(this ViewSet views, SoftwareSystem softwareSystem, PaperSize paperSize)
        {
            var softwareSystemName = softwareSystem.Name;
            var softwareSystemContextView = views.CreateSystemContextView(softwareSystem, string.Format("{0} System Context", softwareSystemName), string.Format("The system context for {0}.", softwareSystemName));
            softwareSystemContextView.AddNearestNeighbours(softwareSystem);
            var subsystems = views.Model.SoftwareSystems.Where(x => x.Tags.Contains(AdditionalTags.Subsystem));
            foreach (var subsystem in subsystems)
            {
                softwareSystemContextView.Remove(subsystem);
            }
            softwareSystemContextView.PaperSize = paperSize;
        }

        public static void CreateSystemContextViewFor(this ViewSet views, SoftwareSystem softwareSystem)
        {
            views.CreateSystemContextViewFor(softwareSystem, PaperSize.A4_Landscape);
        }

        public static void CreateContainerViewFor(this ViewSet views, SoftwareSystem softwareSystem, PaperSize paperSize, params Container[] containersToExclude)
        {
            var softwareSystemName = softwareSystem.Name;
            var containerView = views.CreateContainerView(softwareSystem, string.Format("{0} Containers", softwareSystemName), string.Format("The container diagram for {0}.", softwareSystemName));
            foreach (var container in softwareSystem.Containers)
            {
                containerView.Add(container);
                containerView.AddNearestNeighbours(container);
            }
            foreach (var containerToExclude in containersToExclude)
            {
                containerView.Remove(containerToExclude);
            }
            containerView.PaperSize = paperSize;
        }

        public static void CreateContainerViewFor(this ViewSet views, SoftwareSystem softwareSystem, params Container[] containersToExclude)
        {
            views.CreateContainerViewFor(softwareSystem, PaperSize.A5_Landscape, containersToExclude);
        }

        public static void CreateComponentViewFor(this ViewSet views, Container container, PaperSize paperSize)
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

        public static void CreateComponentViewFor(this ViewSet views, Container container)
        {
            views.CreateComponentViewFor(container, PaperSize.A5_Landscape);
        }

        public static void ConfigureStyles(this ViewSet views)
        {
            Styles styles = views.Configuration.Styles;

            styles.Add(new ElementStyle(Tags.Element) { FontSize = 36 });

            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Component) { Background = "#85bbf0", Color = "#ffffff" });

            styles.Add(new ElementStyle(AdditionalTags.FutureState) { Border = Border.Dashed, Background = "#3cb371" });
            styles.Add(new ElementStyle(AdditionalTags.SunsetPhaseOut) { Border = Border.Dashed, Background = "#ff8c00" });
            styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(AdditionalTags.MessageBroker) { Shape = Shape.Pipe, Height = 300, Width = 750 });
            styles.Add(new ElementStyle(AdditionalTags.Gateway) { Shape = Shape.Hexagon });
            styles.Add(new ElementStyle(AdditionalTags.Files) { Shape = Shape.Folder});
            styles.Add(new ElementStyle(AdditionalTags.InfrastructureServices) { Shape = Shape.Circle });

            styles.Add(new RelationshipStyle(AdditionalTags.PotentiallyUsedRelation) { Color = "#ee7600" });
            styles.Add(new RelationshipStyle(AdditionalTags.CurrentButNotRecommendedRelation) { Color = "#ff0000" });

            //TODO
            //styles.Add(new ElementStyle(AdditionalTags.ViewSubject) { Background = "#ffa500", Color = "#ffffff", Shape = Shape.RoundedBox });
        }
    }
}
