using Structurizr;

namespace Ascension.Structurizr.App
{
    public static class StructurizrExtensions
    {
        public static Container AddApiServiceContainer(this SoftwareSystem softwareSystem, string name, string description = "")
        {
            var descriptionToUse = string.IsNullOrWhiteSpace(description) ? string.Format("{0}.", name) : description;
            var container = softwareSystem.AddContainer(name, descriptionToUse, "TBD");
            container.AddTags(AdditionalTags.ApiService);

            return container;
        }
    }
}
