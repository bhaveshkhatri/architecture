namespace PrattAndWhitney.Structurizr.App
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            WorkspaceUploader.Upload(WorkspaceBuilder.Build());
        }
    }
}