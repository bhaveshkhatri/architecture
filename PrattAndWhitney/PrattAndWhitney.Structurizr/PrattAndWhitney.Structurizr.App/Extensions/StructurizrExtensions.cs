using Structurizr;
using System.Collections.Generic;
using System.Linq;

namespace PrattAndWhitney.Structurizr.App.Extensions
{
    public static class StructurizrExtensions
    {
        public static bool IsBetween(this RelationshipView relationshipView, string firstTag, string secondTag)
        {
            var source = relationshipView.Relationship.Source.Tags;
            var destination = relationshipView.Relationship.Destination.Tags;
            var forward = source.Contains(firstTag) && destination.Contains(secondTag);
            var reverse = source.Contains(secondTag) && destination.Contains(firstTag);
            return forward || reverse;
        }

        public static Component AddApiComponent(this Container container, string name, string description = "", string technology = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var technologyToUse = string.IsNullOrWhiteSpace(technology) ? "TBD-Web API Controller" : technology;
            var component = container.AddComponent(name, descriptionToUse, technologyToUse);

            return component;
        }

        public static Container AddMicroserviceContainer(this SoftwareSystem softwareSystem, string name, string description = "", string technology = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var technologyToUse = string.IsNullOrWhiteSpace(technology) ? "TBD-.NET Core" : technology;
            var container = softwareSystem.AddContainer(name, descriptionToUse, technologyToUse);
            container.AddTags(AdditionalTags.Microservice);

            var infrastructureServices = softwareSystem.Model.SoftwareSystems.Single(x => x.Tags.Contains(AdditionalTags.InfrastructureServices));
            container.Uses(infrastructureServices, "Message Transfer, Cache Access");

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
            var componentView = views.CreateComponentView(container, string.Format("{0} Components", containerName), string.Format("The component diagram for {0}.", containerName));
            foreach (var component in container.Components)
            {
                componentView.Add(component);
                componentView.AddNearestNeighbours(component);
            }
            //TODO
            //componentView.Relationships.RemoveWhere(x => x.IsBetween(Tags.SoftwareSystem, Tags.Person));
            componentView.PaperSize = paperSize;
        }

        public static void CreateComponentViewFor(this ViewSet views, Container container)
        {
            views.CreateComponentViewFor(container, PaperSize.A5_Landscape);
        }

        public static void CreateDeploymentViewFor(this ViewSet views, SoftwareSystem primarySoftwareSystem, PaperSize paperSize, params SoftwareSystem[] subsystems)
        {
            var deploymentView = views.CreateDeploymentView(primarySoftwareSystem, string.Format("{0} Deployment", primarySoftwareSystem.Name), string.Format("The deployment for {0}.", primarySoftwareSystem.Name));
            deploymentView.PaperSize = paperSize;

            var softwareSystems = new List<SoftwareSystem>();
            softwareSystems.Add(primarySoftwareSystem);
            softwareSystems.AddRange(subsystems);

            foreach (var softwareSystem in softwareSystems)
            {
                AddToView(softwareSystem, deploymentView);
            }
        }

        private static void AddToView(SoftwareSystem softwareSystem, DeploymentView deploymentView)
        {
            var softwareSystemContainers = softwareSystem.Containers.Select(x => x.Id);
            var firstLevel = softwareSystem.Model.DeploymentNodes.Where(x => softwareSystemContainers.Intersect(x.ContainerInstances.Select(y => y.ContainerId)).Any()).ToList();
            var secondLevel = softwareSystem.Model.DeploymentNodes.SelectMany(x => x.Children).Where(x => softwareSystemContainers.Intersect(x.ContainerInstances.Select(y => y.ContainerId)).Any()).ToList();
            var thirdLevel = softwareSystem.Model.DeploymentNodes.SelectMany(x => x.Children).SelectMany(x => x.Children).Where(x => softwareSystemContainers.Intersect(x.ContainerInstances.Select(y => y.ContainerId)).Any()).ToList();
            var fourthLevel = softwareSystem.Model.DeploymentNodes.SelectMany(x => x.Children).SelectMany(x => x.Children).SelectMany(x => x.Children).Where(x => softwareSystemContainers.Intersect(x.ContainerInstances.Select(y => y.ContainerId)).Any()).ToList();
            var matchingDeployments = firstLevel.Union(secondLevel).Union(thirdLevel).Union(fourthLevel).ToList();

            foreach (var deploymentNode in matchingDeployments)
            {
                deploymentView.Add(deploymentNode);
            }
        }
    }
}
