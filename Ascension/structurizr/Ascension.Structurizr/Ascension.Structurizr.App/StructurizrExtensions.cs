using Structurizr;

namespace Ascension.Structurizr.App
{
    public static class StructurizrExtensions
    {
        public static Container AddPlatformApplicationContainer(this SoftwareSystem softwareSystem, string name, string description = "", string technology = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var technologyToUse = string.IsNullOrWhiteSpace(technology) ? "TBD" : technology;
            var container = softwareSystem.AddContainer(name, descriptionToUse, technologyToUse);
            container.AddTags(AdditionalTags.PlatformApplication);

            return container;
        }

        public static Container AddApiServiceContainer(this SoftwareSystem softwareSystem, string name, string description = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var container = softwareSystem.AddContainer(name, descriptionToUse, "TBD");
            container.AddTags(AdditionalTags.ApiService);

            return container;
        }

        public static void CreateEnterpriseContextLandscapeViewFor(this ViewSet views, Enterprise enterprise)
        {
            var enterpriseName = enterprise.Name;
            var systemLandscapeEnterpriseContextView = views.CreateEnterpriseContextView(string.Format("{0} Enterprise Context", enterpriseName), string.Format("The system landscape diagram for {0}.", enterpriseName));
            systemLandscapeEnterpriseContextView.AddAllElements();
            systemLandscapeEnterpriseContextView.PaperSize = PaperSize.A4_Landscape;
        }

        public static void CreateSystemContextViewFor(this ViewSet views, SoftwareSystem softwareSystem)
        {
            var softwareSystemName = softwareSystem.Name;
            var softwareSystemContextView = views.CreateSystemContextView(softwareSystem, string.Format("{0} System Context", softwareSystemName), string.Format("The system context for {0}.", softwareSystemName));
            softwareSystemContextView.AddNearestNeighbours(softwareSystem);
            softwareSystemContextView.PaperSize = PaperSize.A4_Landscape;
        }

        public static void CreateContainerViewFor(this ViewSet views, SoftwareSystem softwareSystem, PaperSize paperSize)
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

        public static void CreateContainerViewFor(this ViewSet views, SoftwareSystem softwareSystem)
        {
            views.CreateContainerViewFor(softwareSystem, PaperSize.A5_Landscape);
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
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(AdditionalTags.Queue) { Shape = Shape.Pipe });
            styles.Add(new RelationshipStyle(AdditionalTags.PotentiallyUsedRelation) { Color = "#ffa500" });

            //TODO
            //styles.Add(new ElementStyle(AdditionalTags.ViewSubject) { Background = "#ffa500", Color = "#ffffff", Shape = Shape.RoundedBox });
        }
    }
}
