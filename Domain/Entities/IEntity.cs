namespace RobotControlService.Domain.Entities
{
    public interface IEntity<TKey> where TKey : notnull
    {
        TKey Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
