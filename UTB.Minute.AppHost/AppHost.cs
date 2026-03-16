var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql", port:55555)
                 .WithDataVolume()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithContainerName("minutky-sql-server");

var database = sql.AddDatabase("database");

builder.AddProject<Projects.UTB_Minute_DbManager>("utb-minute-dbmanager")
       .WithReference(database)
       .WithHttpCommand("reset-db", "Reset Database")
       .WaitFor(database);

builder.AddProject<Projects.UTB_Minute_WebApi>("utb-minute-webapi")
       .WithReference(database)
       .WaitFor(database);

builder.Build().Run();
