namespace ShiftDock.Application.DTOs.Dashboard;

public class TodayShiftResponse
{
    public string ShiftId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int AssignedWorkers { get; set; }
}
