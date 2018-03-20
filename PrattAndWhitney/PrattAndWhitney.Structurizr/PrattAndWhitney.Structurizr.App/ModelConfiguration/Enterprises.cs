using Structurizr;

namespace PrattAndWhitney.Structurizr.App.ModelConfiguration
{
    public static class Enterprises
    {
        public static Enterprise Target { get; private set; }

        public static void Configure(Model model)
        {
            Target = model.Enterprise = new Enterprise("Pratt & Whitney");
        }
    }
}