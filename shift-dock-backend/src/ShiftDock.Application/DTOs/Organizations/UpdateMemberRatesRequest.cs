namespace ShiftDock.Application.DTOs.Organizations;

public class UpdateMemberRatesRequest
{
    public decimal? HourlyRate { get; set; }
    public decimal? ContainerRate { get; set; }
    public decimal? BoxRate { get; set; }
}
