namespace CGVModClient.Data;

public class GiveawayEvent
{
    public string Title { get; init; }
    public string EventIndex { get; init; }
    public string Period { get; init; }
    public string DDay { get; init; }

    private DateTime? startDate;
    public DateTime StartDate
    {
        get {
            if (startDate == null)
            {
                try { startDate = DateTime.Parse(Period.Split('~')[0]); }
                catch { startDate = DateTime.MinValue; }
            }
            return startDate.Value;
        }
    }

    private DateTime? endDate;
    public DateTime EndDate
    {
        get {
            if (endDate == null)
            {
                try { endDate = DateTime.Parse(Period.Split('~')[1]); }
                catch { endDate = DateTime.MinValue; }
            }
            return endDate.Value;
        }
    }
}
