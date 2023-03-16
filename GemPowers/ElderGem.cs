namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public class SE_SoulcatcherElder : StatusEffect
    {
        public float Slow;

        public SE_SoulcatcherElder()
        {
            m_tooltip = "Soulcatcher Elder";
            m_icon = NullSprite;
            m_name = "Soulcatcher Elder";
            m_ttl = 4;
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Elder_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }

        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            speed = 0f;
        }
    }


    public static class Elder_Soul_Power
    {
        public static GameObject VFX;
        public static GameObject VFX2;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Elder_VFX");
                VFX2 = asset.LoadAsset<GameObject>("Elder_VFX2");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                __instance.m_prefabs.Add(VFX2);
                __instance.m_namedPrefabs.Add(VFX2.name.GetStableHashCode(), VFX2);
            }
        }


        public struct Config
        {
            [MinPower] public float Value;
        }


        [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
        static class Player_SetLocalPlayer_Patch
        {
            private static int CD;


            static IEnumerator Corout()
            {
                for (;;)
                {
                    yield return new WaitForSeconds(3f);
                    CD += 3;
                    float Effect = Player.m_localPlayer.GetEffectPower<Config>("Elder Soul Power").Value;
                    if (Effect > 0 && CD >= Effect)
                    {
                        CD = 0;
                        IEnumerable<Character> list = Character.GetAllCharacters().Where(c =>
                            EnemyCondition(c) &&
                            Vector3.Distance(c.transform.position, Player.m_localPlayer.transform.position) <= 13f);
                        if (list.Any())
                        {
                            Instantiate(VFX2, Player.m_localPlayer.transform.position,
                                Quaternion.identity);
                            foreach (Character character in list)
                            {
                                character.m_seman.AddStatusEffect("SoulcatcherElder");
                            }
                        }
                    }
                }
            }


            static void Postfix()
            {
                Player.m_localPlayer.StartCoroutine(Corout());
            }
        }


        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherElder"))
            {
                SE_SoulcatcherElder se = ScriptableObject.CreateInstance<SE_SoulcatcherElder>();
                se.name = "SoulcatcherElder";
                odb.m_StatusEffects.Add(se);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDBAwake
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDBCopyOtherDB
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }
    }
}