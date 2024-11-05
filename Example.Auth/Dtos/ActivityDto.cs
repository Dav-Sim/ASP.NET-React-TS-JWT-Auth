namespace Example.Auth.Dtos;

public class ActivityDto
{
    public ActivityDto(string activity, string activityDescription, DateTime date)
    {
        Activity = activity;
        ActivityDescription = activityDescription;
        Date = date;
    }

    public string Activity { get; set; } = null!;
    public string ActivityDescription { get; set; } = null!;
    public DateTime Date { get; set; }
}
