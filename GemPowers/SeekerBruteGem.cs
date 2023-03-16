namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Seeker_Brute_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }
        
        [HarmonyPatch(typeof(Character),nameof(Character.ApplyPushback), typeof(HitData))]
        static class Character_ApplyPushback_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (__instance != Player.m_localPlayer) return;
                var eff = Player.m_localPlayer.GetEffectPower<Seeker_Brute_Soul_Power.Config>(
                    "Seeker Brute Soul Power");
                if (eff.Value > 0)
                {
                    hit.m_pushForce = 0f;
                }
            }
        }
    }
}