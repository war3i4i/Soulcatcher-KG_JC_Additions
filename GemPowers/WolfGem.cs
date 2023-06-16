namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public class SE_WolfAura : StatusEffect
    {
        public SE_WolfAura()
        {
            m_tooltip = "Alpha Wolf Aura";
            m_icon = NullSprite;
            m_name = "Alpha Wolf Aura";
            m_ttl = 4;
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Wolf_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }
    }


    public static class Wolf_Soul_Power
    {
        public static class WolfAuraDBPatches
        {
            private static void AddBuff(ObjectDB odb)
            {
                if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                    ObjectDB.instance.GetItemPrefab("Amber") == null) return;

                if (!odb.m_StatusEffects.Find(se => se.name == "AlphaWolfAura"))
                {
                    SE_WolfAura se = ScriptableObject.CreateInstance<SE_WolfAura>();
                    se.name = "AlphaWolfAura";
                    odb.m_StatusEffects.Add(se);
                }
            }

            [HarmonyPatch(typeof(ObjectDB), "Awake")]
            public static class ObjectDBAwake
            {
                public static void Postfix(ObjectDB __instance)
                {
                    AddBuff(__instance);
                }
            }

            [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
            public static class ObjectDBCopyOtherDB
            {
                public static void Postfix(ObjectDB __instance)
                {
                    AddBuff(__instance);
                }
            }
        }

        public static GameObject VFX;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class WolfGem_Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("WolfGem_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (hit.GetAttacker() is { } attacker && attacker.IsTamed())
                {
                    Vector3 pos = __instance.transform.position;
                    IEnumerable<Player> players = Player.GetAllPlayers()
                        .Where(p => Vector3.Distance(pos, p.transform.position) <= 15);
                    int max = 0;
                    foreach (Player player in players)
                    {
                        int val = (int)player.GetEffectPower<Config>("Wolf Soul Power").Value;
                        if (val > max)
                            max = val;
                    }

                    if (max == 0) return;
                    int tamedCreatures = Mathf.Max(1,
                        Character.GetAllCharacters().Count(p =>
                            Vector3.Distance(attacker.transform.position, p.transform.position) <= 15 && p.IsTamed()));
                    max /= tamedCreatures;
                    hit.ApplyModifier(max / 100f * 2f + 1);
                }
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        static class Character_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (__instance.IsTamed())
                {
                    Vector3 pos = __instance.transform.position;
                    IEnumerable<Player> players = Player.GetAllPlayers()
                        .Where(p => Vector3.Distance(pos, p.transform.position) <= 15);
                    int max = 0;
                    foreach (Player player in players)
                    {
                        int val = (int)player.GetEffectPower<Config>("Wolf Soul Power").Value;
                        if (val > max)
                            max = val;
                    }

                    if (max == 0) return;
                    int modifier = Mathf.Clamp(100 - max, 0, 100);
                    hit.ApplyModifier(modifier / 100f);
                }
            }
        }


        [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
        static class Player_SetLocalPlayer_Patch
        {
            static IEnumerator Corout()
            {
                for (;;)
                {
                    yield return new WaitForSeconds(3f);
                    Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Wolf Soul Power");
                    if (Effect.Value > 0)
                    {
                        IEnumerable<Character> list = Character.GetAllCharacters().Where(c =>
                            c.IsTamed() &&
                            Vector3.Distance(c.transform.position, Player.m_localPlayer.transform.position) <= 15f);
                        list.AddItem(Player.m_localPlayer);
                        foreach (Character character in list)
                        {
                            character.m_seman.AddStatusEffect("AlphaWolfAura".GetStableHashCode(), true);
                        }
                    }
                }
            }


            static void Postfix()
            {
                Player.m_localPlayer.StartCoroutine(Corout());
            }
        }
    }
}