using MongoDB.Bson;

namespace RobotControlService.Domain.Entities
{
    // initialScript should add admin user
    public class User : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
        public UserRole Role { get; set; }
        public List<ObjectId>? RobotIds { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Operator,
        Monitor,
        Robot
    }
}
