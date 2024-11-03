using CompressionLevel = System.IO.Compression.CompressionLevel;
using Object = UnityEngine.Object;


namespace Soulcatcher_KG_JC_Additions;

[BepInPlugin(GUID, PluginName, PluginVersion)]
[BepInDependency("org.bepinex.plugins.jewelcrafting")]
public partial class Soulcatcher : BaseUnityPlugin
{
    private const string GUID = "Soulcatcher";
    private const string PluginName = "Soulcatcher";
    private const string PluginVersion = "4.5.7"; 
    private static AssetBundle asset; 
    public static Soulcatcher _thistype; 
    private static readonly string ConfigFileName = GUID + ".cfg";
 
    private static readonly ConfigSync configSync = new(GUID)
        { DisplayName = PluginName, CurrentVersion = PluginVersion };

 
    public void Awake()
    {
        Ulv_Soul_Power.Script_Layermask = LayerMask.GetMask("character",
            "character_noenv", "piece", "terrain"); 
        Cultist_Soul_Power.LayerForCultist = LayerMask.GetMask("character");
        Wraith_Soul_Power.JumpMask = LayerMask.GetMask("terrain", "Default", "piece", "static_solid"); 
        CE_UseCachedSprites = Config.Bind("Sprites", "UseCachedSprites", true,
            "Use cached sprites instead of loading them every time");
        if (CE_UseCachedSprites.Value) LoadCachedSprites();
        Stopwatch stopwatch = Stopwatch.StartNew();
        JSON.Parameters = new JSONParameters
        {
            UseExtensions = false,
            SerializeNullValues = false,
            DateTimeMilliseconds = false,
            UseUTCDateTime = true,
            UseOptimizedDatasetSchema = true, 
            UseValuesOfEnums = true
        };
        _thistype = this; 
        asset = GetAssetBundle("soulcatcher"); 
        Localizer.Load();
        PrepareSoulComponent();
        PrepareLantern();
        PrepareAltar();
        InitGems();
        InitSoulcatcherSkill();
        InitSoulPlatform(); 
        InitJewelry();
        ScreenshotManager = new();
        ScreenshotManager_Ghost = new();
        SoulAltarUI.Init();
        SetupWatcher();
        InitCustomConvertions();
        CursedDoll = asset.LoadAsset<GameObject>("Soulcatcher_CursedDoll");
        CursedDoll.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0] = CreateDynamicSprite(Gem.PiecesTexture,
            CursedDoll.GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0].texture, "CursedDoll");
        Jewelcrafting.API.OnItemBreak(Soulcatcher_KG_JC_Additions.CursedDoll.BreakHandler_CursedDoll);
        new Harmony(GUID).PatchAll();
        print(
            "If you have an error \"The power must be a list of exactly 4 numbers denoting the strength of the effect....\" then delete BepInEx/Config/Jewelcrafting.Sockets_Soulcatcher_KG_JC_Additions.yml file and restart the game");
        stopwatch.Stop();
        print($"{PluginName} v{PluginVersion} loaded in {stopwatch.ElapsedMilliseconds}ms");
    }

    public static GameObject CursedDoll;

    private static void SetupWatcher()
    {
        FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
        watcher.Changed += _thistype.ConfigChanged;
        watcher.IncludeSubdirectories = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }

    private DateTime LastConfigChange;

    private void ConfigChanged(object sender, FileSystemEventArgs e)
    {
        if ((DateTime.Now - LastConfigChange).TotalSeconds <= 5) return;
        LastConfigChange = DateTime.Now;
        try
        {
            print("Reloading Config");
            Config.Reload();
        }
        catch
        {
            print($"Can't reload Config");
        }
    }

    private static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        ConfigEntry<T> configEntry = _thistype.Config.Bind(group, name, value, description);

        SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }
 

    public static ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true) =>
        config(group, name, value, new ConfigDescription(description), synchronizedSetting);


    private static AssetBundle GetAssetBundle(string filename)
    {
        Assembly execAssembly = Assembly.GetExecutingAssembly();
        string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));
        using Stream stream = execAssembly.GetManifestResourceStream(resourceName);
        return AssetBundle.LoadFromStream(stream);
    }


    private const int CachedSpritesVersion = 2;
    private static readonly Dictionary<int, Sprite> CachedSprites = new();
    private static ConfigEntry<bool> CE_UseCachedSprites;

    private static void LoadCachedSprites()
    {
        string path = Path.Combine(Paths.ConfigPath, $"Soulcatcher_CachedSprites_v{CachedSpritesVersion}");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        foreach (string file in Directory.GetFiles(path))
        {
            try
            {
                int id = Path.GetFileNameWithoutExtension(file).GetStableHashCode();
                byte[] data = File.ReadAllBytes(file);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(data);
                CachedSprites[id] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            }
            catch (Exception e)
            {
                print($"Error loading cached sprite {file}: {e.Message}");
            }
        }
    }

    private static void SaveCachedSprite(string Key, Texture2D sprite)
    {
        string path = Path.Combine(Paths.ConfigPath, $"Soulcatcher_CachedSprites_v{CachedSpritesVersion}");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllBytes(Path.Combine(path, $"{Key}.png"), sprite.EncodeToPNG());
    }


    private static Sprite CreateDynamicSprite(Texture2D background, Texture2D watermark, string Key)
    {
        if (CE_UseCachedSprites.Value && CachedSprites.TryGetValue(Key.GetStableHashCode(), out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Texture2D NewTex = new Texture2D(256, 256);
        for (int x = 0; x < background.width; x++)
        {
            for (int y = 0; y < background.height; y++)
            {
                Color bgColor = background.GetPixel(x, y);
                Color wmColor = watermark.GetPixel(x, y);
                Color lerp = Color.Lerp(bgColor, wmColor, wmColor.a);
                NewTex.SetPixel(x, y, lerp);
            }
        }

        NewTex.Apply();
        if (CE_UseCachedSprites.Value) SaveCachedSprite(Key, NewTex);
        Sprite sprite = Sprite.Create(NewTex, new Rect(0, 0, NewTex.width, NewTex.height), new Vector2(0.5f, 0.5f),
            100f);
        return sprite;
    }

    private static string FormatTime(int secs)
    {
        int hours = secs / 3600;
        int mins = (secs % 3600) / 60;
        secs %= 60;
        if (hours > 0)
            return $"{hours}<color=#00FF00>h</color> {mins:D2}<color=#00FF00>m</color> {secs:D2}<color=#00FF00>s</color>";
        return mins > 0
            ? $"{mins}<color=#00FF00>m</color> {secs:D2}<color=#00FF00>s</color>"
            : $"{secs}<color=#00FF00>s</color>";
    }

    public static void print(object obj)
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            ConsoleManager.SetConsoleColor(ConsoleColor.DarkCyan);
            ConsoleManager.StandardOutStream.WriteLine($"[Soulcatcher] {obj}");
            ConsoleManager.SetConsoleColor(ConsoleColor.White);
        } 
        else 
        {
            MonoBehaviour.print("[Soulcatcher] " + obj);
        }
    }

    public class SE_GenericInstantiate : StatusEffect
    {
        public SE_GenericInstantiate()
        {
            //"Soulcatcher For Instantiate Only (Cooldown)";
            m_tooltip = "";
            m_icon = NullSprite;
            m_name = "";
            m_ttl = 100;
        }
    }
}

public static class EXTENTIONS
{
    public static bool HasFlagFast(this Soulcatcher.Gem.GemTier tier, Soulcatcher.Gem.GemTier flag)
    {
        return (tier & flag) != 0;
    }

    public static string ToHex(this Color c)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(c);
    }

    public static void Compress(this ZPackage pkg, CompressionLevel mode = CompressionLevel.Fastest)
    {
        byte[] array = pkg.GetArray();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, mode))
                gzipStream.Write(array, 0, array.Length);
            byte[] compress = memoryStream.ToArray();
            pkg.Clear();
            pkg.m_writer.Write(compress);
        }
    }


    public static void Decompress(this ZPackage pkg)
    {
        byte[] decompress = Utils.Decompress(pkg.GetArray());
        pkg.Clear();
        pkg.m_writer.Write(decompress);
        pkg.m_stream.Position = 0L;
    }
}