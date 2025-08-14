namespace AstraID.Domain.Entities;

public class AuditEvent
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public Guid? UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Data { get; set; }
}
