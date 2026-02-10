namespace ShiftDock.Application.DTOs.Organizations;

public class CreateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal DefaultHourlyRate { get; set; }
    public decimal DefaultContainerRate { get; set; }
    public decimal DefaultBoxRate { get; set; }
}
