namespace CGVModClient.Data;

public class Movie
{
    public string Index { get; set; }
    public string Title { get; set;}
    public string ThumbnailSource { get; set;}
    /// <summary>
    /// 영화코드 - 모코드
    /// </summary>
    public string MovieGroupCd { get; set;}
}
