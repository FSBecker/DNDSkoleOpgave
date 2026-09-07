using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Items.Weapons;

namespace DNDSkoleOpgave.Utilities;

public static class ItemCatalog
{
    private static readonly Dictionary<string, Func<CoreItem>> ItemFactories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bow"] = () => new Bow(),
        ["short-sword"] = () => new ShortSword(),
        ["axe"] = () => new Axe(),
        ["staff"] = () => new Staff(),
        ["wooden-shield"] = () => new Shield(),
        ["crystal-staff"] = () => new CrystalStaff(),
        ["wizard-robes"] = () => new WizardRobes(),
        ["orb"] = () => new Orb(),
        ["light-leather-armor"] = () => new LightLeatherArmor(),
        ["great-axe"] = () => new GreatAxe(),
        ["small-potion"] = () => new ConsumableItem("small-potion", "Small Potion",
            [new Combat.ActionEffect(Enums.ActionEffectType.Healing, 6, Enums.EffectTarget.Self)])
    };

    public static IEnumerable<string> AllItemIds => ItemFactories.Keys;

    public static void Load(Persistence.GameDefinitions definitions)
    {
        ItemFactories.Clear();
        foreach (Persistence.ItemDefinition item in definitions.AllItems)
            ItemFactories.Add(item.Id, () => ItemCreator.Create(item, definitions));
    }

    public static CoreItem Create(string id) =>
        ItemFactories.TryGetValue(id, out Func<CoreItem>? factory)
            ? factory()
            : throw new KeyNotFoundException($"No item has the ID '{id}'.");

    public static CoreItem Get(string id) => Create(id);
}
