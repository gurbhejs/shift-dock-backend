namespace ShiftDock.Application.DTOs.Organizations;

public class UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public decimal? DefaultHourlyRate { get; set; }
    public decimal? DefaultContainerRate { get; set; }
    public decimal? DefaultBoxRate { get; set; }
}
