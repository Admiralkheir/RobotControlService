using MongoDB.Bson;

namespace RobotControlService.Domain.Entities
{
    public class Robot : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public RobotStatus Status { get; set; }
        public Position CurrentPosition { get; set; }
        public DateTime? LastSeenDate { get; set; }
        public ObjectId? CurrentCommandId { get; set; }
    }

    public enum RobotStatus
    {
        Idle,
        Moving,
        Rotating
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Orientation { get; set; }
    }
}
