namespace ShiftDock.Application.DTOs.Organizations;

public class OrganizationMemberResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty; // Owner, Admin, Worker
    public string Status { get; set; } = string.Empty; // Pending, Active, Inactive
    public DateTime JoinedAt { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? ContainerRate { get; set; }
    public decimal? BoxRate { get; set; }
}
