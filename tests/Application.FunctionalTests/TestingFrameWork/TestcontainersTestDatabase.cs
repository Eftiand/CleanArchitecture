using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithAutoRemove(true)
        .Build();

    private string _connectionString = null!;
    private NpgsqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

        _connection = new NpgsqlConnection(_connectionString);
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connection)
            .Options;

        await using var context = new ApplicationDbContext(options);

        await context.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(_connection,
            new RespawnerOptions
            {
                TablesToIgnore = ["__EFMigrationsHistory"],
                SchemasToInclude = ["public"],
                DbAdapter = DbAdapter.Postgres
            });
    }

    public NpgsqlConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connectionString);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
