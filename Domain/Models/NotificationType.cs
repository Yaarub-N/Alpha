namespace Domain.Models;

public class NotificationType
{

    public int Id { get; set; }
    public string TypeName { get; set; } = null!;

    public virtual Notification Notifications { get; set; } = null!;
}
