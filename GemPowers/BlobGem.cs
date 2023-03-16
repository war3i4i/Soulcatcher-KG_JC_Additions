namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Blob_Soul_Power
    {
        public struct Config
        {
            [MultiplicativePercentagePower] public float Value;
        }

        private static GameObject VFX;
        
        [HarmonyPatch(typeof(ZNetScene),nameof(ZNetScene.Awake))]
        static class ZNetScene_Awake_Patch
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("BlobGem_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
            }
        }
        
        [HarmonyPatch(typeof(Character),nameof(Character.UpdateGroundContact))]
        static class Character_UpdateGroundContact_Patch
        {
            private static void GetDamage(ref HitData hit)
            {
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Blob Soul Power");
                if (Effect.Value > 0)
                {
                    hit.m_damage.m_damage = 0;
                    hit.ApplyModifier(0);
                }
            }
            
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Code(IEnumerable<CodeInstruction> code)
            {
                FieldInfo field = AccessTools.Field(typeof(HitData.DamageTypes), nameof(HitData.DamageTypes.m_damage));
                foreach (CodeInstruction instruction in code)
                {
                    yield return instruction; 
                    if (instruction.opcode == OpCodes.Stfld && instruction.operand == field)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloca_S, 3);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Character_UpdateGroundContact_Patch), nameof(GetDamage)));
                    }
                }
            }
        }
        

         [HarmonyPatch(typeof(Character),nameof(Character.Jump))]
         static class Character_JumpheightPatch
         {
             static void IncreaseJumpHeight(Character c, ref Vector3 vec)
             {
                 if(c != Player.m_localPlayer) return;
                 Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Blob Soul Power");
                 if (Effect.Value > 0)
                 {
                     Vector3 toAdd = Vector3.up * Effect.Value * 0.5f + c.transform.forward * Effect.Value * 6f;
                     Player.m_localPlayer.m_body.AddForce(toAdd , ForceMode.VelocityChange);
                     Instantiate(VFX, Player.m_localPlayer.transform.position, Quaternion.identity);
                 }
             } 
              
             
             [HarmonyTranspiler] 
             static IEnumerable<CodeInstruction> Code(IEnumerable<CodeInstruction> instructions)
             {
                 bool isdone = false;
                 MethodInfo callMethod = AccessTools.Method(typeof(Character_JumpheightPatch), nameof(IncreaseJumpHeight));
                 MethodInfo method = AccessTools.Method(typeof(Rigidbody), nameof(Rigidbody.WakeUp));
                 foreach (CodeInstruction codeInstruction in instructions)
                 {
                     yield return codeInstruction;
                     if (codeInstruction.opcode == OpCodes.Callvirt && codeInstruction.operand == method && !isdone)
                     {
                         isdone = true;
                         yield return new CodeInstruction(OpCodes.Ldarg_0);
                         yield return new CodeInstruction(OpCodes.Ldloca_S, 3);
                         yield return new CodeInstruction(OpCodes.Call, callMethod);
                     }
                 }
             }
         }
    }
}
