namespace PrattAndWhitney.Structurizr.App.Common
{
    public static class Constants
    {
        public static class TechnologyStack
        {
            public static string ApiPlatform { get { return WebAppPlatform + " Web API"; } }
            public static string ApiPlatformController { get { return ApiPlatform + " Controller"; } }
            public static string ApplicationPlatform { get { return ".NET Core 2.0"; } }
            public static string DatabaseServer { get { return "Microsoft SQL Server"; } }
            public static string FrontendFramework { get { return "Angular"; } }
            public static string FrontendComponent { get { return FrontendFramework + " Module/Component"; } }
            public static string FrontendRouteConfig { get { return FrontendFramework + " Route Configuration"; } }
            public static string FrontendService { get { return FrontendFramework + " Service"; } }
            public static string Notifications { get { return "SignalR"; } }
            public static string NotificationsHub { get { return Notifications + " Hub"; } }
            public static string NotificationsClient { get { return Notifications + " Hub Proxy"; } }
            public static string Transport { get { return "RabbitMQ"; } }
            public static string TransportAndWorkflowState { get { return Transport + " and MassTransit"; } }
            public static string WebAppPlatform { get { return "ASP.NET"; } }
            public static string Cache { get { return Transport; } }
        }
    }
}
