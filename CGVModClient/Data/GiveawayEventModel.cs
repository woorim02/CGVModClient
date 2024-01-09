namespace CGVModClient.Data;

public class GiveawayEventModel
{
    public string Title { get => GiveawayItemList.GiveawayItemName; }
    public string EventIndex { get; init; }
    public string GivewayIndex { get => GiveawayItemList.GiveawayItemCode; }
    public string Contents {  get; init; }

    /// <summary>
    /// Please do not use it. This is a property for json serialization.
    /// </summary>
    public GiveawayInfo GiveawayItemList { get; set; }

    public class GiveawayInfo
    {
        public string GiveawayItemCode { get; set; }
        public string GiveawayItemName { get; set; }
    }
}
