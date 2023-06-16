namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static void InitSoulcatcherSkill()
    {
        Skill SoulcatcherSkill = new Skill("Soulcatcher", "icon.png");
        SoulcatcherSkill.Name.English("Soulcatcher");
        SoulcatcherSkill.Description.English("Catch souls faster");
    }
}