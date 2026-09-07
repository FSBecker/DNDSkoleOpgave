using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Game;

public static class GameCatalog
{
    public static void Load(GameDefinitions definitions)
    {
        CharacterRaceCreator.Load(definitions);
        CharacterClassCreator.Load(definitions);
        ActionCatalog.Load(definitions);
        ItemCatalog.Load(definitions);
    }
}
