namespace PhoneAssistant.Cli;

public sealed class Progress
{
    private bool _first = true;
    private const int MaxBarWidth = 60;

    public void Draw(int value, int maximum)
    {
        int barWidth = MaxBarWidth;
        int displacement = 1;

        if (maximum < MaxBarWidth)
        {
            barWidth = maximum;
            displacement = 0;
        }

        if (value > maximum) return;

        if (_first)
        {
            Console.CursorVisible = false;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('.', barWidth));
            Console.ResetColor();
            Console.Write("] 0%");
            _first = false;
            return;
        }

        int cursor = displacement + (value * barWidth / maximum);
        if (cursor < barWidth + 1)
        {
            Console.CursorLeft = cursor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("*");
            Console.ResetColor();
        }
        Console.CursorLeft = barWidth + 3;
        Console.Write("{0}%", value * 100 / maximum);

        if (value == maximum)
        {
            Console.WriteLine("");
            Console.CursorVisible = true;
        }
    }

}
