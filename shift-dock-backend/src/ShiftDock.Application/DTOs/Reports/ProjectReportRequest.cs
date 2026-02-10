namespace ShiftDock.Application.DTOs.Reports;

public class ProjectReportRequest
{
    public string OrganizationId { get; set; } = string.Empty;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Status { get; set; } // Active, Completed, OnHold
}
