namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Hare_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }
        
        /*[HarmonyPatch(typeof(SEMan),nameof(SEMan.ModifyAttack))]
        static class SEMan_ModifyAttack_Patch
        {
            static void Postfix(SEMan __instance, Skills.SkillType skill, ref HitData hitData)
            {
                if(__instance.m_character != Player.m_localPlayer) return;
                var eff = Player.m_localPlayer.GetEffectPower<Hare_Soul_Power.Config>(
                    "Hare Soul Power");
                if (eff.Value > 0 && skill is Skills.SkillType.Crossbows)
                {
                    hitData.ApplyModifier(1 + eff.Value / 100f);
                }
            }
        }*/
    }
}