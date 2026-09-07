namespace DNDSkoleOpgave.UI;

public static class CombatLog
{
    private static readonly Queue<string> Messages = new();
    public static IReadOnlyList<string> RecentMessages => Messages.ToList();

    public static void Clear() => Messages.Clear();

    public static void Write(string message)
    {
        Messages.Enqueue(message);
        while (Messages.Count > 6)
            Messages.Dequeue();
    }

    public static void Write(IEnumerable<string> messages)
    {
        foreach (string message in messages)
            Write(message);
    }
}
