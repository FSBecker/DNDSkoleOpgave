using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Items;
using Spectre.Console;

namespace DNDSkoleOpgave.UI;

public static class EquipmentMenu
{
    public static void Run(PlayerCharacter player)
    {
        while (true)
        {
            GameMenu.ShowTitle("Udstyr");
            CharacterSheet.Show(player);
            List<EquipmentItem> equipment = player.Inventory.OfType<EquipmentItem>().ToList();
            List<string> labels = new();
            for (int i = 0; i < equipment.Count; i++)
                labels.Add($"{i + 1}. Udstyr {equipment[i].Name} ({equipment[i].EquipmentSlot})");
            labels.Add("Tilbage");

            string chosen = GameMenu.Choose("Vælg udstyr fra tasken", labels, Markup.Escape);
            int index = labels.IndexOf(chosen);
            if (index == equipment.Count)
                return;
            player.EquipItem(equipment[index]);
        }
    }
}
