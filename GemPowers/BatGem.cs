namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static float SimulateDamage(HitData other, Character c)
    {
        HitData hit = other.Clone();
        if (c.m_baseAI != null && !c.m_baseAI.IsAlerted() && hit.m_backstabBonus > 1f &&
            Time.time - c.m_backstabTime > 300f) hit.ApplyModifier(hit.m_backstabBonus);
        if (c.IsStaggering() && !c.IsPlayer()) hit.ApplyModifier(2f);
        HitData.DamageModifiers damageModifiers = c.GetDamageModifiers();
        hit.ApplyResistance(damageModifiers, out _);
        return hit.GetTotalDamage();
    }


    
    public static class Bat_Soul_Power
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
                if (hit.GetAttacker() != Player.m_localPlayer) return;
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Bat Soul Power");
                if (Effect.Value > 0)
                {
                    float val = Effect.Value / 100f;
                    float dmg = SimulateDamage(hit, __instance);
                    Player.m_localPlayer.Heal(val * dmg);
                }
            }
        } 
    }
}