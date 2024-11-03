namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static void InitJewelry()
    {
        Item necklace = new(API.CreateNecklaceFromTemplate("SoulNecklace",new Color32(8, 236, 247,255)));
        necklace.Name.English("Soul Necklace");
        necklace.Description.English("Increases soul spawn chance");
        necklace.Crafting.Add(API.GetGemcuttersTable().name, 3);
        necklace.MaximumRequiredStationLevel = 3;
        necklace.RequiredItems.Add("Coins", 1000);
        necklace.RequiredUpgradeItems.Add("Coins", 1500); 
        ItemDrop.ItemData.SharedData necklaceShared = necklace.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared;
        SE_JewelryEffect se_necklace = ScriptableObject.CreateInstance<SE_JewelryEffect>();
        se_necklace.name = "SoulNecklace";
        se_necklace.m_name = "Soul Necklace"; 
        se_necklace.m_icon = necklaceShared.m_icons[0];
        se_necklace.m_tooltip = "Increases soul spawn chance";
        necklaceShared.m_equipStatusEffect = se_necklace;
        necklaceShared.m_movementModifier = 0.05f;
        necklaceShared.m_armor = 10f;
        necklaceShared.m_armorPerLevel = 5f;

        Item ring = new(API.CreateRingFromTemplate("SoulRing",new Color32(8, 236, 247,255)));
        ring.Name.English("Soul Ring");
        ring.Description.English( "Increase soul capture distance");
        ring.Crafting.Add(API.GetGemcuttersTable().name, 3);
        ring.MaximumRequiredStationLevel = 3;
        ring.RequiredItems.Add("Coins", 1000);
        ring.RequiredUpgradeItems.Add("Coins", 1500);
        ItemDrop.ItemData.SharedData ringShared = ring.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared;
        SE_JewelryEffect se_ring = ScriptableObject.CreateInstance<SE_JewelryEffect>();
        se_ring.name = "SoulRing";
        se_ring.m_name = "Soul Ring";
        se_ring.m_icon = ringShared.m_icons[0];
        se_ring.m_tooltip = "Increase souls capture distance"; 
        ringShared.m_equipStatusEffect = se_ring;  
        ringShared.m_movementModifier = 0.05f;
        ringShared.m_armor = 10f; 
        ringShared.m_armorPerLevel = 5f; 
        
    }
    
    public class SE_JewelryEffect : StatusEffect
    {
        
    }
    
    
    
    
}