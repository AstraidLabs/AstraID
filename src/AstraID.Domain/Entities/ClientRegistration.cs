namespace AstraID.Domain.Entities;

public class ClientRegistration
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsConfidential { get; set; }
    public DateTime CreatedUtc { get; set; }
    public Guid? CreatedBy { get; set; }
}
