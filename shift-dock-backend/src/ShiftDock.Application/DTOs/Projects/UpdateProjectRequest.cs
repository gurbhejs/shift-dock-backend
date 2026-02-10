namespace ShiftDock.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; } // Active, Completed, OnHold
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
