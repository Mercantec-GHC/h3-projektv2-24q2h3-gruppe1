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
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<API.Models.PlantOverview> PlantOverviews { get; set; }
        public DbSet<API.Models.PlantSensor> PlantSensor { get; set; } = default!;
        public DbSet<API.Models.Sensor> Sensor { get; set; } = default!;
        public DbSet<API.Models.User> Users { get; set; } = default!;
        public DbSet<API.Models.Plant> Plants { get; set; } = default!;
    }
}
