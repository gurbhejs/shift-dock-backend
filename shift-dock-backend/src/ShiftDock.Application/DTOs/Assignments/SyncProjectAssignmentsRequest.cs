namespace ShiftDock.Application.DTOs.Assignments;

public class SyncProjectAssignmentsRequest
{
    public List<ProjectAssignmentItem> Assignments { get; set; } = new();
}

public class ProjectAssignmentItem
{
    public string ShiftId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
