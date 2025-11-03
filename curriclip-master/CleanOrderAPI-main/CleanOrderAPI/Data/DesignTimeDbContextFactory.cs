#if DEBUG
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CleanOrderAPI.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string basePath = Directory.GetCurrentDirectory();
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        string? cs = config.GetConnectionString("DefaultConnection");
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder =  new DbContextOptionsBuilder<ApplicationDbContext>();
                                                                            optionsBuilder.UseMySql(cs, ServerVersion.AutoDetect(cs));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
#endif