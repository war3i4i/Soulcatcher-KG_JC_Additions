namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class StoneGolem_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }
            
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        private static class PaladinArmorGet
        {
            private static void Postfix(ref float __result)
            {
                __result *= 1 + Player.m_localPlayer.GetEffectPower<Config>("StoneGolem Soul Power").Value / 100f;
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        private static class PaladinDamageNerf
        {
            private static void Prefix(ref HitData hit)
            {
                if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return; 
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("StoneGolem Soul Power");
                if (Effect.Value > 0)
                {
                    float modifier = Mathf.Clamp01(1 - Effect.Value / 200f);
                    hit.ApplyModifier(modifier);
                }
            }
        }


        
    }
}
