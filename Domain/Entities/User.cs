using MongoDB.Bson;

namespace RobotControlService.Domain.Entities
{
    public class User : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
    }
}
