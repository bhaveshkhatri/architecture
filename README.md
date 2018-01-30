# architecture
## Ascension.Structurizr
1. Clone the repository.
2. Open the Ascension.Structurizr.sln solution file in Visual Studio.
3. Add a file called App.Keys.config to the Ascension.Structurizr.App project.
4. Copy and paste the following XML into the App.Keys.config file, and replace with the workspace information provided at structurizr.com:

```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
  <add key="WorkspaceId" value="REPLACE"/>
  <add key="ApiKey" value="REPLACE"/>
  <add key="ApiSecret" value="REPLACE"/>
</appSettings>
```

5. Press F5 to build and run the application - which will upload the JSON represenation of your architecture to structurizr.com. NOTE: Any change you make to elements or relationships will likely result in you having to manually rearrange your views on structurizr.com to what you had before making the update.