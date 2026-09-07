namespace DNDSkoleOpgave.Items;

public sealed class LootEntry
{
    private readonly Func<CoreItem> _createItem;

    public LootEntry(string itemId, int chancePercent, Func<CoreItem> createItem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        ArgumentOutOfRangeException.ThrowIfLessThan(chancePercent, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(chancePercent, 100);
        ItemId = itemId;
        ChancePercent = chancePercent;
        _createItem = createItem ?? throw new ArgumentNullException(nameof(createItem));
    }

    public string ItemId { get; }
    public int ChancePercent { get; }

    public CoreItem? Roll() => Roll(new Utilities.RandomDiceRoller());

    public CoreItem? Roll(Utilities.IDiceRoller dice) => dice.Roll(100) <= ChancePercent
        ? _createItem()
        : null;
}
