namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Neck_Soul_Power
    {
        public struct Config
        {
            [MultiplicativePercentagePower] public float Value;
        }

         [HarmonyPatch(typeof(Character),nameof(Character.UpdateSwimming))]
         static class Character_UpdateSwiming_Patch
         {
             static void IncreaseSwimSpeed(Character c,ref float speed)
             {
                 if(c != Player.m_localPlayer) return;
                 float Effect = Player.m_localPlayer.GetEffectPower<Config>("Neck Soul Power").Value;
                 if (Effect > 0)
                 {
                     speed *= 1 + Effect / 10f;
                 }
             } 
              
             
             [HarmonyTranspiler] 
             static IEnumerable<CodeInstruction> Code(IEnumerable<CodeInstruction> instructions)
             {
                 bool isdone = false;
                 foreach (CodeInstruction codeInstruction in instructions)
                 {
                     yield return codeInstruction;
                     if (codeInstruction.opcode == OpCodes.Stloc_1 && !isdone)
                     {
                         isdone = true;
                         MethodInfo method = AccessTools.Method(typeof(Character_UpdateSwiming_Patch), nameof(IncreaseSwimSpeed), new[] {typeof(Character), typeof(float).MakeByRefType() });
                         yield return new CodeInstruction(OpCodes.Ldarg_0);
                         yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                         yield return new CodeInstruction(OpCodes.Call, method);
                     }
                 }
             }
         }
    }
}
