namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Seeker_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        [HarmonyPatch(typeof(Character), nameof(Character.AddStaggerDamage))]
        static class Character_AddStaggerDamage_Patch
        {
            static void Prefix(Character __instance, ref float damage)
            {
                if (__instance != Player.m_localPlayer) return;
                var eff = Player.m_localPlayer.GetEffectPower<Seeker_Soul_Power.Config>(
                    "Seeker Soul Power");
                if (eff.Value > 0)
                {
                    damage *= Mathf.Clamp01(1 - eff.Value / 100f);
                }
            }
        }
    }
}