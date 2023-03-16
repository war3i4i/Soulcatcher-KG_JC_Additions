namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Bonemass_Soul_Power
    {
        public struct Config
        { 
            [AdditivePower] public float Value;
        } 
        
        [HarmonyPatch(typeof(Character),nameof(Character.ApplyDamage))]
        private static class Character_ApplyDamage_Patch
        {
            static void Prefix(Character __instance, HitData hit)
            {
                if(__instance != Player.m_localPlayer) return; 
                float Effect = Player.m_localPlayer.GetEffectPower<Config>("Bonemass Soul Power").Value;
                if (Effect > 0)
                {
                    hit.m_damage.m_poison *= Mathf.Clamp01(1 - Effect / 100f);
                }
            }
        }
        
    }
}