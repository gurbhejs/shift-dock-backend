namespace ShiftDock.Domain.Entities;

public class Organization
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public decimal DefaultHourlyRate { get; set; }
    public decimal DefaultContainerRate { get; set; }
    public decimal DefaultBoxRate { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
}
