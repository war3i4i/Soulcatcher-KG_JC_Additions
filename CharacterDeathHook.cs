namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    
    private static bool HasItem(string prefab)
    {
        return Player.m_localPlayer.m_inventory.m_inventory.Any(itemData => itemData.m_dropPrefab.name == prefab);
    }
    
    private static readonly Dictionary<Character, long> CharacterLastDamageList = new();

    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    static class HookAttacker
    {
        static void Prefix(Character __instance, long sender, HitData hit)
        {
            if(__instance.IsTamed() || __instance.GetHealth() <= 0) return;
            Character attacker = hit.GetAttacker();
            if (attacker)
            {
                if (attacker.IsPlayer())
                {
                    CharacterLastDamageList[__instance] = sender;
                }
                else
                {
                    if (!attacker.IsTamed())
                    {
                        CharacterLastDamageList[__instance] = 100;
                    }
                }
            }
        }

        static void Postfix(Character __instance)
        {
            if (__instance.GetHealth() <= 0f && CharacterLastDamageList.ContainsKey(__instance))
            {
                ZPackage pkg = new();
                string prefab = Utils.GetPrefabName(__instance.gameObject);
                CheckDverger(__instance, ref prefab);
                pkg.Write(prefab);
                pkg.Write(__instance.transform.position);
                pkg.Write(__instance.GetLevel());
                ZRoutedRpc.instance.InvokeRoutedRPC(CharacterLastDamageList[__instance], "Soulcatcher HookKill",
                    pkg);
                CharacterLastDamageList.Remove(__instance);
            }
        }
    }

    private static void CheckDverger(Character c, ref string prefab)
    {
        if (prefab == "DvergerMage")
        {
            var items = (c as Humanoid).m_inventory.GetAllItems();
            
            if(items.Any(x =>x.m_dropPrefab.name.Contains("DvergerStaffFire")))
            {
                prefab = "DvergerMageFire";
                return;
            }
            if (items.Any(x => x.m_dropPrefab.name.Contains("DvergerStaffIce")))
            {
                prefab = "DvergerMageIce";
                return;
            }
            if (items.Any(x => x.m_dropPrefab.name.Contains("DvergerStaffHeal")))
            {
                prefab = "DvergerMageSupport";
                return;
            }
                
        }
    }
  

    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    static class Character_ApplyDamage_Patch
    {
        static void Postfix(Character __instance)
        {
            if (__instance.GetHealth() <= 0f && CharacterLastDamageList.ContainsKey(__instance))
            {
                ZPackage pkg = new();
                string prefab = Utils.GetPrefabName(__instance.gameObject);
                CheckDverger(__instance, ref prefab);
                pkg.Write(prefab);
                pkg.Write(__instance.transform.position);
                pkg.Write(__instance.GetLevel());
                ZRoutedRpc.instance.InvokeRoutedRPC(CharacterLastDamageList[__instance], "Soulcatcher HookKill", pkg);
                CharacterLastDamageList.Remove(__instance);
            }
        }
    }
 
    [HarmonyPatch(typeof(Character), nameof(Character.OnDestroy))]
    static class Character_OnDestroy_Patch 
    {
        static void Postfix(Character __instance)
        {
            if (CharacterLastDamageList.ContainsKey(__instance)) CharacterLastDamageList.Remove(__instance);
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_QuestsInit
    {
        static void Postfix()
        {
            ZRoutedRpc.instance.Register("Soulcatcher HookKill", new Action<long, ZPackage>(KillEvent));
        }

        private static void KillEvent(long sender, ZPackage pkg)
        {
            SoulCreation(pkg.ReadString(), pkg.ReadVector3(), pkg.ReadInt());
        }
    }
}