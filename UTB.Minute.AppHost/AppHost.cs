using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource> sql;

if (builder.Environment.IsEnvironment("Testing"))
{
    sql = builder.AddSqlServer("sql-testing", port: 55550)
                 .WithContainerName("testing-minutky-sql-server");
}
else
{
    sql = builder.AddSqlServer("sql", port: 55555)
                 .WithDataVolume()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithContainerName("minutky-sql-server");
}

var database = sql.AddDatabase("database");

builder.AddProject<Projects.UTB_Minute_DbManager>("utb-minute-dbmanager")
       .WithReference(database)
       .WithHttpCommand("reset-db", "Reset Database")
       .WaitFor(database);

builder.AddProject<Projects.UTB_Minute_WebApi>("webapi")
       .WithReference(database)
       .WaitFor(database);

builder.Build().Run();
