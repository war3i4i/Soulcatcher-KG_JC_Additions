using System.Runtime.CompilerServices;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static readonly ConditionalWeakTable<Player, Material[]> WeakMaterialTable = new();

    public class SE_SoulcatcherAbomination : StatusEffect
    {
        public float Reduction;

        public SE_SoulcatcherAbomination()
        {
            m_tooltip = "Soulcatcher Abomination";
            m_icon = Gem.ActiveIcons["AbominationGem"];
            m_name = "Soulcatcher Abomination";
            m_ttl = 6;

            m_startEffects = new EffectList
            {
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Abomination_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }

        public override void OnDamaged(HitData hit, Character attacker)
        {
            float Modifier = Mathf.Clamp01(1 - Reduction / 100f);
            hit.m_damage.m_blunt *= Modifier;
            hit.m_damage.m_pierce *= Modifier;
            hit.m_damage.m_slash *= Modifier;
            hit.m_damage.m_chop *= Modifier;
        }
    }


    public static class Abomination_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
            [MinPower] public float Cooldown;
        }

        public static Sprite Icon;
        public static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class TrollGem_Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Abomination_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                Icon = asset.LoadAsset<GameObject>("AbominationGem").GetComponent<ItemDrop>().m_itemData.GetIcon();
            }
        }


        [HarmonyPatch(typeof(Player), nameof(Player.SetCrouch))]
        static class Player_SetCrouch_Patch
        {
            static bool Prefix(bool crouch)
            {
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Abomination Soul Power");
                if (Effect.Value > 0 && crouch && !Player.m_localPlayer.m_seman.GetStatusEffect(Name_Cooldown))
                {
                    StatusEffect cooldown = Player.m_localPlayer.m_seman.AddStatusEffect(Name_Cooldown);
                    if (cooldown) cooldown.m_ttl = Effect.Cooldown;
                    StatusEffect SE_Effect = Player.m_localPlayer.m_seman.AddStatusEffect("SoulcatcherAbomination");
                    if (SE_Effect)
                    {
                        ((SE_SoulcatcherAbomination)SE_Effect).Reduction = Effect.Value;
                    }

                    return false;
                }

                return true;
            }
        }

        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_Abomination";
        private const string Name_Cooldown_Localize = "$soulcatcher_abomination_gem $soulcatcher_cooldown";


        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherAbomination"))
            {
                SE_SoulcatcherAbomination se = ScriptableObject.CreateInstance<SE_SoulcatcherAbomination>();
                se.name = "SoulcatcherAbomination";
                odb.m_StatusEffects.Add(se);
            }


            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["AbominationGem"];
                se.m_name = Name_Cooldown_Localize;
                odb.m_StatusEffects.Add(se);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDBAwake
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDBCopyOtherDB
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }
    }
}