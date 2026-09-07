using Spectre.Console;

namespace DNDSkoleOpgave.UI;

public static class GameMenu
{
    public static T Choose<T>(string title, IEnumerable<T> choices, Func<T, string> label) where T : notnull
    {
        return AnsiConsole.Prompt(new SelectionPrompt<T>()
            .Title(title)
            .PageSize(10)
            .UseConverter(label)
            .AddChoices(choices));
    }

    public static string Choose(string title, params string[] choices) =>
        Choose(title, choices, Markup.Escape);

    public static void Pause(string message = "Tryk Enter for at fortsætte")
    {
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(message)}[/]");
        Console.ReadLine();
    }

    public static void ShowMessage(string message) => AnsiConsole.WriteLine(message);

    public static void ShowTitle(string title)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[bold]{Markup.Escape(title)}[/]"));
    }
}
