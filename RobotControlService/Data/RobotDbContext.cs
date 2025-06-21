using Microsoft.EntityFrameworkCore;
using RobotControlService.Domain.Entities;
using System.Reflection;

namespace RobotControlService.Data
{
    public class RobotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Robot> Robots { get; set; }
        public DbSet<Command> Commands { get; set; }

        public RobotDbContext(DbContextOptions<RobotDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // apply configuration for entities
            modelBuilder
                .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
