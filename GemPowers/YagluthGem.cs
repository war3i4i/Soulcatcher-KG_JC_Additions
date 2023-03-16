namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Yagluth_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value; 
        }  
        
        [HarmonyPatch(typeof(Character),nameof(Character.ApplyDamage))]
        static class Character_ApplyDamage_Patch 
        {
            static void Prefix(Character __instance, HitData hit)
            {
                if(__instance != Player.m_localPlayer) return;
                float Effect = Player.m_localPlayer.GetEffectPower<Config>("Yagluth Soul Power").Value;
                if (Effect > 0)
                {
                    hit.m_damage.m_fire *= Mathf.Clamp01(1 - Effect / 100f);
                }
            }
        }
        
    }
}