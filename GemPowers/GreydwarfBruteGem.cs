namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class GreydwarfBrute_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(ref HitData hit)
            {
                if (hit.GetAttacker() != Player.m_localPlayer) return;
                var Effect = Player.m_localPlayer.GetEffectPower<Config>("GreydwarfBrute Soul Power");
                if (Effect.Value > 0)
                {
                    hit.m_staggerMultiplier += Effect.Value;
                }
            }
            
        }
        
    }
}