namespace ShiftDock.Application.DTOs.Projects;

public class ProjectResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string OrganizationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Active, Completed, OnHold
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? TotalWorkers { get; set; }
    public int? TotalShifts { get; set; }
}
