namespace ShiftDock.Application.DTOs.Organizations;

public class UserOrganizationResponse
{
    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Owner, Admin, Worker
    public DateTime JoinedAt { get; set; }
    public decimal DefaultHourlyRate { get; set; }
    public decimal DefaultContainerRate { get; set; }
    public decimal DefaultBoxRate { get; set; }
}
