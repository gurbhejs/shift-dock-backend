namespace ShiftDock.Application.DTOs.Assignments;

public class SyncProjectAssignmentsResponse
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Deleted { get; set; }
    public List<WorkerAssignmentResponse> CurrentAssignments { get; set; } = new();
}
