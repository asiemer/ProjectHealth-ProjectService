#General
This is the `Projects` component of the Heart Beat project implemented as Micro Services using CQRS, DDD and Event Sourcing.

This project uses

- ASP.NET Web API using attribute routing
- [GetEventStore](https://geteventstore.com/) 3.0.3 for Windows 
- [MongoDB](https://www.mongodb.org/) 3.0.3 (Windows 64-bit 2009 R2+)
- StructureMap for DI
- PostSharp for AOP to provide things like logging
- Psake to build and test the solution
- NUnit and Rhino Mocks for Unit and Integration tests
- OctoPack to generate Nuget as artifacts for deployment

#Prerequisites
You need to have the following installed on your system 

- Visual Studio 2013 Professional or higher
- [PostSharp](https://visualstudiogallery.msdn.microsoft.com/a058d5d3-e654-43f8-a308-c3bdfdd0be4a)
- MS SQL Express 2012 (instance name in app.config is SQLEXPRESS2012
- The solution folder on a path with no spaces

#Instructions

- Clone the Git repository to your local drive ([https://github.com/asiemer/ProjectHeartbeat-IterationZero](https://github.com/asiemer/ProjectHeartbeat-ProjectsService))
- Execute the batch file `ClickToBuild.cmd` located in the folder containing the cloned repository to build and test the solution. 
- Alternatively open a PowerShell command prompt and navigate to the folder containing the cloned repository. Build the solution by invoking the following command `.\psake\psake.ps1 .\default.ps1 Test`
- Create a database called `nservicebus` in SQL Server Express
- Create a sub-folder `data\mongodb` and a sub-folder `logs\mongodb` in the solution folder 
- Run MongoDB as a Windows service by using this PS script from the root of the solution:

```

    $path = Get-Location
    echo "systemLog:" > .\MongoDb\mongod.cfg
    echo "   destination: file" >> .\MongoDb\mongod.cfg
    echo "   path: $path\logs\mongodb\mongod.log" >> .\MongoDb\mongod.cfg
    echo "storage:" >> .\MongoDb\mongod.cfg
    echo "   dbPath: $path\data\mongodb" >> .\MongoDb\mongod.cfg
    sc.exe create MongoDB binPath= "$path\mongodb\mongod.exe --service --config=$path\mongodb\mongod.cfg"  DisplayName= "MongoDB" start= "auto" 
    Start-Service MongoDB

```

- Execute the batch file `RunGES.cmd` in the root folder of the repository to run GetEventStore. This will start GES with the data directory `..\Data\EventStore\Projects` listening at the default tcp-ip port 1113 and http port 2113. The default username is equal to `admin` and the default password is `changeit`.

#Admin GES
Open a browser and navigate to `localhost:2113/web/index.html`. Enter the credentials when asked (`admin`/`changeit`). Navigate to the `Stream Browser` tab. You should see a list of streams. Click on the one whose events you want to see, e.g. `ProjectAggregate-<ID>` where `<ID>` is a Guid representing the ID of the aggregate instance. The list of events in the stream will be displayed starting with the most recent event.

#Admin MongoDB
Install MongoVue (if can be used for free) which is one of the best admin tools for MongoDB. Create a new connection. For server just add `localhost`.

#How to use
Run the application. By default IIS Express will listen at port 3030. You can use the Postman REST client for Google Chrome to test the application. Iteration zero implements a `Projects` controller with multiple endpoints
##GET requests
1 `localhost:3030/api/projects`

2 `localhost:3030/api/projects/<projectId>`

##POST requests

1 `localhost:3030/api/projects/`

2 `localhost:3030/api/projects/<projectId>/cem`

3 `localhost:3030/api/projects/<projectId>/pm`

4 `localhost:3030/api/projects/<projectId>/teamMembers/add`

5 `localhost:3030/api/projects/<projectId>/teamMembers/remove`

etc.

the POST requests to the first endpoint expects a body

`{ name: "Some project name" }`

whilst the POST requests to the second and third endpoints expects a body like this

`{ staffId: "<some GUID>" }`

the POST requests to the fourth and fifth endpoints expect

`{ staffIds: [<array of GUIDs>] }`
 
#Manage the read model
There is a quick and dirty command line application that currently just dumps the existing read model in MongoDB and re-creates it from scratch. For this tool to work both services MongoDB and GES need to be up and running.
The tool is called `ReadModelTool` and can be run from the command line or by double clicking onto `ReadModelTool.exe`. The settings can be changed in the `ReadModelTool.exe.config`.