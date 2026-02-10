namespace ShiftDock.Application.DTOs.Reports;

public class WorkerPayrollResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int TotalContainers { get; set; }
    public int TotalBoxes { get; set; }
    public decimal TotalEarnings { get; set; }
    public int CompletedShifts { get; set; }
}
