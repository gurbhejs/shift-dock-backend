namespace ShiftDock.Domain.Entities;

public class Shift
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = string.Empty;
    public string ShiftDate { get; set; } = string.Empty; // YYYY-MM-DD
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string EndTime { get; set; } = string.Empty; // HH:mm
    public int? TargetQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Project Project { get; set; } = null!;
    public ICollection<WorkerAssignment> Assignments { get; set; } = new List<WorkerAssignment>();
}
