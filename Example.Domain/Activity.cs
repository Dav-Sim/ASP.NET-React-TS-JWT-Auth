namespace Example.Domain;

public class Activity
{
    private Activity() { }
    public Activity(User user, ActivityType.ActivityTypeEnum activityType)
    {
        User = user;
        ActivityTypeId = (int)activityType;
    }

    public int Id { get; set; }
    public int ActivityTypeId { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ActivityType ActivityType { get; set; } = null!;
}
