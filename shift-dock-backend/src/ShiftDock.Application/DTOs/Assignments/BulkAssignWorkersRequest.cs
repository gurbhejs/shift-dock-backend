namespace ShiftDock.Application.DTOs.Assignments;

public class BulkAssignWorkersRequest
{
    public List<string> UserIds { get; set; } = new();
}
