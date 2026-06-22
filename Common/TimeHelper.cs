namespace Common;

public static class TimeHelper
{
    private static readonly TimeZoneInfo RiyadhZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh");

    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, RiyadhZone);

    public static DateTime Today =>
        Now.Date;
}
