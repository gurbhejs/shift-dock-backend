using ShiftDock.Application.DTOs.Assignments;

namespace ShiftDock.Application.DTOs.Dashboard;

public class WorkerDashboardResponse
{
    public int UpcomingShifts { get; set; }
    public int CompletedShifts { get; set; }
    public decimal TotalEarnings { get; set; }
    public int PendingAssignments { get; set; }
    public List<WorkerAssignmentResponse> RecentAssignments { get; set; } = new();
}
