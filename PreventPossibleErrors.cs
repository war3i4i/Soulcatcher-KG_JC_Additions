namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    [HarmonyPatch(typeof(CharacterAnimEvent), nameof(CharacterAnimEvent.Awake))]
    private static class Fix
    {
        private static bool Prefix(CharacterAnimEvent __instance)
        {
            if (__instance.GetComponentInParent<Character>() == null)
            {
                Destroy(__instance);
                return false; 
            } 

            return true; 
        }
    }
    
    [HarmonyPatch(typeof(LevelEffects), nameof(LevelEffects.Start))]
    private static class Fix2
    {
        private static bool Prefix(LevelEffects __instance)
        {
            if (__instance.GetComponentInParent<Character>() == null)
            {
                Destroy(__instance);
                return false;
            }

            return true;
        }
    }

  
}