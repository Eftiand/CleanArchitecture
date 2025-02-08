using Npgsql;

namespace coaches.Application.FunctionalTests.TestingFrameWork.Database;

public interface ITestDatabase
{
    Task InitialiseAsync();
    NpgsqlConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}
