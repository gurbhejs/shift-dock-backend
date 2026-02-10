namespace ShiftDock.Application.DTOs.Organizations;

public class OrganizationResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public decimal DefaultHourlyRate { get; set; }
    public decimal DefaultContainerRate { get; set; }
    public decimal DefaultBoxRate { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? MemberCount { get; set; }
}
