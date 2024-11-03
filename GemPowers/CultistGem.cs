namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    

    private static List<Character> GetObjectsInBoxCollider(BoxCollider collider)
    {
        Collider[] colliders = Physics.OverlapBox(
            center: collider.transform.position + (collider.transform.rotation * collider.center),
            halfExtents: Vector3.Scale(collider.size * 0.5f, collider.transform.lossyScale),
            orientation: collider.transform.rotation,
            layerMask: Cultist_Soul_Power.LayerForCultist);
        List<Character> objectsInBox = new List<Character>();
        foreach (Collider c in colliders)
        {
            if (c.TryGetComponent(out Character character) && EnemyCondition(character))
            {
                objectsInBox.Add(character);
            }
        }
        return objectsInBox;
    }

    public class SE_SoulcatcherCultist : StatusEffect
    {
        public float Damage = 20;

        public SE_SoulcatcherCultist()
        {
            m_tooltip = "Soulcatcher Cultist";
            m_icon = Gem.ActiveIcons["CultistGem"];
            m_name = "Soulcatcher Cultist";
            m_ttl = 6.5f;
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Cultist_Soul_Power.VFX, m_randomRotation = false, m_scale = true,
                        m_childTransform = "Head"
                    }
                }
            };
        }

        private float counter = 10;
        private BoxCollider Collider;

        public override void Setup(Character character)
        {
            base.Setup(character);
            Collider = m_startEffectInstances[0].transform.Find("Effect/Collider").GetComponent<BoxCollider>();
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            counter += dt;
            if (counter >= 0.5f)
            {
                counter = 0;
                List<Character> list = GetObjectsInBoxCollider(Collider);
                foreach (Character character in list)
                {
                    HitData hit = new()
                    {
                        m_attacker = Player.m_localPlayer.GetZDOID(),
                        m_skill = Skills.SkillType.None,
                        m_point = character.m_collider.ClosestPointOnBounds(Player.m_localPlayer.transform.position) +
                                  Vector3.up
                    };
                    hit.m_damage.m_fire = Damage / 2f;
                    character.Damage(hit);
                }
            }
        }
    }


    public static class Cultist_Soul_Power
    {
        public static int LayerForCultist;
        public static GameObject VFX;
        public static Sprite Icon;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Cultist_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                Icon = asset.LoadAsset<GameObject>("CultistGem").GetComponent<ItemDrop>().m_itemData.GetIcon();
            }
        }

        public struct Config
        {
            [MaxPower] public float Value;
            [MinPower] public float Cooldown;
        }


        [HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]
        static class Attack_DoMeleeAttack_Patch
        {
            static void Prefix(Attack __instance)
            {
                if (__instance.m_character != Player.m_localPlayer) return;
                Player p = Player.m_localPlayer;
                Config Effect = p.GetEffectPower<Config>("Cultist Soul Power");
                if (Effect.Value > 0 && !p.m_seman.GetStatusEffect(Name_Cooldown.GetStableHashCode()))
                {
                    StatusEffect cooldown = p.m_seman.AddStatusEffect(Name_Cooldown.GetStableHashCode());
                    if(cooldown) cooldown.m_ttl = Effect.Cooldown;
                    StatusEffect SE_Effect = Player.m_localPlayer.m_seman.AddStatusEffect("SoulcatcherCultist".GetStableHashCode());
                    if (SE_Effect)
                    {
                        ((SE_SoulcatcherCultist)SE_Effect).Damage =
                            Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalBlockableDamage() *
                            Effect.Value / 100f;
                    }
                }
            }
        }

        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_Cultist";
        private const string Name_Cooldown_Localize = "$soulcatcher_cultist_gem $soulcatcher_cooldown";

      
        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherCultist"))
            {
                SE_SoulcatcherCultist se = ScriptableObject.CreateInstance<SE_SoulcatcherCultist>();
                se.name = "SoulcatcherCultist";
                odb.m_StatusEffects.Add(se);
            }
            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["CultistGem"];
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