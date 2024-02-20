using SQLite;

namespace CGVModClient.Data;

[Table(nameof(OpenNotificationInfo))]
public class OpenNotificationInfo
{
    private string movieIndex;
    private string theaterCode;
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string MovieIndex { get => Movie?.Index ?? movieIndex; set => movieIndex = value; }
    [Ignore]
    public Movie? Movie { get; set; }

    public string ScreenType { get; set; }

    public string TheaterCode { get => Theater?.TheaterCode ?? theaterCode; set => theaterCode = value; }
    [Ignore]
    public Theater Theater { get; set; }

    public DateTime TargetDate { get; set; }

    public bool IsOpen { get; set; } = false;
    private bool canReservation = false;
    public bool CanReservation { get => IsOpen && canReservation; set => canReservation = value; }
}
