using ShiftDock.Domain.Enums;

namespace ShiftDock.Application.DTOs.Projects;

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Notes { get; set; }
    public WorkType WorkType { get; set; }
    public decimal Rate { get; set; }
    public ContractStatus ContractStatus { get; set; } = ContractStatus.Active;
}
