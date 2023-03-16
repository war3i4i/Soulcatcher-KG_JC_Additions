using Random = UnityEngine.Random;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static bool EnemyCondition(Character c)
    {
        if (c == Player.m_localPlayer) return false;
        if (c.IsPlayer())
        {
            return Player.m_localPlayer.IsPVPEnabled() && c.IsPVPEnabled();
        }
        return !c.m_baseAI || c.m_baseAI.IsEnemy(Player.m_localPlayer);
    }
 
    
    public static class Troll_Soul_Power
    {
        private static GameObject VFX;
        
        [HarmonyPatch(typeof(ZNetScene),nameof(ZNetScene.Awake))]
        static class TrollGem_Resources
        {
           
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("TrollGem_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct Config
        {
            [AdditivePower] public float Value;
            [MaxPower] public float Chance; 
        } 
 

        [HarmonyPatch(typeof(Character),nameof(Character.Damage))] 
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, HitData hit)
            {
                if (hit.GetAttacker() != Player.m_localPlayer || hit.m_ranged) return;
                Player p = Player.m_localPlayer;
                Config Effect = p.GetEffectPower<Config>("Troll Soul Power");
                float effectPower = Effect.Value;
                if(effectPower <= 0) return;
                
                float randomValue = Random.Range(0, 100);
                if (randomValue > Effect.Chance) return;
                effectPower /= 100f;
                Instantiate(VFX, __instance.transform.position, Quaternion.identity);
                List<Character> list = new List<Character>();
                Character.GetCharactersInRange(__instance.transform.position, 4f, list);
                foreach (Character c in list)
                {
                    if (!EnemyCondition(c)) continue;
                    HitData newHit = new HitData();
                    newHit.m_damage.m_damage += hit.GetTotalDamage() * effectPower;
                    newHit.m_skill = Skills.SkillType.None;
                    newHit.m_ranged = true;
                    newHit.SetAttacker(p);
                    newHit.m_point = c.m_collider.ClosestPointOnBounds(p.transform.position) + Vector3.up;
                    c.Damage(newHit);
                }

            }
            
        }
        
    }
}