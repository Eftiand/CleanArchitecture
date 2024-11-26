var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("rabbitmq", username, password)
    .WithManagementPlugin()
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Web>("web-api")
    .WithReference(postgres)
    .WithReference(rabbitmq);

builder.Build().Run();
