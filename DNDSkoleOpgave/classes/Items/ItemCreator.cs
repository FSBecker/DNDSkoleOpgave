using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Items;

public static class ItemCreator
{
    public static CoreItem Create(string id) => ItemCatalog.Create(id);
}
