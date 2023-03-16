using ItemDataManager;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static class LanternCombinatorUI
    {
        private static GameObject UI;
        private static Text Text_Main;

        private static LanternComponent Target;
        private static LanternComponent Source;

        private static ConfigEntry<int> Fee;

        public static bool IsVisible()
        {
            return UI && UI.activeSelf;
        }

        public static void Init()
        {
            UI = Instantiate(asset.LoadAsset<GameObject>("SoulcatcherUI_Combinator"));
            Text_Main = UI.transform.Find("Canvas/Background/Text").GetComponent<Text>();
            DontDestroyOnLoad(UI);
            UI.SetActive(false);
            Fee = config("Lantern Soul Combination Fee", "Fee", 30, "The fee to combine two lanterns");
            UI.transform.Find("Canvas/Background/Confirm").GetComponent<Button>().onClick
                .AddListener(ClickConfirmButton);
            UI.transform.Find("Canvas/Background/Cancel").GetComponent<Button>().onClick.AddListener(ClickCancelButton);
        }

        public static bool ClickItem(ItemDrop.ItemData target, ItemDrop.ItemData source)
        {
            if (target == source) return false;
            LanternComponent sourceLantern = source?.Data().Get<LanternComponent>();
            LanternComponent targetLantern = target?.Data().Get<LanternComponent>();
            if (!sourceLantern || !targetLantern) return false;
            Show(targetLantern, sourceLantern);
            return true;
        }

        private static void ClickConfirmButton()
        {
            Player p = Player.m_localPlayer;
            if (!p) return;
            AUsrc.Play();
            if (Target != null && Source != null && p.m_inventory.m_inventory.Contains(Target.Item) &&
                p.m_inventory.m_inventory.Contains(Source.Item))
            {
                LanternComponent.CombineTwoLanterns(Target, Source, Fee.Value);
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "<color=#00FF00>Souls combined!</color>");
            }
            Hide();
        }

        private static void ClickCancelButton()
        {
            AUsrc.Play();
            Hide();
        }

        private static void Show(LanternComponent _target, LanternComponent _source)
        {
            Target = _target;
            Source = _source;
            Text_Main.text = LanternComponent.GenerateLanternCombineString(Target, Source, Fee.Value);
            UI.SetActive(true);
        }

        public static void Hide()
        {
            Text_Main.text = "";
            Target = null;
            Source = null;
            UI.SetActive(false);
        }
    }
    [HarmonyPatch(typeof(InventoryGrid), nameof(InventoryGrid.DropItem))]
    static class InventoryGui_SetupDragItem_Patch2
    {
        static bool Prefix(InventoryGrid __instance, ItemDrop.ItemData item, Vector2i pos, ref bool __result)
        {
            ItemDrop.ItemData itemAt = __instance.m_inventory.GetItemAt(pos.x, pos.y);
            if (!LanternCombinatorUI.ClickItem(itemAt, item)) return true;
            __result = true;
            return false;
        }
        
    }
}