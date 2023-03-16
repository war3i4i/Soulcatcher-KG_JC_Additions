namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Dverger_BloodMage_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifySkillLevel))]
        static class SEMan_ModifyAttack_Patch
        {
            static void Postfix(SEMan __instance, Skills.SkillType skill, ref float level)
            {
                if (__instance.m_character != Player.m_localPlayer ||
                    skill is not Skills.SkillType.BloodMagic) return;

                var eff = Player.m_localPlayer.GetEffectPower<Dverger_BloodMage_Soul_Power.Config>(
                    "Dverger Blood Mage Soul Power");

                if (eff.Value > 0)
                {
                    level += eff.Value;
                }
            }
        }
    }
}