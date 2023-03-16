namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class TheQueen_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }


        [HarmonyPatch(typeof(SEMan), nameof(SEMan.OnDamaged))]
        static class SEMan_ModifyEitrRegen_Patch
        {
            static void Postfix(SEMan __instance, ref HitData hit)
            {
                var eff = Player.m_localPlayer.GetEffectPower<TheQueen_Soul_Power.Config>(
                    "The Queen Soul Power");
                if (eff.Value > 0)
                {
                    hit.m_damage.m_blunt *= Mathf.Clamp01(1 - eff.Value / 100f);
                    hit.m_damage.m_slash *= Mathf.Clamp01(1 - eff.Value / 100f);
                    hit.m_damage.m_pierce *= Mathf.Clamp01(1 - eff.Value / 100f);
                }
            }
        }
    }
}