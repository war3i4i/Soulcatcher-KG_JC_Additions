namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public class SE_SoulcatcherTarBlob : StatusEffect
    {
        public float Reduction;

        public SE_SoulcatcherTarBlob()
        {
            m_tooltip = "Soulcatcher Tar Blob";
            m_icon = Gem.ActiveIcons["TarBlobGem"];
            m_name = "Soulcatcher Tar Blob";
            m_ttl = 10;

            m_startEffects = new EffectList
            {
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = TarBlob_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }

        public override void Setup(Character character)
        {
            base.Setup(character);
            character.m_nview.m_zdo.Set("Soulcatcher TarBlob", true);
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            if (IsDone())
            {
                m_character.m_nview.m_zdo.Set("Soulcatcher TarBlob", false);
            }
        }
    }


    public static class TarBlob_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Duration;
            [MinPower] public float Cooldown;
        }

        public static Sprite Icon;
        public static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class TrollGem_Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("TarBlob_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                Icon = asset.LoadAsset<GameObject>("TarBlobGem").GetComponent<ItemDrop>().m_itemData.GetIcon();
            }
        }

        private static bool IsTarBlob(Character p) => p.m_nview.m_zdo.GetBool("Soulcatcher TarBlob");

        [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.CanSeeTarget), typeof(Character))]
        static class BaseAI_CanSeeTarget_Patch
        {
            static void Postfix(Character target, ref bool __result)
            {
                if (target.IsPlayer() && IsTarBlob(target))
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.CanHearTarget), typeof(Character))]
        static class BaseAI_CanHearTarget_Patch
        {
            static void Postfix(Character target, ref bool __result)
            {
                if (target.IsPlayer() && IsTarBlob(target))
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
        static class Attack_Start_Patch
        {
            static void Postfix(Humanoid character)
            {
                if (character != Player.m_localPlayer) return;
                Player.m_localPlayer.m_seman.RemoveStatusEffect("SoulcatcherTarBlob");
                Player.m_localPlayer.m_nview.m_zdo.Set("Soulcatcher TarBlob", false);
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.SetCrouch))]
        static class Player_SetCrouch_Patch
        {
            static bool Prefix(bool crouch)
            {
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("TarBlob Soul Power");
                if (Effect.Duration > 0 && crouch && !Player.m_localPlayer.m_seman.GetStatusEffect(Name_Cooldown))
                {
                    StatusEffect cooldown = Player.m_localPlayer.m_seman.AddStatusEffect(Name_Cooldown);
                    if (cooldown) cooldown.m_ttl = Effect.Cooldown;
                    StatusEffect SE_Effect = Player.m_localPlayer.m_seman.AddStatusEffect("SoulcatcherTarBlob");
                    if (SE_Effect)
                    {
                        ((SE_SoulcatcherTarBlob)SE_Effect).m_ttl = Effect.Duration;
                    }

                    return false;
                }

                return true;
            }
        }

        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_TarBlob";
        private const string Name_Cooldown_Localize = "$soulcatcher_tarblob_gem $soulcatcher_cooldown";

        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherTarBlob"))
            {
                SE_SoulcatcherTarBlob se = ScriptableObject.CreateInstance<SE_SoulcatcherTarBlob>();
                se.name = "SoulcatcherTarBlob";
                odb.m_StatusEffects.Add(se);
            }


            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["TarBlobGem"];
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