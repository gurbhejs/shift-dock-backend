namespace ShiftDock.Application.DTOs.Reports;

public class ProjectReportResponse
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int TotalShifts { get; set; }
    public int CompletedShifts { get; set; }
    public int TotalWorkers { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
