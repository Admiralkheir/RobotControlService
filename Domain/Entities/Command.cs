using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace RobotControlService.Domain.Entities
{
    public class Command : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public ObjectId RobotId { get; set; }
        public ObjectId UserId { get; set; }
        public CommandStatus CommandStatus { get; set; }
        public CommandType CommandType { get; set; }
        public Dictionary<string, string> CommandParameters { get; set; }
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
        Rotate
    }
}
