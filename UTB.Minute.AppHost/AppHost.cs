using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource> sql;
IResourceBuilder<KeycloakResource> keycloak;

if (builder.Environment.IsEnvironment("Testing"))
{
    sql = builder.AddSqlServer("sql-testing", port: 55550)
                 .WithContainerName("testing-minutky-sql-server");

    keycloak = builder.AddKeycloak("keycloak", 8080)
       .WithRealmImport("./Realm")
       .WithContainerName("utb-school-keycloak-testing");
}
else
{
    sql = builder.AddSqlServer("sql", port: 55555)
                 .WithDataVolume()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithContainerName("minutky-sql-server");

    keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithRealmImport("./Realm")
                      .WithContainerName("utb-minute-keycloak")
                      .WithDataVolume("utb-minute-keycloak-data")
                      .WithLifetime(ContainerLifetime.Persistent);
}

var database = sql.AddDatabase("database");

builder.AddProject<Projects.UTB_Minute_DbManager>("utb-minute-dbmanager")
       .WithReference(database)
       .WithHttpCommand("reset-db", "Reset Database")
       .WaitFor(database);

var webapi = builder.AddProject<Projects.UTB_Minute_WebApi>("webapi")
       .WithReference(database)
       .WithReference(keycloak)
       .WaitFor(database)
       .WaitFor(keycloak);

builder.AddProject<Projects.UTB_Minute_CanteenClient>("utb-minute-canteenclient")
       .WithReference(webapi)
       .WithReference(keycloak)
       .WaitFor(webapi)
       .WaitFor(keycloak);

builder.AddProject<Projects.UTB_Minute_AdminClient>("utb-minute-adminclient")
       .WithReference(webapi)
       .WithReference(keycloak)
       .WaitFor(webapi)
       .WaitFor(keycloak);

builder.Build().Run();
