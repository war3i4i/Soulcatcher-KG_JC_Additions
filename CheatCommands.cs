namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private const string All_Souls_Command = "/sc souls";
    private const string Add_Soul_Command = "/sc soul ";
    private const string Finish_Altars = "/sc altar time";
    private const string Update_Convertion = "/sc update convertions";


    [HarmonyPatch(typeof(Chat), nameof(Chat.InputText))]
    static class Cheat_Commands
    {
        static bool Prefix(Chat __instance)
        {
            if (!Player.m_debugMode) return true;
            string text = __instance.m_input.text;

            if (text.ToLower() == All_Souls_Command && FindLantern(out _) is { } cheatAllLantern)
            {
                cheatAllLantern.CheatGetSouls();
                __instance.AddString("<color=#00FFFF>Cheat Souls Added</color>");
                return false;
            }

            if (text.ToLower().StartsWith(Add_Soul_Command))
            {
                int l = Add_Soul_Command.Length;

                string cmd = text.Substring(l);
                string[] _params = cmd.Split(' ');
                if (_params.Length != 2)
                {
                    __instance.AddString("<color=red>Invalid Parameters</color>");
                }
                else
                {
                    string prefab = _params[0];
                    if (!SoulConvertions.ContainsKey(prefab))
                    {
                        __instance.AddString("<color=red>Wrong Prefab</color>");
                        return false;
                    }

                    int amount = int.Parse(_params[1]);
                    if (FindLantern(out _) is { } cheatSingleLantern && amount > 0)
                    {
                        __instance.AddString(
                            $"<color=#00FF00>Added x{amount} {LanternComponent.GetLocalizedName(prefab)} to lantern</color>");
                        cheatSingleLantern.AddSoul(prefab, amount, true);
                    }
                }

                return false;
            }

            if (text.ToLower() == Finish_Altars)
            {
                IEnumerable<SoulAltarComponent> altars = SoulAltarComponent._soulAltarComponents.Where(d =>
                    Utils.DistanceXZ(d.transform.position, Player.m_localPlayer.transform.position) <= 20);
                int c = 0;
                foreach (var component in altars)
                {
                    component._CheatFinishTime();
                    ++c;
                }

                __instance.AddString($"<color=#00FFFF>Finished Time for {c} Altars</color>");
                return false;
            }

            if (text.ToLower() == Update_Convertion)
            {
                SoulAltarUI.FillConvertionsUI();
                __instance.AddString($"<color=#00FFFF>Updated Convertions UI</color>");
                return false;
                
            }
            
            

            return true;
        }
    }
}