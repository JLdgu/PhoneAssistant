namespace PhoneAssistant.WPF.Shared;

public static class Utility
{
    public static string ToOrdinalWorkingDate(DateTime date, bool hexSuperscript = false, int buffer = 0)
    {
        DateTime weekDay = date.AddDays(buffer);
        if (buffer > 0)
        {
            while (weekDay.DayOfWeek == DayOfWeek.Saturday || weekDay.DayOfWeek == DayOfWeek.Sunday)
                weekDay = weekDay.AddDays(buffer);
        }

        string ordinalDay = string.Empty;
        int number = weekDay.Day;
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02E2\x1D57" : number.ToString() + "<sup>st</sup>";
                    break;
                case 2:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x207F\x1D48" : number.ToString() + "<sup>nd</sup>";
                    break;
                case 3:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02B3\x1D48" : number.ToString() + "<sup>rd</sup>";
                    break;
                default:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
                    break;
            }
        }
        string from = weekDay.ToString("dddd * MMMM yyyy");
        from = from.Replace("*", ordinalDay);

        return from;
    }

}
