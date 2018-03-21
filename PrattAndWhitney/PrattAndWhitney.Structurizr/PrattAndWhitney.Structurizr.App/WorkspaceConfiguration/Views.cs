using PrattAndWhitney.Structurizr.App.Extensions;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Views
    {
        public static void Configure(Workspace workspace)
        {
            Context.Configure(workspace);
            Container.Configure(workspace);
            Component.Configure(workspace);
            Deployment.Configure(workspace);
            Styles.Configure(workspace);
        }

        public static class Context
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateEnterpriseContextLandscapeViewFor(Enterprises.Target, PaperSize.A3_Landscape);

                workspace.Views.CreateSystemContextViewFor(SoftwareSystems.Target.InvoiceTransactionsSystem, PaperSize.A3_Landscape);
            }
        }

        public static class Container
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateContainerViewFor(SoftwareSystems.Target.InvoiceTransactionsSystem, PaperSize.A3_Landscape);

                workspace.Views.CreateContainerViewFor(SoftwareSystems.Target.InfrastructureServices, PaperSize.A3_Landscape);
            }
        }

        public static class Component
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateComponentViewFor(Containers.TargetSystem.WebClient, PaperSize.A4_Landscape);
                workspace.Views.CreateComponentViewFor(Containers.TargetSystem.ApiService, PaperSize.A4_Landscape);
            }
        }

        public static class Deployment
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateDeploymentViewFor(SoftwareSystems.Target.InvoiceTransactionsSystem, PaperSize.A4_Landscape);
                workspace.Views.CreateDeploymentViewFor(SoftwareSystems.Target.InfrastructureServices, PaperSize.A4_Landscape);
            }
        }

        public static class Styles
        {
            public static void Configure(Workspace workspace)
            {
                var styles = workspace.Views.Configuration.Styles;

                styles.Add(new ElementStyle(Tags.Element) { FontSize = 36 });

                styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff", Shape = Shape.RoundedBox });
                styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
                styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
                styles.Add(new ElementStyle(Tags.Component) { Background = "#85bbf0", Color = "#ffffff" });

                styles.Add(new ElementStyle(AdditionalTags.FutureState) { Border = Border.Dashed, Background = "#3cb371" });
                styles.Add(new ElementStyle(AdditionalTags.SunsetPhaseOut) { Border = Border.Dashed, Background = "#ff8c00" });
                styles.Add(new ElementStyle(AdditionalTags.EventBus) { Shape = Shape.Pipe, Height = 300, Width = 750 });
                styles.Add(new ElementStyle(AdditionalTags.Database) { Shape = Shape.Cylinder });
                styles.Add(new ElementStyle(AdditionalTags.Hub) { Shape = Shape.Hexagon });
                styles.Add(new ElementStyle(AdditionalTags.Gateway) { Shape = Shape.Hexagon });
                styles.Add(new ElementStyle(AdditionalTags.Files) { Shape = Shape.Folder });
                styles.Add(new ElementStyle(AdditionalTags.InfrastructureServices) { Shape = Shape.Circle });

                styles.Add(new RelationshipStyle(AdditionalTags.PotentiallyUsedRelation) { Color = "#ee7600" });
                styles.Add(new RelationshipStyle(AdditionalTags.CurrentButNotRecommendedRelation) { Color = "#ff0000" });

                //TODO
                //styles.Add(new ElementStyle(AdditionalTags.ViewSubject) { Background = "#ffa500", Color = "#ffffff", Shape = Shape.RoundedBox });
            }
        }
    }
}