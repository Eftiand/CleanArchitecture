var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume()
    .WithPgAdmin(x =>
    {
        x.WithLifetime(ContainerLifetime.Persistent);
        x.WithHostPort(5433);
    })
    .WithLifetime(ContainerLifetime.Persistent);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("rabbitmq", username, password, port: 5672)
    .WithManagementPlugin(port: 15672)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Web>("web-api")
    .WithReference(postgres)
    .WithReference(rabbitmq);

builder.Build().Run();
