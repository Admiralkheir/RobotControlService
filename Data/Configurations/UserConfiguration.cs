using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToCollection("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.CreatedDate).HasAnnotation("BsonDefaultValue", DateTime.UtcNow);
            // create a unique index on the Username field
            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
