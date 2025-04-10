namespace Domain.Models;

public class NotificationTarget
{
 
    public int Id { get; set; }
    public string TargetName { get; set; } = null!;

    public virtual Notification Notifications { get; set; } =null!;
}
