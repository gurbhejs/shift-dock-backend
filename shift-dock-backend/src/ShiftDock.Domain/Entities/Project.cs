using ShiftDock.Domain.Enums;

namespace ShiftDock.Domain.Entities;

public class Project
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Notes { get; set; }
    public WorkType WorkType { get; set; }
    public decimal Rate { get; set; }
    public ContractStatus ContractStatus { get; set; } = ContractStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
