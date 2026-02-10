using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class UpdateShiftRequest
{
    public string? ShiftDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public WorkType? WorkType { get; set; }
    public decimal? Rate { get; set; }
    public int? TargetQuantity { get; set; }
}
