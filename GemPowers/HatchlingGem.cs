namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public class Hatchling_VFX_Component : MonoBehaviour
    {
        private HashSet<Character> list = new();
        private ZNetView znv;
        private float DamageValue;

        private void OnTriggerEnter(Collider other)  
        { 
            if (!znv.IsOwner()) return;
            if (other.TryGetComponent(out Character c))
            { 
                if (!list.Contains(c))
                {  
                    if (EnemyCondition(c))
                    {
                        Instantiate(Hatchling_Soul_Power.VFX2, c.transform.position, Quaternion.identity);
                        HitData hit = new(); 
                        hit.m_attacker = Player.m_localPlayer.GetZDOID();
                        hit.m_point = c.m_collider.ClosestPointOnBounds(transform.position);
                        hit.m_damage.m_damage = DamageValue;
                        c.Damage((hit));
                    }
                    list.Add(c);
                }
            }
        }

        private void Awake()
        {
            znv = GetComponent<ZNetView>();
        }

        public void Setup(Vector3 dir, float Value)
        {
            DamageValue = Value;
            StartCoroutine(Move(dir));
        }

        private IEnumerator Move(Vector3 dir)
        {
            float speed = 8f;
            float count = 0;
            while (count <= 2f)
            {
                count += Time.deltaTime;
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }

            ZNetScene.instance.Destroy(gameObject);
        }
    }


    public static class Hatchling_Soul_Power
    {
        private static GameObject VFX;
        public static GameObject VFX2;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] 
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Hatchling_VFX");
                if (!VFX.GetComponent<Hatchling_VFX_Component>()) VFX.AddComponent<Hatchling_VFX_Component>();
                VFX2 = asset.LoadAsset<GameObject>("Hatchling_VFX2");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                __instance.m_prefabs.Add(VFX2);
                __instance.m_namedPrefabs.Add(VFX2.name.GetStableHashCode(), VFX2);
            }
        }

        public struct Config
        {
            [AdditivePower] public float Value;
        }
        

        [HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]
        static class Attack_DoMeleeAttack_Patch
        {
            static void Prefix(Attack __instance)
            {
                if (__instance.m_character != Player.m_localPlayer) return; 
                Player p = Player.m_localPlayer; 
                Config Effect = p.GetEffectPower<Config>("Hatchling Soul Power");
                if (Effect.Value > 0 && !p.m_seman.GetStatusEffect(Name_Cooldown))
                {
                    StatusEffect cooldown = p.m_seman.AddStatusEffect(Name_Cooldown);
                    if (cooldown) cooldown.m_ttl = 20f;
                    Vector3 target = GameCamera.instance.transform.position + p.GetLookDir() * 100f;
                    Vector3 rot = (target - p.transform.position).normalized;
                    rot.y = 0;
                    p.transform.rotation = Quaternion.LookRotation(rot);
                    GameObject proj = Instantiate(VFX, p.transform.position + Vector3.up + p.transform.forward * 0.3f,
                        GameCamera.instance.transform.rotation);
                    Vector3 debug = (target - proj.transform.position).normalized;
                    proj.GetComponent<Hatchling_VFX_Component>().Setup(debug, p.GetCurrentWeapon().GetDamage().GetTotalBlockableDamage() * Effect.Value / 100f);

                }
            }
        }
        
        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_Hatchling";
        private const string Name_Cooldown_Localize = "$soulcatcher_hatchling_gem $soulcatcher_cooldown";

        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;
            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["HatchlingGem"];
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
