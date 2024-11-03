namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Draugr_Soul_Power
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
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Draugr Soul Power");
                if (Effect.Value > 0)
                {
                    float modify = Mathf.Clamp01(1 - Effect.Value / 300f);
                    hit.ApplyModifier(modify);
                }
            }
        }
          
        [HarmonyPatch(typeof(ItemDrop.ItemData),nameof(ItemDrop.ItemData.GetBlockPower), typeof(float))]
        static class ItemData_GetBlockPower_Patch
        {  
           
            static void Postfix(ref float __result)
            {
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Draugr Soul Power");
                if (Effect.Value > 0) 
                {
                    __result += __result * (Effect.Value / 100f);
                }
            }
        }

    }
}