# architecture
## Ascension.Structurizr
1. Clone the repository.
2. Open the Ascension.Structurizr.sln solution file.
3. Add a file called App.Keys.config to the Ascension.Structurizr.App project.
4. Enter the following into the contents, and replace with the workspace information provided at structurizr.com:

```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
  <add key="WorkspaceId" value="REPLACE"/>
  <add key="ApiKey" value="REPLACE"/>
  <add key="ApiSecret" value="REPLACE"/>
</appSettings>
```