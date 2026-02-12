using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class ProjectResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Notes { get; set; }
    public WorkType WorkType { get; set; }
    public decimal Rate { get; set; }
    public string OrganizationId { get; set; } = string.Empty;
    public ContractStatus ContractStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? TotalWorkers { get; set; }
    public int? TotalShifts { get; set; }
}
