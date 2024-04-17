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

        public DbSet<PlantOverview> Plants { get; set; }
        public DbSet<API.Models.PlantSensor> PlantSensor { get; set; } = default!;
        public DbSet<API.Models.Sensor> Sensor { get; set; } = default!;
        public DbSet<API.Models.User> User { get; set; } = default!;
    }
}
