namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    
    public class SE_SoulcatcherModer : StatusEffect
    {
        public float Slow;
        
        public SE_SoulcatcherModer()
        {
            m_tooltip = "Soulcatcher Moder";
            m_icon = NullSprite; 
            m_name = "Soulcatcher Moder";
            m_ttl = 4;
            m_startEffects = new EffectList
            { 
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Moder_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }

        public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
        {
            speed *= Slow;
        }
    }


    
    public static class Moder_Soul_Power
    {
        public static GameObject VFX; 

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources 
        {
            static void Postfix(ZNetScene __instance) 
            {
                VFX = asset.LoadAsset<GameObject>("Moder_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX); 
            }
        }
        
        
        public struct Config
        { 
            [MaxPower] public float Value;
        } 
        
        
        
        
        [HarmonyPatch(typeof(Character),nameof(Character.Awake))]
        static class Character_Awake_Patch 
        { 
            static void Postfix(Character __instance) 
            {
                __instance.m_nview.Register("Soulcatcher ModerEffect", new Action<long, float>((_, Value) =>
                {
                    StatusEffect SE_Effect = __instance.m_seman.GetStatusEffect("SoulcatcherModer".GetStableHashCode());
                    if (SE_Effect)
                    {
                        ((SE_SoulcatcherModer)SE_Effect).Slow = Value;
                        SE_Effect.ResetTime();
                    }
                    else
                    {
                        SE_Effect = __instance.m_seman.AddStatusEffect("SoulcatcherModer".GetStableHashCode(), true);
                        if (SE_Effect) 
                        {
                            ((SE_SoulcatcherModer)SE_Effect).Slow = Value;
                        }
                    }
                }));
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
                    Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Moder Soul Power");
                    if (Effect.Value > 0)
                    {
                        float slowValue = Mathf.Clamp01(1 - Effect.Value * 0.01f);
                        IEnumerable<Character> list = Character.GetAllCharacters().Where(c => EnemyCondition(c) && Vector3.Distance(c.transform.position ,Player.m_localPlayer.transform.position) <= 15f);
                        foreach (Character character in list)
                        {
                            character.m_nview.InvokeRPC( ZNetView.Everybody,"Soulcatcher ModerEffect", slowValue);
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

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherModer"))
            {
                SE_SoulcatcherModer se = ScriptableObject.CreateInstance<SE_SoulcatcherModer>();
                se.name = "SoulcatcherModer";
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