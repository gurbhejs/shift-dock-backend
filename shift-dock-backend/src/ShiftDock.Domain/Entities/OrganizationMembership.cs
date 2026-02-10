using ShiftDock.Domain.Enums;

namespace ShiftDock.Domain.Entities;

public class OrganizationMembership
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrganizationId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public OrgRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
