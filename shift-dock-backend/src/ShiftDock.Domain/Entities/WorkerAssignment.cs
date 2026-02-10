namespace ShiftDock.Domain.Entities;

public class WorkerAssignment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ShiftId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Assigned"; // Assigned, Accepted, Rejected, Completed
    public int? ActualQuantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Shift Shift { get; set; } = null!;
    public User User { get; set; } = null!;
}
