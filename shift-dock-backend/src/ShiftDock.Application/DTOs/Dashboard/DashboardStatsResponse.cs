namespace ShiftDock.Application.DTOs.Dashboard;

public class DashboardStatsResponse
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalWorkers { get; set; }
    public int ActiveWorkers { get; set; }
    public int TotalShifts { get; set; }
    public int UpcomingShifts { get; set; }
    public int CompletedShifts { get; set; }
    public int PendingAssignments { get; set; }
}
