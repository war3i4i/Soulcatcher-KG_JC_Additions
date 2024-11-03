namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Tick_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }


        /*[HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyEitrRegen))]
        static class SEMan_ModifyEitrRegen_Patch
        {
            static void Postfix(ref float eitrMultiplier)
            {
                var eff = Player.m_localPlayer.GetEffectPower<Tick_Soul_Power.Config>(
                    "Tick Soul Power");
                if (eff.Value > 0)
                {
                    eitrMultiplier += (eff.Value / 100f);
                }
            }
        }*/
    }
}