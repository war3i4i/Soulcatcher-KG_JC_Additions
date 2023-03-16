namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Deer_Soul_Power
    {
        public struct Config
        {
            [MultiplicativePercentagePower] public float Value;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetJogSpeedFactor))]
        private class IncreaseJogSpeed
        {
            private static void Postfix(Player __instance, ref float __result)
            {
                float value = __instance.GetEffectPower<Config>("Deer Soul Power").Value;
                if (value <= 0) return;
                float healthPercentage = 1 - __instance.GetHealth() / __instance.GetMaxHealth();
                healthPercentage = Mathf.Clamp01(healthPercentage);
                __result *= 1 + value / 90f * healthPercentage;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetRunSpeedFactor))]
        private class IncreaseRunSpeed
        {
            private static void Postfix(Player __instance, ref float __result)
            {
                float value = __instance.GetEffectPower<Config>("Deer Soul Power").Value;
                if (value <= 0) return;
                float healthPercentage = 1 - __instance.GetHealth() / __instance.GetMaxHealth();
                healthPercentage = Mathf.Clamp01(healthPercentage);
                __result *= 1 + value / 90f * healthPercentage;
            }
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyRunStaminaDrain))]
        static class SEMan_ModifyRunStaminaDrain_Patch
        {
            static void Prefix(SEMan __instance, ref float drain)
            {
                if (__instance.m_character != Player.m_localPlayer) return;
                Player p = Player.m_localPlayer;
                float value = p.GetEffectPower<Config>("Deer Soul Power").Value;
                if (value <= 0) return;
                float healthPercentage = p.GetHealth() / p.GetMaxHealth() / 2 + 0.6f;
                healthPercentage = Mathf.Clamp01(healthPercentage);
                drain *= healthPercentage;
            }
        }
    }
}