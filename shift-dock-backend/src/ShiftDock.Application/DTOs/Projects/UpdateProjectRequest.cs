using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Notes { get; set; }
    public WorkType? WorkType { get; set; }
    public decimal? Rate { get; set; }
    public ContractStatus? ContractStatus { get; set; }
}
