using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class SyncShiftsRequest
{
    public List<ShiftSyncItem> Shifts { get; set; } = new();
}

public class ShiftSyncItem
{
    public string? Id { get; set; } // Null for new shifts, populated for existing
    public string ShiftDate { get; set; } = string.Empty; // YYYY-MM-DD
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string EndTime { get; set; } = string.Empty; // HH:mm
    public WorkType WorkType { get; set; }
    public decimal Rate { get; set; }
    public int? TargetQuantity { get; set; }
}
