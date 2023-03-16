// ReSharper disable ObjectCreationAsStatement

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static readonly List<Gem> MainGems = new();
    private static readonly List<Gem> GemsToObjectDB = new();
    private static readonly List<GameObject> Special_VFX_ToObjectDB = new();


    private static void InitGems()
    {
        StringBuilder sb = new();

        new Gem("DeerGem", "Deer", "Deer Soul Gem");
        API.AddGemEffect<Deer_Soul_Power.Config>("Deer Soul Power");
        sb.AppendLine();
        sb.AppendLine("Deer Soul Power:");
        sb.AppendLine("  slot: legs");
        sb.AppendLine("  gem: Deer Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [10, 15, 20, 25, 30]");
        sb.AppendLine("  unique: None");

        new Gem("NeckGem", "Neck", "Neck Soul Gem");
        API.AddGemEffect<Neck_Soul_Power.Config>("Neck Soul Power");
        sb.AppendLine();
        sb.AppendLine("Neck Soul Power:");
        sb.AppendLine("  slot: legs");
        sb.AppendLine("  gem: Neck Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: None");

        new Gem("BoarGem", "Boar", "Boar Soul Gem");
        API.AddGemEffect<Boar_Soul_Power.Config>("Boar Soul Power");
        sb.AppendLine();
        sb.AppendLine("Boar Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Boar Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [3, 5, 7, 9, 11]");
        sb.AppendLine("  unique: None");

        new Gem("GreydwarfGem", "Greydwarf", "Greydwarf Soul Gem");
        API.AddGemEffect<Greydwarf_Soul_Power.Config>("Greydwarf Soul Power");
        sb.AppendLine();
        sb.AppendLine("Greydwarf Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Greydwarf Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [3, 6, 9, 12, 15]");
        sb.AppendLine("    cooldown: [60, 50, 45, 40, 35]");
        sb.AppendLine("  unique: Item");
        
        new Gem("GreydwarfBruteGem", "Greydwarf_Elite", "GreydwarfBrute Soul Gem");
        API.AddGemEffect<GreydwarfBrute_Soul_Power.Config>("GreydwarfBrute Soul Power");
        sb.AppendLine();
        sb.AppendLine("GreydwarfBrute Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: GreydwarfBrute Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [0.4, 0.7, 1, 1.3, 1.6]");
        sb.AppendLine("  unique: Gem");
        
        new Gem("GreydwarfShamanGem", "Greydwarf_Shaman", "GreydwarfShaman Soul Gem");
        API.AddGemEffect<GreydwarfShaman_Soul_Power.Config>("GreydwarfShaman Soul Power");
        sb.AppendLine();
        sb.AppendLine("GreydwarfShaman Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: GreydwarfShaman Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [26, 25, 24, 23, 20]");
        sb.AppendLine("  unique: Gem");

        new Gem("TrollGem", "Troll", "Troll Soul Gem");
        API.AddGemEffect<Troll_Soul_Power.Config>("Troll Soul Power");
        sb.AppendLine();
        sb.AppendLine("Troll Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Troll Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("    chance: [30, 35, 40, 45, 50]");
        sb.AppendLine("  unique: None");


        new Gem("SkeletonGem", "Skeleton", "Skeleton Soul Gem");
        API.AddGemEffect<Skeleton_Soul_Power.Config>("Skeleton Soul Power");
        sb.AppendLine();
        sb.AppendLine("Skeleton Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Skeleton Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    chance: [7, 9, 11, 13, 15]");
        sb.AppendLine("    value: [3, 2, 2, 1, 0]");
        sb.AppendLine("  unique: Gem");

        new Gem("DraugrGem", "Draugr", "Draugr Soul Gem");
        API.AddGemEffect<Draugr_Soul_Power.Config>("Draugr Soul Power");
        sb.AppendLine();
        sb.AppendLine("Draugr Soul Power:");
        sb.AppendLine("  slot: shield");
        sb.AppendLine("  gem: Draugr Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [20, 30, 40, 50, 60]");
        sb.AppendLine("  unique: Gem");

        new Gem("BlobGem", "Blob", "Blob Soul Gem");
        API.AddGemEffect<Blob_Soul_Power.Config>("Blob Soul Power");
        sb.AppendLine();
        sb.AppendLine("Blob Soul Power:");
        sb.AppendLine("  slot: legs");
        sb.AppendLine("  gem: Blob Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [0.3, 0.8, 1.5, 2.2, 2.9]");
        sb.AppendLine("  unique: None");

        new Gem("LeechGem", "Leech", "Leech Soul Gem");
        API.AddGemEffect<Bat_Soul_Power.Config>("Leech Soul Power");
        sb.AppendLine();
        sb.AppendLine("Leech Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Leech Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [1, 1.5, 2, 2.5, 3]");
        sb.AppendLine("  unique: None");

        new Gem("WraithGem", "Wraith", "Wraith Soul Gem");
        API.AddGemEffect<Wraith_Soul_Power.Config>("Wraith Soul Power");
        sb.AppendLine();
        sb.AppendLine("Wraith Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Wraith Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    cooldown: [30, 26, 22, 18, 14]");
        sb.AppendLine("  unique: Gem");

        new Gem("AbominationGem", "Abomination", "Abomination Soul Gem");
        API.AddGemEffect<Abomination_Soul_Power.Config>("Abomination Soul Power");
        sb.AppendLine();
        sb.AppendLine("Abomination Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Abomination Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [30, 40, 50, 60, 70]");
        sb.AppendLine("    cooldown: [30, 28, 26, 24, 22]");
        sb.AppendLine("  unique: Gem");

        new Gem("WolfGem", "Wolf", "Wolf Soul Gem");
        API.AddGemEffect<Wolf_Soul_Power.Config>("Wolf Soul Power");
        sb.AppendLine();
        sb.AppendLine("Wolf Soul Power:");
        sb.AppendLine("  slot: [head, chest, legs, weapon]");
        sb.AppendLine("  gem: Wolf Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [9, 12, 15, 18, 21]");
        sb.AppendLine("  unique: Item");

        new Gem("BatGem", "Bat", "Bat Soul Gem");
        API.AddGemEffect<Bat_Soul_Power.Config>("Bat Soul Power");
        sb.AppendLine();
        sb.AppendLine("Bat Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Bat Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [1, 1.5, 2, 2.5, 3]");
        sb.AppendLine("  unique: None");

        new Gem("FenringGem", "Fenring", "Fenring Soul Gem");
        API.AddGemEffect<Fenring_Soul_Power.Config>("Fenring Soul Power");
        sb.AppendLine();
        sb.AppendLine("Fenring Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Fenring Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [1, 2, 3, 4, 5]");
        sb.AppendLine("  unique: None");

        new Gem("CultistGem", "Fenring_Cultist", "Cultist Soul Gem");
        API.AddGemEffect<Cultist_Soul_Power.Config>("Cultist Soul Power");
        sb.AppendLine();
        sb.AppendLine("Cultist Soul Power:");
        sb.AppendLine("  slot: head");
        sb.AppendLine("  gem: Cultist Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [20, 35, 50, 65, 80]");
        sb.AppendLine("    cooldown: [35, 30, 25, 20, 15]");
        sb.AppendLine("  unique: Gem");

        new Gem("UlvGem", "Ulv", "Ulv Soul Gem");
        API.AddGemEffect<Ulv_Soul_Power.Config>("Ulv Soul Power");
        sb.AppendLine();
        sb.AppendLine("Ulv Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Ulv Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    cooldown: [20, 18, 16, 14, 12]");
        sb.AppendLine("  unique: Gem");

        new Gem("StoneGolemGem", "StoneGolem", "StoneGolem Soul Gem");
        API.AddGemEffect<StoneGolem_Soul_Power.Config>("StoneGolem Soul Power");
        sb.AppendLine();
        sb.AppendLine("StoneGolem Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: StoneGolem Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Gem");

        new Gem("HatchlingGem", "Hatchling", "Hatchling Soul Gem");
        API.AddGemEffect<Hatchling_Soul_Power.Config>("Hatchling Soul Power");
        sb.AppendLine();
        sb.AppendLine("Hatchling Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Hatchling Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [150, 200, 250, 300, 350]");
        sb.AppendLine("  unique: Gem");

        new Gem("GoblinGem", "Goblin", "Goblin Soul Gem");
        API.AddGemEffect<Goblin_Soul_Power.Config>("Goblin Soul Power");
        sb.AppendLine();
        sb.AppendLine("Goblin Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Goblin Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Gem");

        new Gem("DeathsquitoGem", "Deathsquito", "Deathsquito Soul Gem");
        API.AddGemEffect<Deathsquito_Soul_Power.Config>("Deathsquito Soul Power");
        sb.AppendLine();
        sb.AppendLine("Deathsquito Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Deathsquito Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 7, 9, 11, 13]");
        sb.AppendLine("  unique: Gem");

        new Gem("LoxGem", "Lox", "Lox Soul Gem");
        API.AddGemEffect<Lox_Soul_Power.Config>("Lox Soul Power");
        sb.AppendLine();
        sb.AppendLine("Lox Soul Power:");
        sb.AppendLine("  slot: [head, chest, legs, weapon, cloak]");
        sb.AppendLine("  gem: Lox Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [10, 20, 30, 40, 50]");
        sb.AppendLine("  unique: Gem");

        new Gem("GoblinBruteGem", "GoblinBrute", "GoblinBrute Soul Gem");
        API.AddGemEffect<GoblinBrute_Soul_Power.Config>("GoblinBrute Soul Power");
        sb.AppendLine();
        sb.AppendLine("GoblinBrute Soul Power:");
        sb.AppendLine("  slot: [head, chest, legs, cloak]");
        sb.AppendLine("  gem: GoblinBrute Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [15, 20, 25, 30, 35]");
        sb.AppendLine("  unique: Gem");

        new Gem("GoblinShamanGem", "GoblinShaman", "GoblinShaman Soul Gem");
        API.AddGemEffect<GoblinShaman_Soul_Power.Config>("GoblinShaman Soul Power");
        sb.AppendLine();
        sb.AppendLine("GoblinShaman Soul Power:");
        sb.AppendLine("  slot: [head, chest, legs, cloak]");
        sb.AppendLine("  gem: GoblinShaman Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [10, 15, 20, 25, 30]");
        sb.AppendLine("  unique: Gem");

        new Gem("TarBlobGem", "BlobTar", "TarBlob Soul Gem");
        API.AddGemEffect<TarBlob_Soul_Power.Config>("TarBlob Soul Power");
        sb.AppendLine();
        sb.AppendLine("TarBlob Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: TarBlob Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    duration: [6, 7, 8, 9, 10]");
        sb.AppendLine("    cooldown: [32, 30, 28, 26, 24]");
        sb.AppendLine("  unique: Gem");

        new Gem("SurtlingGem", "Surtling", "Surtling Soul Gem");
        API.AddGemEffect<Surtling_Soul_Power.Config>("Surtling Soul Power");
        sb.AppendLine();
        sb.AppendLine("Surtling Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Surtling Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("    cooldown: [60, 50, 45, 40, 35]");
        sb.AppendLine("  unique: Gem");

        new Gem("SerpentGem", "Serpent", "Serpent Soul Gem");
        API.AddGemEffect<Serpent_Soul_Power.Config>("Serpent Soul Power");
        sb.AppendLine();
        sb.AppendLine("Serpent Soul Power:");
        sb.AppendLine("  slot: shield");
        sb.AppendLine("  gem: Serpent Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [25, 50, 75, 100, 125]");
        sb.AppendLine("  unique: Gem");
        
        new Gem("HareGem", "Hare", "Hare Soul Gem");
        API.AddGemEffect<Hare_Soul_Power.Config>("Hare Soul Power");
        sb.AppendLine();
        sb.AppendLine("Hare Soul Power:");
        sb.AppendLine("  slot: weapon");
        sb.AppendLine("  gem: Hare Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [4, 8, 12, 16, 20]");
        sb.AppendLine("  unique: None");
        
        new Gem("DvergerGem","Dverger","Dverger Soul Gem");
        API.AddGemEffect<Dverger_Soul_Power.Config>("Dverger Soul Power");
        sb.AppendLine();
        sb.AppendLine("Dverger Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Dverger Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Item");

        new Gem("DvergerGemFireMage", "DvergerMageFire", "Dverger Fire Mage Soul Gem", postfix: " (Fire)");
        API.AddGemEffect<Dverger_FireMage_Soul_Power.Config>("Dverger Fire Mage Soul Power");
        sb.AppendLine();
        sb.AppendLine("Dverger Fire Mage Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Dverger Fire Mage Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]"); 
        sb.AppendLine("  unique: Item");
        
        new Gem("DvergerGemBloodMage", "DvergerMageSupport", "Dverger Blood Mage Soul Gem", postfix: " (Blood)");
        API.AddGemEffect<Dverger_BloodMage_Soul_Power.Config>("Dverger Blood Mage Soul Power");
        sb.AppendLine();
        sb.AppendLine("Dverger Blood Mage Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Dverger Blood Mage Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [10, 15, 20, 25, 30]");
        sb.AppendLine("  unique: Item");
        
        new Gem("DvergerGemIceMage", "DvergerMageIce", "Dverger Ice Mage Soul Gem", postfix: " (Frost)");
        API.AddGemEffect<Dverger_IceMage_Soul_Power.Config>("Dverger Ice Mage Soul Power");
        sb.AppendLine();
        sb.AppendLine("Dverger Ice Mage Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Dverger Ice Mage Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Item");
        
        new Gem("TickGem", "Tick", "Tick Soul Gem");
        API.AddGemEffect<Tick_Soul_Power.Config>("Tick Soul Power");
        sb.AppendLine();
        sb.AppendLine("Tick Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Tick Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Item");
        
        new Gem("GjallGem", "Gjall", "Gjall Soul Gem");
        API.AddGemEffect<Gjall_Soul_Power.Config>("Gjall Soul Power");
        sb.AppendLine();
        sb.AppendLine("Gjall Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Gjall Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [3, 6, 9, 12, 15]");
        sb.AppendLine("  unique: Item");
        
        new Gem("SeekerGem","Seeker", "Seeker Soul Gem");
        API.AddGemEffect<Seeker_Soul_Power.Config>("Seeker Soul Power");
        sb.AppendLine();
        sb.AppendLine("Seeker Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Seeker Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [5, 10, 15, 20, 25]");
        sb.AppendLine("  unique: Item");
        
        new Gem("SeekerBruteGem","SeekerBrute", "Seeker Brute Soul Gem", Gem.GemTier.None);
        API.AddGemEffect<Seeker_Brute_Soul_Power.Config>("Seeker Brute Soul Power");
        sb.AppendLine();
        sb.AppendLine("Seeker Brute Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Seeker Brute Soul Gem"); 
        sb.AppendLine("  power:");
        sb.AppendLine("    value: 1");
        sb.AppendLine("  unique: Gem");

        new Gem("EikthyrGem", "Eikthyr", "Eikthyr Soul Gem"); 
        API.AddGemEffect<Eikthyr_Soul_Power.Config>("Eikthyr Soul Power");
        sb.AppendLine();
        sb.AppendLine("Eikthyr Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Eikthyr Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [1, 2, 3, 4, 5]");
        sb.AppendLine("  unique: Gem");

        new Gem("ElderGem", "gd_king", "Elder Soul Gem");
        API.AddGemEffect<Elder_Soul_Power.Config>("Elder Soul Power");
        sb.AppendLine();
        sb.AppendLine("Elder Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Elder Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [25, 24, 23, 22, 21]");
        sb.AppendLine("  unique: Gem");

        new Gem("BonemassGem", "Bonemass", "Bonemass Soul Gem", Gem.GemTier.None);
        API.AddGemEffect<Bonemass_Soul_Power.Config>("Bonemass Soul Power");
        sb.AppendLine();
        sb.AppendLine("Bonemass Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Bonemass Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: 40");
        sb.AppendLine("  unique: Gem");

        new Gem("ModerGem", "Dragon", "Moder Soul Gem");
        API.AddGemEffect<Moder_Soul_Power.Config>("Moder Soul Power");
        sb.AppendLine();
        sb.AppendLine("Moder Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Moder Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: [20, 30, 40, 50, 60]");
        sb.AppendLine("  unique: Gem");

        new Gem("YagluthGem", "GoblinKing", "Yagluth Soul Gem", Gem.GemTier.None);
        API.AddGemEffect<Yagluth_Soul_Power.Config>("Yagluth Soul Power");
        sb.AppendLine();
        sb.AppendLine("Yagluth Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: Yagluth Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: 40");
        sb.AppendLine("  unique: Gem");
        
        new Gem("TheQueenGem", "SeekerQueen", "The Queen Soul Gem", Gem.GemTier.None);
        API.AddGemEffect<TheQueen_Soul_Power.Config>("The Queen Soul Power");
        sb.AppendLine();
        sb.AppendLine("The Queen Soul Power:");
        sb.AppendLine("  slot: all");
        sb.AppendLine("  gem: The Queen Soul Gem");
        sb.AppendLine("  power:");
        sb.AppendLine("    value: 20");
        sb.AppendLine("  unique: Gem");
        

        string yamlData = sb.ToString();

        string YamlFilePath =
            Path.Combine(Paths.ConfigPath,
                "Jewelcrafting.Sockets_" + Assembly.GetExecutingAssembly().GetName().Name + ".yml");
        if (!File.Exists(YamlFilePath))
        {
            File.Create(YamlFilePath).Dispose();
            File.WriteAllText(YamlFilePath, yamlData);
        }

        API.AddGemConfig(yamlData);
    }


    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    public class Gem
    {
        public static readonly Dictionary<string, Sprite> CooldownIcons = new();
        public static readonly Dictionary<string, Sprite> ActiveIcons = new();


        [Flags]
        public enum GemTier
        {
            None = 0,
            Ascend = 1 << 0,
            Immortal = 1 << 1,
            Godlike = 1 << 2,
            Odinwrath = 1 << 3,

            All = Ascend | Immortal | Godlike | Odinwrath
        }


        private static readonly Dictionary<string, Texture2D> GemTextures = new();

        public string Name { get; }
        public string CostPrefab { get; }
        public int CostCount => CostCountEntry.Value;
        public int CraftDuration => CraftDurationEntry.Value;
        public GameObject Prefab { get; }
        public Sprite GetIcon { get; }
        private ConfigEntry<int> CostCountEntry;
        private ConfigEntry<int> CraftDurationEntry;
        internal ConfigEntry<bool> IsCraftable;
        public string Name_Postfix;


        public static readonly Texture2D PiecesTexture;
        private static readonly Texture2D DefaultTexture;
        private static readonly Texture2D DefaultTextureWhite;
        private static readonly Texture2D CooldownTexture;
        private static readonly Texture2D SpecialTexture;
        private static readonly Texture2D AscendTexture;
        private static readonly Texture2D ImmortalTexture;
        private static readonly Texture2D GodlikeTexture;
        private static readonly Texture2D OdinwrathTexture;

        static Gem()
        {
            DefaultTexture = asset.LoadAsset<Texture2D>("BG_Default_TS");
            DefaultTextureWhite = asset.LoadAsset<Texture2D>("BG_Default_TS_White");
            SpecialTexture = asset.LoadAsset<Texture2D>("BG_Special_TS");
            AscendTexture = asset.LoadAsset<Texture2D>("BG_Ascend_TS");
            ImmortalTexture = asset.LoadAsset<Texture2D>("BG_Immortal_TS");
            GodlikeTexture = asset.LoadAsset<Texture2D>("BG_Godlike_TS");
            OdinwrathTexture = asset.LoadAsset<Texture2D>("BG_Odinwrath_TS");
            PiecesTexture = asset.LoadAsset<Texture2D>("BG_Pieces_TS");
            CooldownTexture = asset.LoadAsset<Texture2D>("BG_Cooldown_TS");
        }

        private static void SettingChanged_ConvertionUpdate(object obj, EventArgs _)
        {
            SoulAltarUI.FillConvertionsUI();
        }

        public Gem(string prefab, string costPrefab, string JC_Name, GemTier tier = GemTier.All, string postfix = "")
        {
            Prefab = asset.LoadAsset<GameObject>(prefab);
            CostPrefab = costPrefab;
            Name_Postfix = postfix;

            CostCountEntry = config($"Gems Cost Amount", $"{Prefab.name} Cost Amount", 30,
                "Cost amount of gem creation");
            CraftDurationEntry = config($"Gems Craft Time", $"{Prefab.name} Craft Time", 600,
                "Gem craft duration (seconds)");
            IsCraftable = config("Gems Craftable", $"{Prefab.name} Craftable", true, "Set gem craftable");

            CostCountEntry.SettingChanged += SettingChanged_ConvertionUpdate;
            IsCraftable.SettingChanged += SettingChanged_ConvertionUpdate;

            GetIcon = Prefab.GetComponent<ItemDrop>().m_itemData.GetIcon();
            Name = Localization.instance.Localize(Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
            IEnumerable<Color> pixels = Prefab.GetComponentsInChildren<ParticleSystem>(true)
                .Select(ps => ps.startColor);

            float r = 0, g = 0, b = 0;
            foreach (Color pixel in pixels)
            {
                if (pixel == Color.clear) continue;
                r += pixel.r;
                g += pixel.g;
                b += pixel.b;
            }

            int count = pixels.Count();
            r /= count;
            g /= count; 
            b /= count;

            SoulColors[CostPrefab] = new Color(r, g, b);
            SoulConvertions[CostPrefab] = prefab;
            MainGems.Add(this);
            API.AddGem(Prefab, JC_Name);
            GemTextures.Add(Prefab.name, GetIcon.texture);
            GemsToObjectDB.Add(this);
            CooldownIcons[prefab] = CreateDynamicSprite(CooldownTexture, GetIcon.texture, $"{prefab}_Cooldown");
            ActiveIcons[prefab] = CreateDynamicSprite(DefaultTextureWhite, GetIcon.texture, $"{prefab}_Active");

            if (tier != GemTier.None)
            {
                Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                    CreateDynamicSprite(DefaultTexture, GetIcon.texture, $"{prefab}_Default");
            }
            else
            {
                Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                    CreateDynamicSprite(SpecialTexture, GetIcon.texture, $"{prefab}_Special");
            }

            if (tier.HasFlagFast(GemTier.Ascend))
            {
                Add_Ascend(prefab, JC_Name);
            }

            if (tier.HasFlagFast(GemTier.Immortal))
            {
                Add_Immortal(prefab, JC_Name);
            }

            if (tier.HasFlagFast(GemTier.Godlike))
            {
                Add_Godlike(prefab, JC_Name);
            }

            if (tier.HasFlagFast(GemTier.Odinwrath))
            {
                Add_Odinwrath(prefab, JC_Name);
            }
        }

        private Gem(string prefab, string JC_Name)
        {
            Prefab = asset.LoadAsset<GameObject>(prefab);
            API.AddGem(Prefab, JC_Name);
            GemsToObjectDB.Add(this);
        }

        private static Gem Add_Ascend(string original, string JC_Name)
        {
            Gem ascend_Gem = new(original + "_Ascend", JC_Name);
            ascend_Gem.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                CreateDynamicSprite(AscendTexture, GemTextures[original], $"{original}_Ascend");
            return ascend_Gem;
        }

        private static Gem Add_Immortal(string original, string JC_Name)
        {
            Gem immortal_Gem = new(original + "_Immortal", JC_Name);
            immortal_Gem.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                CreateDynamicSprite(ImmortalTexture, GemTextures[original], $"{original}_Immortal");
            return immortal_Gem;
        }

        private static Gem Add_Godlike(string original, string JC_Name)
        {
            Gem godlike_Gem = new(original + "_Godlike", JC_Name);
            godlike_Gem.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                CreateDynamicSprite(GodlikeTexture, GemTextures[original], $"{original}_Godlike");
            return godlike_Gem;
        }

        private static Gem Add_Odinwrath(string original, string JC_Name)
        {
            Gem odinwrath_Gem = new(original + "_Odinwrath", JC_Name);
            odinwrath_Gem.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] =
                CreateDynamicSprite(OdinwrathTexture, GemTextures[original], $"{original}_Odinwrath");
            return odinwrath_Gem;
        }


        public bool CanBuy(LanternComponent _lantern)
        {
            return IsCraftable.Value && _lantern.GetSouls().TryGetValue(CostPrefab, out int count) &&
                   count >= CostCount;
        }
        
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_Gems
    {
        static void Postfix(ZNetScene __instance)
        {
            foreach (Gem gem in GemsToObjectDB)
            {
                GameObject prefab = gem.Prefab;
                __instance.m_prefabs.Add(prefab);
                __instance.m_namedPrefabs.Add(prefab.name.GetStableHashCode(), prefab);
            }

            foreach (var prefab in Special_VFX_ToObjectDB)
            {
                __instance.m_prefabs.Add(prefab);
                __instance.m_namedPrefabs.Add(prefab.name.GetStableHashCode(), prefab);
            }
        }
    }

    private static void AddGems()
    {
        if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
            ObjectDB.instance.GetItemPrefab("Amber") == null) return;
        if (MainGems.Count == 0 ||
            ObjectDB.instance.GetItemPrefab(MainGems[0].Prefab.name.GetStableHashCode()) != null) return;

        foreach (Gem gem in GemsToObjectDB)
        {
            GameObject prefab = gem.Prefab;
            ObjectDB.instance.m_items.Add(prefab);
            ObjectDB.instance.m_itemByHash.Add(prefab.name.GetStableHashCode(), prefab);
        }

        GemRecipes();
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    [HarmonyPriority(-5000)]
    public static class DB_Patch_Gems
    {
        private static void Postfix()
        {
            AddGems();
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    [HarmonyPriority(-5000)]
    public static class DB_Patch_Gems2
    {
        private static void Postfix()
        {
            AddGems();
        }
    }
}