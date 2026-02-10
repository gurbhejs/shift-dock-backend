namespace ShiftDock.Application.DTOs.Organizations;

public class UpdateMemberRoleRequest
{
    public string Role { get; set; } = string.Empty; // Admin or Worker
}
