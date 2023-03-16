namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    
    public class SE_SoulcatcherFenring : StatusEffect
    {
        public int Multiplier;
        
        public SE_SoulcatcherFenring()
        {
            m_tooltip = "Soulcatcher Fenring";
            m_icon = NullSprite; 
            m_name = "Soulcatcher Fenring"; 
            m_ttl = 6; 
            m_startEffects = new EffectList
            { 
                m_effectPrefabs = new[]
                {
                    new EffectList.EffectData()
                    {
                        m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                        m_inheritParentScale = true,
                        m_prefab = Fenring_Soul_Power.VFX, m_randomRotation = false, m_scale = true
                    }
                }
            };
        }

        public override void OnDamaged(HitData hit, Character attacker)
        {
            float multiply = 1 + Multiplier * 0.01f;
            hit.ApplyModifier(multiply);
            
        }
    }
 

    
    public static class Fenring_Soul_Power
    {
        public static GameObject VFX; 

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources 
        {
            static void Postfix(ZNetScene __instance) 
            {
                VFX = asset.LoadAsset<GameObject>("Fenring_VFX");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX); 
            } 
        }
        
        
        public struct Config
        { 
            [AdditivePower] public float Value;
        } 
        
        
        [HarmonyPatch(typeof(Character),nameof(Character.Awake))]
        static class Character_Awake_Patch 
        {  
            static void Postfix(Character __instance) 
            { 
                __instance.m_nview.Register("Soulcatcher FentingEffect", new Action<long, int>((_, Value) =>
                {
                    StatusEffect SE_Effect = __instance.m_seman.GetStatusEffect("SoulcatcherFenring");
                    if (SE_Effect)
                    {
                        ((SE_SoulcatcherFenring)SE_Effect).Multiplier += Value;
                    }
                    else
                    {
                        SE_Effect = __instance.m_seman.AddStatusEffect("SoulcatcherFenring");
                        if (SE_Effect) 
                        {
                            ((SE_SoulcatcherFenring)SE_Effect).Multiplier = Value;
                        }
                    }
                }));
            }
        }
        
        [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if(hit.GetAttacker() != Player.m_localPlayer) return;
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Fenring Soul Power");
                if (Effect.Value > 0)
                {
                    __instance.m_nview.InvokeRPC( "Soulcatcher FentingEffect", (int)Effect.Value);
                }
            }
        }
        
        
        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;

            if (!odb.m_StatusEffects.Find(se => se.name == "SoulcatcherFenring"))
            {
                SE_SoulcatcherFenring se = ScriptableObject.CreateInstance<SE_SoulcatcherFenring>();
                se.name = "SoulcatcherFenring";
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
