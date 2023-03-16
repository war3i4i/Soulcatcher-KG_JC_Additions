namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Gjall_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        [HarmonyPatch(typeof(Player),nameof(Player.GetTotalFoodValue))]
        static class Player_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.Last)]
            static void Postfix(ref float hp, ref float stamina, ref float eitr)
            {
                var eff = Player.m_localPlayer.GetEffectPower<Gjall_Soul_Power.Config>(
                    "Gjall Soul Power");
                if (eff.Value > 0)
                {
                    hp *= 1 + eff.Value / 100f;
                    stamina *= 1 + eff.Value / 100f;
                    eitr *= 1 + eff.Value / 100f;
                }
            }
        }
    }
}