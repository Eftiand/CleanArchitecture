using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace coaches.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // Use a temporary connection string for migration generation
        optionsBuilder.UseNpgsql("Host=localhost;Database=coaches;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
