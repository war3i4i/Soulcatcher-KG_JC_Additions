using ItemDataManager;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static Sprite NullSprite;

    private static class SoulAltarUI
    {
        private static GameObject UI;
        private static GameObject InfoElement;
        private static GameObject CraftElement;
        private static GameObject ClickEffect;
        private static GameObject ConvertionElement;

        public static Action<ItemDrop.ItemData> OnItemClick;

        private static Text GemInfoText;
        private static Text GemInfoTextName;
        private static Image LanternSlot;


        private static Transform InfoContent;
        private static Transform CraftContent;
        private static Transform ConvertionsContent;

        private static LanternComponent CurrentLantern;
        private static SoulAltarComponent CurrentSoulAltar;
        private static Gem CurrentGem;
        private static readonly List<GameObject> InfoElements = new List<GameObject>();
        private static readonly List<GameObject> CraftElements = new List<GameObject>();


        public static bool IsVisible()
        {
            return UI && UI.activeSelf;
        }

        public static void Init()
        {
            OnItemClick += ItemClick;
            UI = Instantiate(asset.LoadAsset<GameObject>("SoulcatcherUI"));
            DontDestroyOnLoad(UI);
            InfoElement = asset.LoadAsset<GameObject>("SoulcatcherInfoElement");
            CraftElement = asset.LoadAsset<GameObject>("SoulcatcherCraftElement");
            NullSprite = asset.LoadAsset<Sprite>("NullSpriteSoulcatcher");
            ClickEffect = asset.LoadAsset<GameObject>("AltarClickEffect");
            ConvertionElement = asset.LoadAsset<GameObject>("ConvertionInstantiate");
            GemInfoText = UI.transform.Find("Canvas/Background/GemInfoView/Text").GetComponent<Text>();
            GemInfoTextName = UI.transform.Find("Canvas/Background/GemInfoView/Name").GetComponent<Text>();
            LanternSlot = UI.transform.Find("Canvas/Background/NeededItem/Icon").GetComponent<Image>();
            InfoContent = UI.transform.Find("Canvas/Background/InfoView/Scroll View/Viewport/Content");
            CraftContent = UI.transform.Find("Canvas/Background/CraftView/Scroll View/Viewport/Content");
            ConvertionsContent = UI.transform.Find("Canvas/InfoTab/InfoView/Scroll View/Viewport/Content");
            UI.transform.Find("Canvas/Background/StatusButton").GetComponent<Button>().onClick
                .AddListener(ClickCraftButton);
            UI.SetActive(false);
            LanternCombinatorUI.Init();
        }

        private static void ClickCraftButton()
        {
            AUsrc.PlayOneShot(AdditionalCraftSound);
            if (CurrentGem == null || CurrentLantern == null || !CurrentGem.CanBuy(CurrentLantern)) return;
            CurrentLantern.RemoveSouls(CurrentGem.CostPrefab, CurrentGem.CostCount);
            CurrentSoulAltar.StartCraft(CurrentGem.Prefab.name, CurrentGem.CraftDuration,
                SoulColors[CurrentGem.CostPrefab]);
            Hide();
            InventoryGui.instance.Hide();
        }


        private static void ItemClick(ItemDrop.ItemData item)
        {
            if (!IsVisible()) return;
            InventoryGui.instance.SetupDragItem(null, null, 1);
            if (item?.Data().Get<LanternComponent>() is { } _lantern)
            {
                InitData(_lantern);
            }
            else
            {
                Default();
            }
        }


        private static void Default()
        {
            CurrentGem = null;
            CurrentLantern = null;
            GemInfoText.text = "";
            GemInfoTextName.text = "";
            LanternSlot.sprite = NullSprite;
            InfoElements.ForEach(Destroy);
            CraftElements.ForEach(Destroy);
            InfoElements.Clear();
            CraftElements.Clear();
        }

        private static void InitData(LanternComponent _lantern)
        {
            Default();
            CurrentLantern = _lantern;
            LanternSlot.sprite = _lantern.Item.GetIcon();

            Dictionary<string, int> souls = _lantern.GetSouls();

            foreach (KeyValuePair<string, int> soul in souls)
            {
                GameObject go = ZNetScene.instance.GetPrefab(soul.Key);
                if (!go) continue;
                GameObject element = Instantiate(InfoElement, InfoContent);
                element.transform.Find("MonsterName").GetComponent<Text>().text =
                    Localization.instance.Localize(go.GetComponent<Character>().m_name);
                element.transform.Find("Count").GetComponent<Text>().text = soul.Value.ToString();
                string ConvertText = "";
                if (SoulConvertions.ContainsKey(soul.Key))
                {
                    GameObject ConvertItem = ZNetScene.instance.GetPrefab(SoulConvertions[soul.Key]);
                    if (ConvertItem && ConvertItem.GetComponent<ItemDrop>() is { } item)
                    {
                        ConvertText = Localization.instance.Localize("$soulcatcher_convertsinto: ") +
                                      Localization.instance.Localize(item.m_itemData.m_shared.m_name);
                    }
                }

                element.transform.Find("CanBeConverted").GetComponent<Text>().text = ConvertText;
                Color c = Color.white;
                if (SoulColors.ContainsKey(soul.Key))
                {
                    c = SoulColors[soul.Key];
                }

                element.transform.Find("border").GetComponent<Image>().color = c;
                element.transform.Find("MonsterName/Image").GetComponent<Image>().color = c;
                element.transform.Find("Count/Image").GetComponent<Image>().color = c;
                element.transform.Find("CanBeConverted/Image").GetComponent<Image>().color = c;
                ScreenshotManager.MakeSprite(go);
                element.transform.Find("Icon/IconItem").GetComponent<Image>().sprite =
                    ScreenshotManager.GetSprite(go.name, PLACEHOLDERMONSTERICON);


                InfoElements.Add(element);
            }

            int counter = 0;
            foreach (Gem gem in MainGems.Where(g => g.CanBuy(CurrentLantern)))
            {
                GameObject costPrefab = ZNetScene.instance.GetPrefab(gem.CostPrefab);
                if (!costPrefab) continue;
                GameObject element = Instantiate(CraftElement, CraftContent);
                element.transform.Find("Icon/IconItem").GetComponent<Image>().sprite = gem.GetIcon;
                element.transform.Find("border").GetComponent<Image>().color = SoulColors[gem.CostPrefab];
                element.transform.Find("GemName").GetComponent<Text>().text = gem.Name;
                element.transform.Find("Price").GetComponent<Text>().text =
                    $"{Localization.instance.Localize("$soulcatcher_need")}: x{gem.CostCount} {Localization.instance.Localize(costPrefab.GetComponent<Character>().m_name)}";
                CraftElements.Add(element);
                int c1 = counter;
                element.GetComponent<Button>().onClick.AddListener(() => ChooseGem(gem, c1));
                ++counter;
            }
        }


        private static void ChooseGem(Gem gem, int index)
        {
            AUsrc.Play();
            Instantiate(ClickEffect, CraftElements[index].transform);
            CurrentGem = gem;
            foreach (GameObject element in CraftElements)
            {
                element.GetComponent<Image>().color = Color.white;
            }

            CraftElements[index].GetComponent<Image>().color = Color.green;
            ItemDrop.ItemData data = gem.Prefab.GetComponent<ItemDrop>().m_itemData.Clone();
            data.m_shared = (ItemDrop.ItemData.SharedData)AccessTools
                .Method(typeof(ItemDrop.ItemData.SharedData), "MemberwiseClone")
                .Invoke(data.m_shared, Array.Empty<object>());
            GemInfoTextName.text = gem.Name + Localization.instance.Localize(
                $"\n<color=#00FF00>$soulcatcher_craftduration:</color> <color=yellow>{FormatTime(gem.CraftDuration)}</color>");
            string tooltip = Localization.instance.Localize(data.GetTooltip());
            GemInfoText.text = tooltip;
        }


        public static void Show(SoulAltarComponent _altar)
        {
            CurrentSoulAltar = _altar;
            Default();
            InventoryGui.instance.Show(null);
            UI.SetActive(true);
            UI.transform.Find("Canvas/InfoTab").gameObject.SetActive(false);
        }

        public static void Hide()
        {
            CurrentSoulAltar = null;
            CurrentLantern = null;
            UI.SetActive(false);
        }


        private static readonly List<GameObject> ConvertionsElements = new();
        private static DateTime LastConvertionsUIChange;

        public static void FillConvertionsUI()
        {
            if ((DateTime.Now - LastConvertionsUIChange).TotalSeconds < 5) return;
            LastConvertionsUIChange = DateTime.Now;
            ConvertionsElements.ForEach(Destroy);
            ConvertionsElements.Clear();
            foreach (Gem convertion in MainGems)
            { 
                if (!convertion.IsCraftable.Value) continue;
                GameObject key = ZNetScene.instance.GetPrefab(convertion.CostPrefab);
                GameObject value = convertion.Prefab;
                if (!key || !value) continue;
                GameObject element = Instantiate(ConvertionElement, ConvertionsContent);
                element.transform.Find("Background/NeededItemText").GetComponent<Text>().text =
                    LanternComponent.GetLocalizedName(convertion.CostPrefab) + $"{convertion.Name_Postfix} x{convertion.CostCount}";
                element.transform.Find("Background/ResultItemText").GetComponent<Text>().text =
                    Localization.instance.Localize(value.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
                ScreenshotManager.MakeSprite(key);
                element.transform.Find("Background/NeededItem/Icon").GetComponent<Image>().sprite =
                    ScreenshotManager.GetSprite(key.name, PLACEHOLDERMONSTERICON);
                element.transform.Find("Background/ResultItem/Icon").GetComponent<Image>().sprite =
                    value.GetComponent<ItemDrop>().m_itemData.GetIcon();

                ScreenshotManager_Ghost.MakeSprite(key, true);
                element.transform.Find("Background/GhostItem/Icon").GetComponent<Image>().sprite =
                    ScreenshotManager_Ghost.GetSprite(key.name, PLACEHOLDERMONSTERICON);
                element.gameObject.AddComponent<ConvertionElementComponent>();
                ConvertionsElements.Add(element);
            }
        }
    }


    public class ConvertionElementComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Transform on, off;

        private void Awake()
        {
            on = transform.Find("Background/NeededItem");
            off = transform.Find("Background/GhostItem");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            on.gameObject.SetActive(false);
            off.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            on.gameObject.SetActive(true);
            off.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
    static class Player_SetLocalPlayer_Patch
    {
        static void Postfix()
        {
            SoulAltarUI.FillConvertionsUI();
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.SetupDragItem))]
    static class InventoryGui_SetupDragItem_Patch
    {
        static void Postfix(InventoryGui __instance)
        {
            if (__instance.m_dragGo && __instance.m_dragItem != null)
            {
                SoulAltarUI.OnItemClick(__instance.m_dragItem);
            }
        }
    }

    [HarmonyPatch(typeof(TextInput), nameof(TextInput.IsVisible))]
    static class Menu_IsVisible_Patch
    {
        static void Postfix(ref bool __result)
        {
            if (SoulAltarUI.IsVisible() || LanternCombinatorUI.IsVisible()) __result = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SoulAltarUI.IsVisible())
                SoulAltarUI.Hide();

            if (LanternCombinatorUI.IsVisible())
                LanternCombinatorUI.Hide();
        }
    }
}