using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Data.Configurations
{
    public class CommandConfiguration : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder.ToCollection("commands");
            builder.HasKey(c => c.Id);
            //builder.Property(c => c.CreatedDate).HasAnnotation("BsonDefaultValue", DateTime.UtcNow);
        }
    }
}
