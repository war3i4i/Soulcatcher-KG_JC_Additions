namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Goblin_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }


        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return; 
                Player p = Player.m_localPlayer;
                Config Effect = p.GetEffectPower<Config>("Goblin Soul Power");
                if (Effect.Value > 0)
                {
                    Vector3 pVec = (__instance.transform.position - p.transform.position).normalized;
                    Vector3 eVec = __instance.transform.forward;
                    float playerVec = Quaternion.LookRotation(pVec).eulerAngles.y;
                    float enemyVec = Quaternion.LookRotation(eVec).eulerAngles.y;
                    float num = Mathf.Abs(Mathf.DeltaAngle(playerVec, enemyVec));
                    if (num <= 50f)
                    {
                        FloatingText(Color.magenta, "BACKSTAB");
                        hit.ApplyModifier(1 + Effect.Value * 0.01f);
                    }
                }
            }
        }
    }
}