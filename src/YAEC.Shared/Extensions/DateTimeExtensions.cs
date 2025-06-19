namespace YAEC.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime Now => DateTime.UtcNow;
    
    public static bool IsToday(this DateTime input)
    {
        return input.Date == Now.Date;
    }
    
    public static bool IsWeekDay(this DateTime input)
    {
        return !new[]
        {
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        }.Contains(input.DayOfWeek);
    }

    public static bool IsWeekend(this DateTime input)
    {
        return !input.IsWeekDay();
    }

    public static int Age(this DateTime birthDay)
    {
        if (DateTime.Today.Month < birthDay.Month || DateTime.Today.Month == birthDay.Month && DateTime.Today.Day < birthDay.Day)
            return DateTime.Today.Year - birthDay.Year - 1;
        return DateTime.Today.Year - birthDay.Year;
    }
    
    public static DateTime StartOfDay(this DateTime input)
    {
        return new DateTime(input.Year, input.Month, input.Day);
    }
    
    public static DateTime EndOfDay(this DateTime input)
    {
        return new DateTime(input.Year, input.Month, input.Day)
            .AddDays(1)
            .AddTicks(-1);
    }
    
    public static DateTime StartOfMonth(this DateTime input)
    {
        return new DateTime(input.Year, input.Month, 1);
    }
    
    public static DateTime EndOfMonth(this DateTime input)
    {
        return new DateTime(input.Year, input.Month, 1)
            .AddMonths(1)
            .AddTicks(-1);
    }
}