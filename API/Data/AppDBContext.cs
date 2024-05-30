using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDBContext : DbContext
    {
        protected readonly IConfiguration configuration;

        public AppDBContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }

        public DbSet<API.Models.PlantOverview> PlantOverviews { get; set; }
        public DbSet<API.Models.Arduino> Arduinos { get; set; } = default!;
        public DbSet<API.Models.User> Users { get; set; } = default!;
        public DbSet<API.Models.Plant> Plants { get; set; } = default!;
        public DbSet<API.Models.Setting> Setting { get; set; } = default!;
    }
}
