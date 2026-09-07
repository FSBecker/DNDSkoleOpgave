using DNDSkoleOpgave.Characters;

namespace DNDSkoleOpgave.UI;

public static class CharacterArt
{
    public static string GetArt(CoreCharacter character)
    {
        if (character.IsDefeated)
            return "   .-----.\n   | RIP |\n   |_____|";
        if (character is PlayerCharacter)
            return "     O\n    /|\\\n    / \\";
        return character.CharacterRace.RaceName.ToLowerInvariant() switch
        {
            "goblin" => "  /\\   /\\\n ( o   o )\n   \\___/",
            "orc" => " .-----.\n | o o |\n |v___v|",
            "skeleton" => " .-----.\n |(o)(o)|\n  |||||",
            _ => "  .---.\n  |o o|\n  /|||\\"
        };
    }
}
