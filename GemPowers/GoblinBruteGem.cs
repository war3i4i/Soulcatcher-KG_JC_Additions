namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class GoblinBrute_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.RPC_UseStamina))]
        static class Player_UseStamina_Patch
        {
            static void Prefix(Player __instance, ref float v)
            {
                float Effect = __instance.GetEffectPower<Config>("GoblinBrute Soul Power").Value;
                if (Effect > 0)
                {
                    v *= Mathf.Clamp01(1 - Effect / 100f);
                }
            }
        }
    }
}