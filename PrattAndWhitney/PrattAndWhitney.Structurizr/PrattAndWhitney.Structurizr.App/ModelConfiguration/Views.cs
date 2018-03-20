using PrattAndWhitney.Structurizr.App.Extensions;
using Structurizr;

namespace PrattAndWhitney.Structurizr.App.ModelConfiguration
{
    public static class Views
    {
        public static void Configure(Workspace workspace)
        {
            Context.Configure(workspace);
            Container.Configure(workspace);
            Component.Configure(workspace);
            Styles.Configure(workspace);
        }

        public static class Context
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateEnterpriseContextLandscapeViewFor(Enterprises.Target, PaperSize.A3_Landscape);

                workspace.Views.CreateSystemContextViewFor(SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem, PaperSize.A3_Landscape);
            }
        }

        public static class Container
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateContainerViewFor(SoftwareSystems.Target.InvoiceTransactionsSoftwareSystem, PaperSize.A3_Landscape);

                workspace.Views.CreateContainerViewFor(SoftwareSystems.Target.InfrastructureServicesSoftwareSystem, PaperSize.A3_Landscape);
            }
        }

        public static class Component
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.CreateComponentViewFor(Containers.TargetSystem.WebClientSpaContainer, PaperSize.A4_Landscape);
            }
        }

        public static class Styles
        {
            public static void Configure(Workspace workspace)
            {
                workspace.Views.ConfigureStyles();
            }
        }
    }
}