namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Dverger_FireMage_Soul_Power
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
                if (__instance.m_character != Player.m_localPlayer ||
                    skill is not Skills.SkillType.ElementalMagic) return;

                var eff = Player.m_localPlayer.GetEffectPower<Dverger_FireMage_Soul_Power.Config>(
                    "Dverger Fire Mage Soul Power");

                if (eff.Value > 0)
                {
                    hitData.m_damage.m_fire *= (1 + eff.Value / 100f);
                }
            }
        }
    }
}