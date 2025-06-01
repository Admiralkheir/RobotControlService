using MongoDB.Bson;

namespace RobotControlService.Domain.Entities
{
    public class Command : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public ObjectId RobotId { get; set; }
        public ObjectId UserId { get; set; }
        public CommandStatus CommandStatus { get; set; }
        public CommandType CommandType { get; set; }
        public Dictionary<string, object> CommandParameters { get; set; } = new Dictionary<string, object>();
        public string? FailureReason { get; set; }
    }
    public enum CommandStatus
    {
        Queued,
        InProgress,
        Completed,
        Failed
    }
    public enum CommandType
    {
        Move,
        Rotate,
        Stop,
        Start,
        Pause,
        Resume
    }
}
