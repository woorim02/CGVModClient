using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CGVModClient.Data;

[Table(nameof(OpenNotificationInfo))]
public class OpenNotificationInfo
{
    private string movieIndex;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ForeignKey(typeof(Movie))]
    public string MovieIndex { get => Movie.Index; set => movieIndex = value; }
    [ManyToOne(CascadeOperations = CascadeOperation.All)]
    public Movie Movie { get; set; }

    public string ScreenType { get; set; }

    [ManyToOne]
    public Theater Theater { get; set; }

    public DateTime TargetDate { get; set; }
}
