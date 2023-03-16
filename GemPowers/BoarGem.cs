namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Boar_Soul_Power
    {
        public struct Config
        {
            [MultiplicativePercentagePower] public float Value;
        }

        [HarmonyPatch(typeof(SEMan),nameof(SEMan.ModifyStaminaRegen))]
        static class SEMan_ModifyStaminaRegen_Patch
        {
            static void Prefix(ref float staminaMultiplier)
            {
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Boar Soul Power");
                if (Effect.Value > 0)
                {
                    staminaMultiplier *= 1 + Effect.Value / 100f;
                }
            }
        }
    }
}
