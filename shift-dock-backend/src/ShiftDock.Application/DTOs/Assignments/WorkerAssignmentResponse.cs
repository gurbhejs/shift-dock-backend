namespace ShiftDock.Application.DTOs.Assignments;

public class WorkerAssignmentResponse
{
    public string Id { get; set; } = string.Empty;
    public string ShiftId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Assigned, Accepted, Rejected, Completed
    public int? ActualQuantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
