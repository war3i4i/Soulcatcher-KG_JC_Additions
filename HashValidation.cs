using Debug = UnityEngine.Debug;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

     
    private static ICryptoTransform CreateAes()
    {
        Aes myAes = Aes.Create();
        myAes.Key = new byte[] { 2, 60, 49, 128, 197, 156, 75, 24, 63, 192, 191, 90, 189, 188, 187, 186 };
        myAes.IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        myAes.Padding = PaddingMode.None;
        myAes.Mode = CipherMode.ECB;
        return myAes.CreateEncryptor();
    }

    private static void CalculateHash()
    {
        byte[] bytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
        byte[] temp = CreateAes().TransformFinalBlock(bytes, 0, bytes.Length);
        VLD = "soulcatcherVLDsys:" + BitConverter.ToString(MD5.Create().ComputeHash(temp)).ToLower().Replace("-", "");
        VLDbytes = Encoding.UTF8.GetBytes(VLD);
    }
    
    private static string VLD;
    private static byte[] VLDbytes;
    
    
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_PeerInfo))]
    private static class PatchZNetRPC_PeerInfo
    {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(ZRpc rpc, ref ZPackage pkg)
        {
            //////server validation
            if (ZNet.instance.IsServer())
            {
                print("[Soulcatcher] Hash Validating: checking");
                if (!Encoding.UTF8.GetString(pkg.m_stream.GetBuffer()).Contains(VLD))
                {
                    Debug.LogError("[Soulcatcher] Hash Validating: ERROR");
                    rpc.Invoke("Error", 725);
                    return false;
                } 
                
                print("[Soulcatcher] Hash Validating: OK");
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.ShowConnectError))]
    static class RPC_Error_Patch
    {
        [HarmonyPriority(0)]
        static bool Prefix(FejdStartup __instance)
        {
            ZNet.ConnectionStatus connectionStatus = ZNet.GetConnectionStatus();
            if (connectionStatus == (ZNet.ConnectionStatus)725)
            {
                Task.Run(() => { MessageBox(IntPtr.Zero, "Wrong Soulcatcher Version", "", (uint)(0x00000020L)); });
                return false;
            }

            return true;
        }
    }
    
    private static readonly MethodInfo ZpkgWrite_Method =
        AccessTools.Method(typeof(ZPackage), nameof(ZPackage.Write), new[] { typeof(byte[]) });

    private static readonly MethodInfo BinaryWrite_Method =
        AccessTools.Method(typeof(BinaryWriter), nameof(BinaryWriter.Write),
            new[] { typeof(byte[]) });
    
    
    
    [HarmonyPatch(typeof(ZNet),nameof(ZNet.SendPeerInfo))]
    [HarmonyPriority(Priority.Last)]
    static class ZNet_SendPeerInfo_Patch
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> WriteHash(IEnumerable<CodeInstruction> orig)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(orig);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].operand == ZpkgWrite_Method)
                {
                    List<CodeInstruction> newCodes = new List<CodeInstruction>
                    {
                        new(OpCodes.Ldloc_0),
                        new(OpCodes.Ldfld,
                            AccessTools.Field(typeof(ZPackage), nameof(ZPackage.m_writer))),
                        new(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(Soulcatcher), nameof(VLDbytes))),
                        new(OpCodes.Callvirt, BinaryWrite_Method)
                    };
                    list.InsertRange(i + 1, newCodes);
                    break;
                }
            }
            return list;
        }
    }
    
    
}