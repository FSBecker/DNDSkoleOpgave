using System.Text;
using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        GameApplication game = new(new RandomDiceRoller());
        game.Run();
    }
}
