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
            builder.Property(r => r.CreatedDate).HasAnnotation("BsonDefaultValue", DateTime.UtcNow);
            // create a unique index on the Name field
            builder.HasIndex(u => u.Name).IsUnique();

        }
    }
}
