namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static readonly Dictionary<string, string> _soulcatcher_soulspawn_additions = new();
    private static string _soulcatcher_soulspawn_JSON;
    private static string soulspawnFile;

    private static void InitCustomConvertions()
    {
        soulspawnFile = Path.Combine(BepInEx.Paths.ConfigPath, "Soulcatcher_Custom_SoulSpawn.cfg");
        if (!File.Exists(soulspawnFile)) File.Create(soulspawnFile).Dispose();
        ReadCustomSpawns();
        FileSystemWatcher watcher = new(Path.GetDirectoryName(soulspawnFile), Path.GetFileName(soulspawnFile) );
        watcher.Changed += CustomConvertionsFSW;
        watcher.IncludeSubdirectories = false;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }

    private static void CustomConvertionsFSW(object sender, FileSystemEventArgs e)
    {
        if(ZNet.instance == null || !ZNet.instance.IsServer()) return;
        print($"Custom Soul Spawn File Changed, reloading...");
        ReadCustomSpawns();
        if (string.IsNullOrEmpty(_soulcatcher_soulspawn_JSON)) ;
        ZPackage pkg = new();
        pkg.Write(_soulcatcher_soulspawn_JSON);
        pkg.Compress();
        foreach (ZNetPeer peer in ZNet.instance.m_peers)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "Soulcatcher_Custom_SoulSpawn", pkg);
        }
    }

    private static void ReadCustomSpawns()
    {
        _soulcatcher_soulspawn_additions.Clear();
        try
        {
            string[] convertionLines = File.ReadAllLines(soulspawnFile);
            foreach (string convertionLine in convertionLines)
            {
                string[] convertionParts = convertionLine.Replace(" ", "").Split(':');
                if (convertionParts.Length == 2)
                {
                    string convertionName = convertionParts[0];
                    string convertionValue = convertionParts[1];
                    if (string.IsNullOrEmpty(convertionName) || string.IsNullOrEmpty(convertionValue)) continue;
                    _soulcatcher_soulspawn_additions[convertionName] = convertionValue;
                    //print($"{convertionName} added to custom soul spawn with {convertionValue}");
                }
            }
        }
        catch
        {
            // ignored
        }
        _soulcatcher_soulspawn_JSON = fastJSON.JSON.ToJSON(_soulcatcher_soulspawn_additions);
    }


    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_PeerInfo))]
    private static class ZnetSyncTeleporterProfiles
    {
        private static void Postfix(ZRpc rpc)
        {
            if (!ZNet.instance.IsServer()) return;
            ZNetPeer peer = ZNet.instance.GetPeer(rpc);
            if (peer == null || string.IsNullOrEmpty(_soulcatcher_soulspawn_JSON)) return;
            ZPackage pkg = new();
            pkg.Write(_soulcatcher_soulspawn_JSON);
            pkg.Compress();
            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "Soulcatcher_Custom_SoulSpawn", pkg);
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class RegisterRPC
    {
        static void Postfix()
        {
            ZRoutedRpc.instance.Register("Soulcatcher_Custom_SoulSpawn",
                new Action<long, ZPackage>(Get_SoulcatcherSpawnData));
        }

        private static void Get_SoulcatcherSpawnData(long sender, ZPackage data)
        {
            _soulcatcher_soulspawn_additions.Clear();
            data.Decompress();
            _soulcatcher_soulspawn_additions.AddRange(
                fastJSON.JSON.ToObject<Dictionary<string, string>>(data.ReadString()));
        }
    }
    
    
}