var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    .WithVolume("rabbitmq-data", "/var/lib/rabbitmq");

builder.AddProject<Projects.Web>("web-api")
    .WithReference(postgres)
    .WithReference(rabbitmq);

builder.Build().Run();
