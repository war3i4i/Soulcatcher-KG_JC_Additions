namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Eikthyr_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Character), "Jump")]
        public static class AdditionalJump
        { 
            private static int JumpCount;
            
            private static void Prefix(Character __instance)
            {
                if (Player.m_localPlayer && __instance == Player.m_localPlayer)
                {
                    if (JumpCount > 0 && __instance.IsOnGround())
                    {
                        JumpCount = 0;
                    }

                    Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Eikthyr Soul Power");
                    
                    if (Effect.Value > 0  && JumpCount < Effect.Value + 1)
                    {
                        __instance.m_lastGroundTouch = 0f;
                        __instance.m_maxAirAltitude = __instance.transform.position.y;
                        JumpCount++;
                    }
                }
            }
        }

    }
}