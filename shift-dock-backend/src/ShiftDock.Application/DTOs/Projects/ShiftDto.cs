using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class ShiftDto
{
    public string Id { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string ShiftDate { get; set; } = string.Empty; // YYYY-MM-DD
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string EndTime { get; set; } = string.Empty; // HH:mm
    public WorkType WorkType { get; set; }
    public decimal Rate { get; set; }
    public int? TargetQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
