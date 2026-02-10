namespace ShiftDock.Application.DTOs.Notifications;

public class NotificationResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // ShiftAssignment, ShiftUpdate, ShiftCancellation, General
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? RelatedEntityId { get; set; }
}
