using Structurizr;

namespace PrattAndWhitney.Structurizr.App.WorkspaceConfiguration
{
    public static class Enterprises
    {
        public static Enterprise Target { get; private set; }

        public static void Configure(Model model)
        {
            Target = model.Enterprise = new Enterprise("Pratt & Whitney v1");
        }
    }
}