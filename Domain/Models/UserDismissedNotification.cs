namespace Domain.Models;

public class UserDismissedNotification
{

    public int Id { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Notification Notification { get; set; } = null!;
}