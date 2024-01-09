namespace CGVModClient.Data;

public class GiveawayTheaterInfo
{
    public string GivewayIndex { get; init; }
    public List<Area> AreaList { get; init; }
    public List<Theater> TheaterList { get; init; }

    public class Area
    {
        public string AreaName { get; init; }
        public string AreaCode { get; init; }
        public string TheaterCount { get; init; }
    }

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
}
