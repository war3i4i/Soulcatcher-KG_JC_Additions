using Random = UnityEngine.Random;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public class PeriodicDamage : MonoBehaviour
    {
        private ZNetView znv;
        private float _damage;
        private int _damageTicks;

        private float counter = 100;
        private int currentTick;

        private void Awake()
        {
            znv = GetComponent<ZNetView>();
        }

        public void Setup(float damage, int damageTicks)
        {
            _damage = damage;
            _damageTicks = damageTicks;
        }
        
        void Update()
        {
            if(znv.m_zdo == null || !znv.IsOwner()) return;
            counter += Time.deltaTime;
            if (counter >= 1)
            {
                counter = 0; 
                currentTick++; 
                if(currentTick > _damageTicks)
                {
                    ZNetScene.instance.Destroy(gameObject);
                    return;
                }

                IEnumerable<Character> list = Character.GetAllCharacters().Where(c =>
                    EnemyCondition(c) && Vector3.Distance(transform.position, c.transform.position) <= 5.5f);

                foreach (Character character in list)
                {
                    HitData hit = new()
                    {
                        m_attacker = Player.m_localPlayer.GetZDOID(),
                        m_skill = Skills.SkillType.None,
                        m_point = character.m_collider.ClosestPointOnBounds(Player.m_localPlayer.transform.position) + Vector3.up
                    };
                    hit.m_damage.m_fire = _damage;
                    character.Damage(hit);
                }

 
            }
        }
    }
    
    public static class Surtling_Soul_Power
    {
        
        
        public static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources 
        {
            static void Postfix(ZNetScene __instance) 
            {
                VFX = asset.LoadAsset<GameObject>("Surtling_VFX");
                if (!VFX.GetComponent<PeriodicDamage>()) VFX.AddComponent<PeriodicDamage>();
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX); 
            }
        }
        
        
        

        [StructLayout(LayoutKind.Sequential)]
        public struct Config
        { 
            [AdditivePower] public float Value;
            [MinPower] public float Cooldown;
        }

        private static int Chance = 10;
        private static long lastCD;
        
        [HarmonyPatch(typeof(Character),nameof(Character.Damage))] 
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, HitData hit) 
            { 
                if (hit.GetAttacker() != Player.m_localPlayer) return;
                Player p = Player.m_localPlayer;
                Config Effect = p.GetEffectPower<Config>("Surtling Soul Power");
                float effectPower = Effect.Value;
                if(effectPower <= 0 || (long)EnvMan.instance.m_totalSeconds - lastCD <= Effect.Cooldown) return;
                float randomValue = Random.Range(0, 100);
                if (randomValue > Chance) return; 
                lastCD = (long)EnvMan.instance.m_totalSeconds; 
                effectPower /= 100f;
                GameObject go = Instantiate(VFX, __instance.transform.position, Quaternion.identity);
                go.GetComponent<PeriodicDamage>().Setup(Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalBlockableDamage() * effectPower, 8);
            }
            
        }
        
    }
}