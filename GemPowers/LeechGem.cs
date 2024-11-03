namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Leech_Soul_Power
    {

        public struct Config
        { 
            [InverseMultiplicativePercentagePower] public float Value;
        } 
        
        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        private static class VampirismPatch
        {
            private static void Prefix(Character __instance, HitData hit)
            {
                if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return; 
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Leech Soul Power");
                if (Effect.Value > 0)
                {
                    float val = Effect.Value / 100f;
                    float dmg = SimulateDamage(hit, __instance);
                    Player.m_localPlayer.AddStamina(val * dmg);
                }
            }
        } 
    }
}