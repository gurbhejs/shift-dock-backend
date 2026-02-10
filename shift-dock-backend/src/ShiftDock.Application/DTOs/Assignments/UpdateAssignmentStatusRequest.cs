namespace ShiftDock.Application.DTOs.Assignments;

public class UpdateAssignmentStatusRequest
{
    public string Status { get; set; } = string.Empty; // Accepted, Rejected, Completed
    public int? ActualQuantity { get; set; }
    public string? Notes { get; set; }
}
