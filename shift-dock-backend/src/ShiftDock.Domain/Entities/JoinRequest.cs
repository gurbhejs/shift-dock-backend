using ShiftDock.Domain.Enums;

namespace ShiftDock.Domain.Entities;

public class JoinRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrganizationId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
}
