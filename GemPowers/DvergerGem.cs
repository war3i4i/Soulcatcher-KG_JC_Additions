namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    public static class Dverger_Soul_Power
    {
        public struct Config
        {
            [AdditivePower] public float Value;
        }

        /*[HarmonyPatch(typeof(ItemDrop.ItemData),nameof(ItemDrop.ItemData.GetWeaponLoadingTime))]
        static class ItemDropItemData_GetWeaponLoadingTime_Patch 
        {

            static void Postfix(ItemDrop.ItemData __instance, ref float __result)
            {
                var eff = Player.m_localPlayer.GetEffectPower<Dverger_Soul_Power.Config>("Dverger Soul Power");
                if(eff.Value > 0) 
                {
                    eff.Value = Mathf.Min(1, eff.Value / 100f);
                    __result = __result * (1 - eff.Value);
                }
            }
        }*/
        
    }
}
