using ItemDataManager;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static GameObject Lantern;
    private static Transform LanternTransform;
    private static Action<SoulComponent> OnSoulGain;
    private static Action OnLanternEffectUpdate;
    private static bool LanternMainCondition;
    private static ConfigEntry<int> MaxSouls;


    private static void PrepareLantern()
    {
        Lantern = asset.LoadAsset<GameObject>("LanternSoulcatcher");
        Lantern.GetComponent<ItemDrop>().m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Shield;
        Lantern.GetComponent<ItemDrop>().m_itemData.Data().Add<LanternComponent>();
        OnSoulGain += CaptureSoul;
        OnLanternEffectUpdate += UpdateLanternEffect;
        MaxSouls = config("Souls", "MaxSouls", 500, "Max number of souls that can be captured by the lantern.");
    }

    private static LanternComponent FindLantern(out bool leftHand)
    {
        leftHand = false;
        if (!Player.m_localPlayer) return null;
        ItemDrop.ItemData leftItem = Player.m_localPlayer.m_leftItem;
        if (leftItem?.Data().Get<LanternComponent>() is { } lanternComponent)
        {
            leftHand = true;
            return lanternComponent;
        }

        ItemDrop.ItemData rightItem = Player.m_localPlayer.m_rightItem;
        if (rightItem?.Data().Get<LanternComponent>() is { } lanternComponent2)
        {
            return lanternComponent2;
        }

        return null;
    }

    private static bool HasLantern()
    {
        if (!Player.m_localPlayer) return false;
        return HasItem(Lantern.name);
    }

    private static void UpdateLanternEffect()
    {
        if (FindLantern(out bool leftHand) is { } lantern)
        {
            ZPackage pkg = new ZPackage();
            pkg.Write(leftHand);
            pkg.Write(lantern.SoulCount);

            Player.m_localPlayer.m_nview.InvokeRPC(ZNetView.Everybody, "SoulcatcherKG UpdateLanternEffect", pkg);
        }
    }

    private static void CaptureSoul(SoulComponent soul)
    {
        if (FindLantern(out _) is { } lantern)
        {
            lantern.AddSoul(soul.Prefab, soul.DoubleSoul ? 2 : 1);
        }
    }

    private void FixedUpdate()
    {
        if (!Player.m_localPlayer) return;

        if (FindLantern(out bool leftHand))
        {
            LanternTransform = leftHand
                ? Player.m_localPlayer.m_visEquipment.m_leftHand
                : Player.m_localPlayer.m_visEquipment.m_rightHand;
            LanternMainCondition = Player.m_localPlayer.IsBlocking();
        }
        else
        {
            LanternTransform = Player.m_localPlayer.m_visEquipment.m_rightHand;
            LanternMainCondition = false;
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch
    {
        static void Postfix(ZNetScene __instance)
        {
            __instance.m_prefabs.Add(Lantern);
            __instance.m_namedPrefabs.Add(Lantern.name.GetStableHashCode(), Lantern);
        }
    }


    private static void AddLanternToODB()
    {
        if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
            ObjectDB.instance.GetItemPrefab("Amber") == null)
            return;

        if (ObjectDB.instance.GetItemPrefab(Lantern.name.GetStableHashCode()) != null) return;

        ObjectDB.instance.m_items.Add(Lantern);
        ObjectDB.instance.m_itemByHash[Lantern.name.GetStableHashCode()] = Lantern;

        List<Piece.Requirement> reqs = new()
        {
            new()
            {
                m_resItem = ObjectDB.instance.GetItemPrefab("GreydwarfEye").GetComponent<ItemDrop>(),
                m_amount = 50,
                m_amountPerLevel = 0,
                m_recover = false
            },
            new()
            {
                m_resItem = ObjectDB.instance.GetItemPrefab("Crystal").GetComponent<ItemDrop>(),
                m_amount = 20,
                m_amountPerLevel = 0,
                m_recover = false
            },
            new()
            {
                m_resItem = ObjectDB.instance.GetItemPrefab("SurtlingCore").GetComponent<ItemDrop>(),
                m_amount = 20,
                m_amountPerLevel = 0,
                m_recover = false
            },
            new()
            {
                m_resItem = ObjectDB.instance.GetItemPrefab("Iron").GetComponent<ItemDrop>(),
                m_amount = 10,
                m_amountPerLevel = 0,
                m_recover = false
            }
        };

        Recipe lanternRecipe =
            CreateRecipe($"Recipe_{Lantern.name}", 1, 1, Lantern.GetComponent<ItemDrop>(), reqs.ToArray());

        if (!ObjectDB.instance.m_recipes.Find(r => r.name == lanternRecipe.name))
        {
            ObjectDB.instance.m_recipes.Add(lanternRecipe);
        }

        ObjectDB.instance.UpdateItemHashes();
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPriority(Priority.Last)]
    public static class DB_Patch
    {
        private static void Postfix()
        {
            AddLanternToODB();
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    [HarmonyPriority(Priority.Last)]
    public static class DB_Patch2
    {
        private static void Postfix()
        {
            AddLanternToODB();
        }
    }

    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData),
        typeof(int), typeof(bool))]
    static class ItemDrop__Patch
    {
        static void Postfix(ItemDrop.ItemData item, ref string __result)
        {
            if (item.Data().Get<LanternComponent>() is { } lantern)
            {
                __result += lantern.TooltipText();
            }
        }
    }


    public class LanternComponent : ItemDataManager.ItemData
    {
        Dictionary<string, int> CreatureSouls = new();

        private static readonly Dictionary<string, string> CachedLocalizedNames = new();


        public override void Load()
        {
            if (string.IsNullOrEmpty(Value)) return;
            CreatureSouls = JSON.ToObject<Dictionary<string, int>>(Value);
        }

        public override void Save()
        {
            Value = JSON.ToJSON(CreatureSouls);
        }


        public static string GetLocalizedName(string prefab, bool colored = true)
        {
            if (CachedLocalizedNames.TryGetValue(prefab, out string LOCALIZED))
            {
                if (!colored)
                    return LOCALIZED;
                return $"<color={SoulColors[prefab].ToHex()}>" + LOCALIZED + "</color>";
            }

            GameObject go = ZNetScene.instance.GetPrefab(prefab);
            if (!go) return "None";
            string localized = Localization.instance.Localize(go.GetComponent<Character>().m_name);
            CachedLocalizedNames[prefab] = localized;
            if (!colored)
                return localized;
            return $"<color={SoulColors[prefab].ToHex()}>" + localized + "</color>";
        }

        public string TooltipText()
        {
            string text = Localization.instance.Localize("\n\n<color=#00FFFF>$soulcatcher $soulcatcher_souls:</color>");
            int c = 0;
            foreach (KeyValuePair<string, int> pair in CreatureSouls)
            {
                text += $"\n- {GetLocalizedName(pair.Key)}: {pair.Value}";
                if (++c >= 30)
                {
                    text += "\n...";
                    break;
                }
            }

            return text;
        }

        public int SoulCount => CreatureSouls.Sum(pair => pair.Value);
        public Dictionary<string, int> GetSouls() => CreatureSouls;

        public IEnumerable<KeyValuePair<string, int>> GetSortedSouls(HashSet<string> hash)
        {
            return CreatureSouls.Where(d => hash.Contains(d.Key));
        }

        public void AddSoul(string name, int howMany = 1, bool cheat = false, bool silent = false)
        {
            if (!cheat)
                Player.m_localPlayer.RaiseSkill("Soulcatcher", 4f);
            if (!cheat && SoulCount >= MaxSouls.Value)
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "You can't hold more souls");
                return;
            }

            if (CreatureSouls.TryGetValue(name, out int count))
            {
                CreatureSouls[name] = count + howMany;
            }
            else
            {
                CreatureSouls[name] = howMany;
            }

            if (!silent)
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                    $"You captured a x{howMany} soul(s) of {GetLocalizedName(name)}");

            Save();
            OnLanternEffectUpdate();
        }

        public void RemoveSouls(string name, int count)
        {
            if (CreatureSouls.TryGetValue(name, out int currentCount))
            {
                if (currentCount <= count)
                {
                    CreatureSouls.Remove(name);
                }
                else
                {
                    CreatureSouls[name] = currentCount - count;
                }
            }

            Save();
            OnLanternEffectUpdate();
        }


        public static void CombineTwoLanterns(LanternComponent target, LanternComponent source, int _fee)
        {
            float Fee = Mathf.Max(0, _fee / 100f);
            foreach (KeyValuePair<string, int> pair in source.GetSouls())
            {
                target.AddSoul(pair.Key, Mathf.CeilToInt(pair.Value * (1 - Fee)), true, true);
            }

            source.ClearSouls();
        }

        public static string GenerateLanternCombineString(LanternComponent target, LanternComponent source, int _fee)
        {
            float Fee = Mathf.Max(0, _fee / 100f);
            StringBuilder text = new("$soulcatcher_combined_souls:");
            Dictionary<string, int> firstSouls = target.GetSouls();
            Dictionary<string, int> secondSouls = source.GetSouls();
            IEnumerable<string> CombinedKeys = target.GetSouls().Keys.Concat(source.GetSouls().Keys).Distinct();
            foreach (string combinedKey in CombinedKeys)
            {
                GameObject go = ZNetScene.instance.GetPrefab(combinedKey);
                if (!go) continue;

                text.Append($"\n{GetLocalizedName(combinedKey)} : ");

                text.Append(firstSouls.TryGetValue(combinedKey, out int firstCount) ? $"{firstCount} + " : $"0 + ");

                text.Append(secondSouls.TryGetValue(combinedKey, out int secondCount)
                    ? $"{secondCount} (-{_fee}%) = "
                    : $"0 = ");

                int resultAmount = Mathf.CeilToInt(firstCount + secondCount * (1 - Fee));
                text.Append($"{resultAmount}");
            }

            return Localization.instance.Localize(text.ToString());
        }


        private void ClearSouls()
        {
            CreatureSouls.Clear();
            Save();
        }

        public void CheatGetSouls()
        {
            foreach (KeyValuePair<string, string> convertion in SoulConvertions)
            {
                AddSoul(convertion.Key, 100, true);
            }
        }

        public Vector3 GetSoulColor()
        {
            if (CreatureSouls.Count == 0)
                return Vector3.one;
            int overallSouls = 0;
            float r = 0, g = 0, b = 0;
            foreach (KeyValuePair<string, int> pair in CreatureSouls)
            {
                if (!SoulColors.TryGetValue(pair.Key, out Color color)) continue;
                r += color.r * pair.Value;
                g += color.g * pair.Value;
                b += color.b * pair.Value;
                overallSouls += pair.Value;
            }

            return new Vector3(r / overallSouls, g / overallSouls, b / overallSouls);
        }

        public static implicit operator bool(LanternComponent component)
        {
            return component != null;
        }
    }


    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    static class Player_Start_Patch
    {
        static void Postfix(Player __instance)
        {
            __instance.m_nview.Register("SoulcatcherKG UpdateLanternEffect", new Action<long, ZPackage>((
                (_, pkg) =>
                {
                    bool leftHand = pkg.ReadBool();
                    int soulCount = pkg.ReadInt();
                    GameObject gameObject = leftHand
                        ? __instance.m_visEquipment.m_leftItemInstance
                        : __instance.m_visEquipment.m_rightItemInstance;

                    if (gameObject)
                    {
                        if (gameObject.transform.Find("LanternSmoke") is { } psTransform)
                        {
                            float rate = 1f / MaxSouls.Value;
                            Color c = psTransform.GetComponent<ParticleSystem>().startColor;
                            psTransform.GetComponent<ParticleSystem>().startColor = new Color(c.r,
                                c.g, c.b,
                                Mathf.Clamp01(rate * soulCount));
                        }
                    }
                })));
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    static class Humanoid_EquipItem_Patch
    {
        static IEnumerator DelayedAction(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }


        static void Postfix(Humanoid __instance, ref bool __result)
        {
            if (__instance == Player.m_localPlayer && __result)
            {
                __instance.StartCoroutine(DelayedAction(OnLanternEffectUpdate));
            }
        }
    }

    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachItem))]
    static class VisEquipment_AttachItem_Patch
    {
        private static int? HashCode;

        static void Postfix(VisEquipment __instance, Transform joint, int itemHash, ref GameObject __result)
        {
            HashCode ??= Lantern.name.GetStableHashCode();
            if (joint == __instance.m_backTool && itemHash == HashCode)
            {
                __result.transform.Rotate(Vector3.right, 180);
            }
        }
    }


    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.GetHoverText))]
    static class ItemDrop_GetHoverText_Patch
    {
        static void Postfix(ItemDrop __instance, ref string __result)
        {
            if (__instance.m_itemData?.Data()?.Get<LanternComponent>() is { } lanternComponent)
            {
                __result += lanternComponent.TooltipText();
            }
        }
    }

    [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.CanAttach))]
    static class ItemStand_CanAttach_Patch
    {
        static void Postfix(ItemDrop.ItemData item, ref bool __result)
        {
            if (item.m_dropPrefab == Lantern)
            {
                __result = true;
            }
        }
    }
}