using Npgsql;

namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

public interface ITestDatabase
{
    Task InitialiseAsync();
    NpgsqlConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}
