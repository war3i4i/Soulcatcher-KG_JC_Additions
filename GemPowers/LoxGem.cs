namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Lox_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        private static class DamageNerf
        {
            private static void Prefix(ref HitData hit)
            {
                if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return; 
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Lox Soul Power");
                if (Effect.Value > 0)
                {
                    float modify = 1 + Effect.Value / 200f;
                    hit.ApplyModifier(modify);
                }
            } 
        }
          
        [HarmonyPatch(typeof(SEMan), nameof(SEMan.ApplyStatusEffectSpeedMods))]
        static class SEMan_ModifyRunStaminaDrain_Patch
        { 
            static void Prefix(SEMan __instance, ref float speed)
            {
                if(__instance.m_character != Player.m_localPlayer) return;
                Player p = Player.m_localPlayer; 
                float value = p.GetEffectPower<Config>("Lox Soul Power").Value;
                if(value <= 0) return;
                float modify = Mathf.Clamp01(1 - value / 100f);
                speed *= modify;
            }
        }

    }
}