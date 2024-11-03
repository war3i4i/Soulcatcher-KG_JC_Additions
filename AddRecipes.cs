namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static Dictionary<string, CraftingStation> stations;
    private static CraftingStation JC_Station;

    private static void InitCraftingStations()
    {
        if (stations == null) 
        {
            JC_Station = API.GetGemcuttersTable().GetComponent<CraftingStation>();
            stations = new Dictionary<string, CraftingStation>();
            foreach (Recipe recipe in ObjectDB.instance.m_recipes)
            {
                if (recipe.m_craftingStation != null &&
                    !stations.ContainsKey(recipe.m_craftingStation.name))
                    stations.Add(recipe.m_craftingStation.name, recipe.m_craftingStation);
            } 
        }
    }


    private static Recipe CreateRecipe(string name, int amount, int minstation, ItemDrop item,
        Piece.Requirement[] reqs)
    {
        Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
        recipe.name = name;
        recipe.m_amount = amount;
        recipe.m_minStationLevel = minstation;
        recipe.m_item = item;
        recipe.m_enabled = true;
        recipe.m_resources = reqs;
        recipe.m_craftingStation = JC_Station;
        recipe.m_repairStation = JC_Station;
        return recipe;
    }


    private static void GemRecipes()
    {
        InitCraftingStations();
        foreach (Gem gem in MainGems)
        {
            GameObject prefab = gem.Prefab;
            GameObject ascendPrefab = ObjectDB.instance.GetItemPrefab($"{prefab.name}_Ascend");
            if (!ascendPrefab) continue;
            List<Piece.Requirement> reqs_Ascend = new List<Piece.Requirement>();
            Piece.Requirement req_Ascend = new Piece.Requirement
            {
                m_resItem = prefab.GetComponent<ItemDrop>(),
                m_amount = 3,
                m_amountPerLevel = 0,
                m_recover = false
            };
            reqs_Ascend.Add(req_Ascend);

            Recipe recipe_Ascend =
                CreateRecipe($"Recipe_{prefab.name}_Ascend", 1, 1, ascendPrefab.GetComponent<ItemDrop>(),
                    reqs_Ascend.ToArray());

            if (!ObjectDB.instance.m_recipes.Find(r => r.name == recipe_Ascend.name))
            {
                ObjectDB.instance.m_recipes.Add(recipe_Ascend);
            }

            GameObject immortalPrefab = ObjectDB.instance.GetItemPrefab($"{prefab.name}_Immortal");
            if (!immortalPrefab) continue;
            List<Piece.Requirement> reqs_Immortal = new List<Piece.Requirement>();
            Piece.Requirement req_Immortal = new Piece.Requirement
            {
                m_resItem = ascendPrefab.GetComponent<ItemDrop>(),
                m_amount = 3,
                m_amountPerLevel = 0,
                m_recover = false
            };
            reqs_Immortal.Add(req_Immortal);
            Recipe recipe_Immortal =
                CreateRecipe($"Recipe_{prefab.name}_Immortal", 1, 1, immortalPrefab.GetComponent<ItemDrop>(),
                    reqs_Immortal.ToArray());

            if (!ObjectDB.instance.m_recipes.Find(r => r.name == recipe_Immortal.name))
            {
                ObjectDB.instance.m_recipes.Add(recipe_Immortal);
            }


            GameObject godlikePrefab = ObjectDB.instance.GetItemPrefab($"{prefab.name}_Godlike");
            if(!godlikePrefab) continue;
            List<Piece.Requirement> reqs_Godlike = new List<Piece.Requirement>();
            Piece.Requirement req_Godlike = new Piece.Requirement
            {
                m_resItem = immortalPrefab.GetComponent<ItemDrop>(),
                m_amount = 3,
                m_amountPerLevel = 0,
                m_recover = false 
            };
            reqs_Godlike.Add(req_Godlike);
            Recipe recipeGodlike =
                CreateRecipe($"Recipe_{prefab.name}_Godlike", 1, 1, godlikePrefab.GetComponent<ItemDrop>(),
                    reqs_Godlike.ToArray());

            if (!ObjectDB.instance.m_recipes.Find(r => r.name == recipeGodlike.name))
            {
                ObjectDB.instance.m_recipes.Add(recipeGodlike);
            }
            
            
            GameObject odinwrathPrefab = ObjectDB.instance.GetItemPrefab($"{prefab.name}_Odinwrath");
            if(!odinwrathPrefab) continue;
            List<Piece.Requirement> reqs_Odinwrath = new List<Piece.Requirement>();
            Piece.Requirement req_Odinwrath = new Piece.Requirement
            {
                m_resItem = godlikePrefab.GetComponent<ItemDrop>(),
                m_amount = 3,
                m_amountPerLevel = 0,
                m_recover = false 
            };
            reqs_Odinwrath.Add(req_Odinwrath);
            Recipe recipeOdinwrath =
                CreateRecipe($"Recipe_{prefab.name}_Odinwrath", 1, 1, odinwrathPrefab.GetComponent<ItemDrop>(),
                    reqs_Odinwrath.ToArray());

            if (!ObjectDB.instance.m_recipes.Find(r => r.name == recipeOdinwrath.name))
            {
                ObjectDB.instance.m_recipes.Add(recipeOdinwrath);
            }
            
            
        }


        ObjectDB.instance.UpdateRegisters();
    }
}