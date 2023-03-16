namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Ulv_Soul_Power
    {
        private static GameObject VFX;
        public static int Script_Layermask;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Ulv_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
            }
        }

        public struct Config
        {
            [MinPower] public float Cooldown;
        }
        
        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.SetTrigger))]
        public static class ZSyncAnimation_SetTrigger_Patch
        {
            static bool Prefix(ZSyncAnimation __instance, string name)
            {
                
                if (!Player.m_localPlayer || __instance.m_animator != Player.m_localPlayer.m_animator) return true;
                if (name == "dodge")
                {
                    Player p = Player.m_localPlayer;
                    Config Effect = p.GetEffectPower<Config>("Ulv Soul Power");
                    if (Effect.Cooldown > 0 && !p.m_seman.GetStatusEffect(Name_Cooldown))
                    {
                        p.GetComponent<Collider>().enabled = false;
                        bool castHit = Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward,
                            out var raycast, 60f, Script_Layermask);
                        p.GetComponent<Collider>().enabled = true;
                        if (castHit && raycast.collider && raycast.collider.GetComponentInParent<Character>() is {} c)
                        {
                            if (Vector3.Distance(raycast.point, p.transform.position) > 40f)
                            {
                                return true;
                            }

                            var target = raycast.point;
                            var oppositeVec = target +
                                              (target - new Vector3(p.transform.position.x, target.y,
                                                  p.transform.position.z)).normalized * 3f;
                            ZoneSystem.instance.FindFloor(oppositeVec, out float height);
                            if (oppositeVec.y - height <= 3f)
                            {
                                oppositeVec.y = height;
                            }

                            Instantiate(VFX, p.transform.position + Vector3.up, Quaternion.identity);
                            Instantiate(VFX, oppositeVec + Vector3.up, Quaternion.identity);
                            p.transform.position = oppositeVec;
                            var look = (target - oppositeVec).normalized;
                            look.y = 0;
                            p.transform.rotation = Quaternion.LookRotation(look);
                            p.m_lookYaw = Quaternion.LookRotation(look);
                            p.m_lookPitch = 0;

                            float damage = p.GetCurrentWeapon().GetDamage().GetTotalBlockableDamage() * 1.5f;
                            HitData hit = new();
                            hit.m_damage.m_lightning = damage;
                            hit.m_skill = Skills.SkillType.None;
                            hit.SetAttacker(p);
                            hit.m_point = target;
                            c.Damage(hit);

                            StatusEffect cooldown = p.m_seman.AddStatusEffect(Name_Cooldown);
                            if (cooldown) cooldown.m_ttl = Effect.Cooldown;
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        
        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_Ulv";
        private const string Name_Cooldown_Localize = "$soulcatcher_ulv_gem $soulcatcher_cooldown";

        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;
            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["UlvGem"];
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