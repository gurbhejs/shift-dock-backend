namespace ShiftDock.Application.DTOs.Organizations;

public class UpdateMemberStatusRequest
{
    public string Status { get; set; } = string.Empty; // Active or Inactive
}
