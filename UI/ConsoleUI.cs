namespace Day1_TicketDetective.UI
{
    // A small set of helpers so the console output looks like a real demo screen
    // instead of plain white text on black.
    public static class ConsoleUI
    {
        public static void Header(string title)
        {
            Console.WriteLine();
            var line = new string('═', title.Length + 4);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔{line}╗");
            Console.WriteLine($"║  {title}  ║");
            Console.WriteLine($"╚{line}╝");
            Console.ResetColor();
        }

        public static void SubHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n▶ {text}");
            Console.ResetColor();
        }

        public static void Success(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✔ {text}");
            Console.ResetColor();
        }

        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✘ {text}");
            Console.ResetColor();
        }

        public static void Warning(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  ⚠ {text}");
            Console.ResetColor();
        }

        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"  · {text}");
            Console.ResetColor();
        }

        public static void ScenarioStep(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n[Scenario] {text}");
            Console.ResetColor();
        }

        public static void Divider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('─', 60));
            Console.ResetColor();
        }

        public static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPress ENTER to continue...");
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}
