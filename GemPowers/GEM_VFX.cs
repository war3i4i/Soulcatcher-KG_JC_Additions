using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
   private static class GEM_VFX
   {
      private static readonly GameObject Yagluth_VFX;
      private static readonly GameObject Bonemass_VFX;
      private static readonly GameObject Eikthyr_VFX;
      private static readonly GameObject Moder_VFX;
      
      static GEM_VFX()
      {
         API.OnEffectRecalc += () =>
         {
            Player p = Player.m_localPlayer;
            if (p.GetEffectPower<Yagluth_Soul_Power.Config>("Yagluth Soul Power").Value > 0)
            {
               p.m_seman.AddStatusEffect("Soulcatcher_Yagluth_VFX".GetStableHashCode());
            }
            else
            {
               p.m_seman.RemoveStatusEffect("Soulcatcher_Yagluth_VFX".GetStableHashCode());
            }
            
            if (p.GetEffectPower<Bonemass_Soul_Power.Config>("Bonemass Soul Power").Value > 0)
            {
               p.m_seman.AddStatusEffect("Soulcatcher_Bonemass_VFX".GetStableHashCode());
            }
            else
            {
               p.m_seman.RemoveStatusEffect("Soulcatcher_Bonemass_VFX".GetStableHashCode());
            }
            
            if (p.GetEffectPower<Moder_Soul_Power.Config>("Moder Soul Power").Value > 0)
            {
               p.m_seman.AddStatusEffect("Soulcatcher_Moder_VFX".GetStableHashCode());
            }
            else
            {
               p.m_seman.RemoveStatusEffect("Soulcatcher_Moder_VFX".GetStableHashCode());
            }
         };
         
         Yagluth_VFX = asset.LoadAsset<GameObject>("Yagluth_VFX");
         Bonemass_VFX = asset.LoadAsset<GameObject>("Bonemass_VFX");
         Eikthyr_VFX = asset.LoadAsset<GameObject>("Eikthyr_VFX");

         var tempFix = Eikthyr_VFX.AddComponent<TimedDestruction>();
         tempFix.m_triggerOnAwake = true;
         tempFix.m_timeout = 3f;
         
         Moder_VFX = asset.LoadAsset<GameObject>("Moder_Main_VFX");
         Special_VFX_ToObjectDB.Add(Yagluth_VFX);
         Special_VFX_ToObjectDB.Add(Bonemass_VFX);
         Special_VFX_ToObjectDB.Add(Eikthyr_VFX);
         Special_VFX_ToObjectDB.Add(Moder_VFX);
      }
      
      
      [HarmonyPatch(typeof(FootStep),nameof(FootStep.OnFoot), typeof(Transform))]
      static class FootStep_OnFoot_Patch
      {
         static void Postfix(FootStep __instance, Transform foot)
          {
            if(__instance.m_character != Player.m_localPlayer) return;
            if(Player.m_localPlayer.GetEffectPower<Eikthyr_Soul_Power.Config>("Eikthyr Soul Power").Value > 0)
            {
               Instantiate(Eikthyr_VFX, foot.position, Quaternion.identity);
            }
          }
      }
      
      
      private static void AddSE(ObjectDB odb)
      {
         if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
             ObjectDB.instance.GetItemPrefab("Amber") == null) return;
         
         SE_GenericInstantiate yagluth_vfx = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
         yagluth_vfx.m_ttl = 0;
         yagluth_vfx.name = "Soulcatcher_Yagluth_VFX";
         yagluth_vfx.m_icon = null;
         yagluth_vfx.m_startEffects = new EffectList
         { 
            m_effectPrefabs = new[]
            {
               new EffectList.EffectData()
               {
                  m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                  m_inheritParentScale = true,
                  m_prefab = Yagluth_VFX, m_randomRotation = false, m_scale = true, 
               }
            }
         };
         
         if (!odb.m_StatusEffects.Find(se => se.name == "Soulcatcher_Yagluth_VFX"))
            odb.m_StatusEffects.Add(yagluth_vfx);
         
         SE_GenericInstantiate bonemass_vfx = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
         bonemass_vfx.m_ttl = 0;
         bonemass_vfx.name = "Soulcatcher_Bonemass_VFX";
         bonemass_vfx.m_icon = null;
         bonemass_vfx.m_startEffects = new EffectList
         { 
            m_effectPrefabs = new[]
            {
               new EffectList.EffectData()
               {
                  m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                  m_inheritParentScale = true,
                  m_prefab = Bonemass_VFX, m_randomRotation = false, m_scale = true
               }
            }
         };
         
         if (!odb.m_StatusEffects.Find(se => se.name == "Soulcatcher_Bonemass_VFX"))
            odb.m_StatusEffects.Add(bonemass_vfx);
         
         SE_GenericInstantiate moder_vfx = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
         moder_vfx.m_ttl = 0;
         moder_vfx.name = "Soulcatcher_Moder_VFX";
         moder_vfx.m_icon = null;
         moder_vfx.m_startEffects = new EffectList
         { 
            m_effectPrefabs = new[]
            {
               new EffectList.EffectData()
               {
                  m_attach = true, m_enabled = true, m_inheritParentRotation = true,
                  m_inheritParentScale = true,
                  m_prefab = Moder_VFX, m_randomRotation = false, m_scale = true
               }
            }
         };
         
         if (!odb.m_StatusEffects.Find(se => se.name == "Soulcatcher_Moder_VFX"))
            odb.m_StatusEffects.Add(moder_vfx);


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