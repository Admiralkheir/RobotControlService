using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Data.Configurations
{
    public class RobotConfiguration : IEntityTypeConfiguration<Robot>
    {
        public void Configure(EntityTypeBuilder<Robot> builder)
        {
            builder.ToCollection("robots");
            builder.HasKey(r => r.Id);
            builder.Property(u => u.Status).HasConversion<string>();
            //builder.HasIndex(u => u.Name).IsUnique();

        }
    }
}
