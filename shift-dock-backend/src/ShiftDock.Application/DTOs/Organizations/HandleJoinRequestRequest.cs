namespace ShiftDock.Application.DTOs.Organizations;

public class HandleJoinRequestRequest
{
    public List<string> RequestIds { get; set; } = new();
    public bool Approved { get; set; }
}
