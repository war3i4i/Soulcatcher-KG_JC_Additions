namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Serpent_Soul_Power
    {
        private static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Serpent_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
            }
        }

        public struct Config
        {
            [MaxPower] public float Value;
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.BlockAttack))]
        static class Humanoid_BlockAttack_Patch
        {
            private static void SpartanBlock(Character main, Character attacker)
            {
                if (main != Player.m_localPlayer) return;
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Serpent Soul Power");
                if (Effect.Value <= 0) return;
                Vector3 pos = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward +
                              Vector3.up * 1.5f;
                Instantiate(VFX, pos, Player.m_localPlayer.transform.rotation);


                float damage = Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalBlockableDamage() *
                    Effect.Value / 100f;
                HitData hit = new();
                hit.m_attacker = Player.m_localPlayer.GetZDOID();
                hit.m_point = attacker.m_collider.ClosestPointOnBounds(pos);
                hit.m_ranged = false;
                hit.m_damage.m_damage = damage;
                attacker.Damage(hit);
            }

            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> BlockTranspiler(IEnumerable<CodeInstruction> code)
            {
                FieldInfo field = AccessTools.DeclaredField(typeof(Humanoid), "m_perfectBlockEffect");
                MethodInfo method = AccessTools.DeclaredMethod(typeof(Humanoid_BlockAttack_Patch), nameof(SpartanBlock),
                    new[] { typeof(Character), typeof(Character) });
                List<CodeInstruction> list = new(code);
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].opcode == OpCodes.Ldfld && (FieldInfo)list[i].operand == field)
                    {
                        list.InsertRange(i - 1, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldarg_2),
                            new CodeInstruction(OpCodes.Call, method)
                        });
                        break;
                    }
                }

                return list;
            }
        }
    }
}