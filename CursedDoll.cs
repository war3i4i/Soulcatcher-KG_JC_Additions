namespace Soulcatcher_KG_JC_Additions;

public class CursedDoll
{
    private static bool HasItem(string prefab)
    {
        return Player.m_localPlayer.m_inventory.m_inventory.Any(itemData => itemData.m_dropPrefab.name == prefab);
    }
    
    public static bool HasCursedDoll()
    {
        return HasItem(Soulcatcher.CursedDoll.name);
    }
    
    public static bool BreakHandler_CursedDoll(ItemDrop.ItemData _)
    {
        if (HasCursedDoll())
        {
            CustomRemoveItemsNoLevel(Soulcatcher.CursedDoll.name, 1);
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,"$soulcatcher_curseddoll_destroyed");
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_shaman_fireball_expl"), Player.m_localPlayer.transform.position, Quaternion.identity);
            return false;
        }
        return true;
    }
    
    private static void CustomRemoveItemsNoLevel(string name, int amount)
    {
        foreach (ItemDrop.ItemData itemData in Player.m_localPlayer.m_inventory.m_inventory)
        {
            if (itemData.m_dropPrefab.name == name)
            {
                int num = Mathf.Min(itemData.m_stack, amount);
                itemData.m_stack -= num;
                amount -= num;
                if (amount <= 0)
                    break;
            }
        }

        Player.m_localPlayer.m_inventory.m_inventory.RemoveAll(x => x.m_stack <= 0);
        Player.m_localPlayer.m_inventory.Changed(); 
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_ValhallaItem
    {
        static void Postfix(ZNetScene __instance)
        {
            __instance.m_namedPrefabs.Add(Soulcatcher.CursedDoll.name.GetStableHashCode(), Soulcatcher.CursedDoll);
        }
    }

    private static void AddValhallaItemToODB()
    {
        if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
            ObjectDB.instance.GetItemPrefab("Amber") == null)
            return;

        if (ObjectDB.instance.GetItemPrefab(Soulcatcher.CursedDoll.name.GetStableHashCode()) != null) return;

        ObjectDB.instance.m_items.Add(Soulcatcher.CursedDoll);
        ObjectDB.instance.m_itemByHash[Soulcatcher.CursedDoll.name.GetStableHashCode()] = Soulcatcher.CursedDoll;
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPriority(Priority.Last)]
    public static class DB_Patch_ValhallaItem
    {
        private static void Postfix()
        {
            AddValhallaItemToODB();
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    [HarmonyPriority(Priority.Last)]
    public static class DB_Patch2_ValhallaItem
    {
        private static void Postfix()
        {
            AddValhallaItemToODB();
        }
    }
}