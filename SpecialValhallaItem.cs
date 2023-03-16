namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{

    private static GameObject ValhallaItem;
    
    private static void AddValhallaItem()
    {
        ValhallaItem = asset.LoadAsset<GameObject>("Soulcatcher_ValhallaItem");
        ValhallaItem.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] = CreateDynamicSprite(Gem.PiecesTexture,
            ValhallaItem.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0].texture, "ValhallaItem");
    }

    [HarmonyPatch(typeof(ZNetScene),nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_ValhallaItem
    {
        static void Postfix(ZNetScene __instance)
        {
            __instance.m_prefabs.Add(ValhallaItem);
            __instance.m_namedPrefabs.Add(ValhallaItem.name.GetStableHashCode(), ValhallaItem);
        }
    } 

    private static void AddValhallaItemToODB()
    {
        if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
            ObjectDB.instance.GetItemPrefab("Amber") == null)
            return;

        if (ObjectDB.instance.GetItemPrefab(ValhallaItem.name.GetStableHashCode()) != null) return;

        ObjectDB.instance.m_items.Add(ValhallaItem);
        ObjectDB.instance.m_itemByHash[ValhallaItem.name.GetStableHashCode()] = ValhallaItem;
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

    [HarmonyPatch(typeof(GemStones.AddSocketToItem), nameof(GemStones.AddSocketToItem.Prefix))]
    static class Jewelcrafting__Patch
    {
        private const string prefabName = "Soulcatcher_ValhallaItem";

        private static bool _HadValhallaItem;

        private static void Prefix()
        {
            if (!GemStones.AddSocketAddingTab.TabOpen())
            {
                _HadValhallaItem = false;
                return;
            }
            _HadValhallaItem = HasItem(prefabName);
            if (_HadValhallaItem) CustomRemoveItemsNoLevel(prefabName, 1);
        }


        public static bool HasValhallaItem()
        {
            if (_HadValhallaItem)
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,"$soulcatcher_valhallaitem_destroyed");
                Instantiate(ZNetScene.instance.GetPrefab("fx_shaman_fireball_expl"),
                    Player.m_localPlayer.transform.position, Quaternion.identity);
            }
            return _HadValhallaItem;
        }

        private enum State
        {
            Init,
            MarkLabel,
            Done
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label newLabel = generator.DefineLabel();
            State state = State.Init;
            var method = AccessTools.Method(typeof(Jewelcrafting__Patch), nameof(HasValhallaItem));
            foreach (var codeInstruction in instructions)
            {
                if (state is State.MarkLabel)
                {
                    codeInstruction.labels.Add(newLabel);
                    state = State.Done;
                }

                yield return codeInstruction;
                if (codeInstruction.opcode == OpCodes.Ble_Un && state is State.Init)
                {
                    state = State.MarkLabel;
                    yield return new CodeInstruction(OpCodes.Call, method);
                    yield return new CodeInstruction(OpCodes.Brfalse, newLabel);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ret);
                }
            }
        }
    }
}