using Random = UnityEngine.Random;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{

    IEnumerator DelayedWarlockSpawn(GameObject orig, Vector3 pos, float time, int level)
    {
        yield return new WaitForSecondsRealtime(time);
        GameObject go = Instantiate(orig, pos, Quaternion.identity);
        if(!go) yield break;
        if (go.GetComponent<Tameable>())
        {
            go.GetComponent<Tameable>().Tame();
        }

        if (go.GetComponent<Character>())
        {
            go.GetComponent<Character>().SetLevel(level);
        }
        Instantiate(Skeleton_Soul_Power.VFX, go.transform.position, Quaternion.identity);
    }
    
    public static class Skeleton_Soul_Power
    {
        private static readonly int Tool_Tier = "Skeleton_Soul_Power".GetStableHashCode();
        
        public static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources 
        {
            static void Postfix(ZNetScene __instance) 
            {
                VFX = asset.LoadAsset<GameObject>("Skeleton_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX); 
            } 
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (__instance.IsPlayer()|| __instance.IsTamed() || !__instance.gameObject.GetComponent<Tameable>() || hit.GetAttacker() != Player.m_localPlayer) return;
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Skeleton Soul Power");
                if (Effect.Chance > 0)
                {
                    int random = Random.Range(0, 101);
                    if (random <= Effect.Chance)
                    {
                        hit.m_toolTier = Tool_Tier; 
                        hit.m_pushForce = (int)Effect.Value; 
                    }
                }
            }
        }
        

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        static class Character_RPC_Damage_Patch
        {
            static void Postfix(Character __instance, HitData hit)
            {
                if (__instance.GetHealth() <= 0 && hit.m_toolTier == Tool_Tier)
                {
                    string prefab = Utils.GetPrefabName(__instance.gameObject);
                    float time = 2f;
                    int level = Mathf.Max(1,__instance.GetLevel() - (int)hit.m_pushForce);
                    GameObject go = ZNetScene.instance.GetPrefab(prefab);
                    if (go)
                    {
                        Player.m_localPlayer.StartCoroutine(_thistype.DelayedWarlockSpawn(go, __instance.gameObject.transform.position, time, level));
                    }
                }
            }
        }

        
        public struct Config
        {
            [MaxPower] public float Chance;
            [MinPower] public float Value;
        }

        
    }
}