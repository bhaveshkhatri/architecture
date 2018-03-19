using Structurizr;
using Structurizr.Api;
using System.Configuration;

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