namespace ShiftDock.Application.DTOs.Reports;

public class PayrollReportRequest
{
    public string OrganizationId { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty; // YYYY-MM-DD
    public string EndDate { get; set; } = string.Empty; // YYYY-MM-DD
}
