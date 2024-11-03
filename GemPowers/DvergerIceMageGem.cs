namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Dverger_IceMage_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyAttack))]
        static class SEMan_ModifyAttack_Patch
        {
            static void Postfix(SEMan __instance, Skills.SkillType skill, ref HitData hitData)
            {
                if (__instance.m_character != Player.m_localPlayer) return;

                var eff = Player.m_localPlayer.GetEffectPower<Dverger_IceMage_Soul_Power.Config>(
                    "Dverger Ice Mage Soul Power");

                if (eff.Value > 0)
                {
                    hitData.m_damage.m_frost *= (1 + eff.Value / 100f);
                }
            }
        }
    }
}