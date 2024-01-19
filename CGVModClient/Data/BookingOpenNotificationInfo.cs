namespace CGVModClient.Data;

public class BookingOpenNotificationInfo
{
    public Movie Movie { get; set; }
    public string ScreenType { get; set; }
    public Theater Theater { get; set; }
    public DateTime Date { get; set; }
}
