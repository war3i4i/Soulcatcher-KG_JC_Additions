namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class GoblinShaman_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if(!Player.m_localPlayer) return;
              if(__instance != Player.m_localPlayer || !hit.m_ranged) return;
              float Effect = Player.m_localPlayer.GetEffectPower<Config>("GoblinShaman Soul Power").Value;
              if (Effect > 0)
              {
                  hit.ApplyModifier(Mathf.Clamp01(1 - Effect / 100f));
              }
            } 
            
        }
    }
}