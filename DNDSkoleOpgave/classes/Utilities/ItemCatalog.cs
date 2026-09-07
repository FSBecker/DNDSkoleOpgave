using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Items.Weapons;

namespace DNDSkoleOpgave.Utilities;

public static class ItemCatalog
{
    private static readonly Dictionary<string, CoreItem> Items = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bow"] = new Bow(),
        ["short-sword"] = new ShortSword(),
        ["axe"] = new Axe(),
        ["staff"] = new Staff(),
        ["wooden-shield"] = new Shield()
    };

    public static IReadOnlyDictionary<string, CoreItem> All => Items;

    public static CoreItem Get(string id) =>
        Items.TryGetValue(id, out CoreItem? item)
            ? item
            : throw new KeyNotFoundException($"No item has the ID '{id}'.");
}
