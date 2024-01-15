namespace CGVModClient.Data;

public class Theater
{
    public string TheaterCode { get; init; }
    public string TheaterName { get; init; }
    public string CountTypeCode { get; init; }
    public string EncCount { get; init; }
    public string GiveawayName { get; init; }
    public string GiveawayRemainCount { get; set; }
    public string ReceiveTypeCode { get; init; }
}
