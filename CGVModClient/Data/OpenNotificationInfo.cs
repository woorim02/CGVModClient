using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CGVModClient.Data;

[Table(nameof(OpenNotificationInfo))]
public class OpenNotificationInfo
{
    [ManyToOne]
    public Movie Movie { get; set; }
    public string ScreenType { get; set; }
    [ManyToOne]
    public Theater Theater { get; set; }
    public DateTime TargetDate { get; set; }
}
