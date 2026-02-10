namespace ShiftDock.Application.DTOs.Projects;

public class CreateProjectWithOrgRequest : CreateProjectRequest
{
    public string OrganizationId { get; set; } = string.Empty;
}
