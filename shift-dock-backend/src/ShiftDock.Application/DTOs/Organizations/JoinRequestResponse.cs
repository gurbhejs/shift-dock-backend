namespace ShiftDock.Application.DTOs.Organizations;

public class JoinRequestResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Status { get; set; } = "Requested"; // Always "Requested" for pending join requests
    public DateTime RequestedAt { get; set; }
}
