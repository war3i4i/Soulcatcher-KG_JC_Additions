namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    IEnumerator HealEffect(Dictionary<GameObject, KeyValuePair<Character, Vector3>> data, float HealValue)
    {
        float count = 0;
        while (count < 1f)
        {
            if (!Player.m_localPlayer) break; 
            count += 1f * Time.deltaTime;
            foreach (KeyValuePair<GameObject, KeyValuePair<Character, Vector3>> c in data)
            {
                if (!c.Value.Key || c.Value.Key.GetHealth() <= 0) continue;

                Vector3 point = c.Value.Value + (c.Value.Key.transform.position - c.Value.Value) / 2 +
                                Vector3.up * 7.0f;
                Vector3 m1 = Vector3.Lerp(c.Value.Value, point, count);
                Vector3 m2 = Vector3.Lerp(point, c.Value.Key.transform.position + Vector3.up, count);
                c.Key.transform.position = Vector3.Lerp(m1, m2, count);
            }

            yield return null;
        }
        
        foreach (KeyValuePair<GameObject, KeyValuePair<Character, Vector3>> go in data)
        {
            if (go.Value.Key && go.Value.Key.GetHealth() > 0)
            {
                go.Value.Key.Heal(HealValue);
                Instantiate(Greydwarf_Soul_Power.VFX2, go.Value.Key.transform.position, Quaternion.identity);
            }
            ZNetScene.instance.Destroy(go.Key);
        }
    }
    
    
    
    public static class Greydwarf_Soul_Power
    {
        private static GameObject VFX;
        public static GameObject VFX2;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class GreydwarfGem_Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("GreydwarfGem_VFX");
                VFX2 = asset.LoadAsset<GameObject>("GreydwarfGem_VFX2");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);

                __instance.m_prefabs.Add(VFX2);
                __instance.m_namedPrefabs.Add(VFX2.name.GetStableHashCode(), VFX2);
            }
        }
 
        [StructLayout(LayoutKind.Sequential)]
        public struct Config
        {
            [AdditivePower] public float Value;
            [MinPower] public float Cooldown;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
        static class Player_SetLocalPlayer_Patch
        { 
            private static int Counter;

            static IEnumerator HealCoroutine()
            {
                Counter = 0;
                for (;;)
                {
                    yield return new WaitForSeconds(3f);
                    Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Greydwarf Soul Power");
                    if (Effect.Value > 0)
                    { 
                        Counter += 3; 
                        if (Counter >= Effect.Cooldown && (int)Player.m_localPlayer.GetHealth() <
                            (int)Player.m_localPlayer.GetMaxHealth())
                        { 
                            Counter = 0;
                            Dictionary<GameObject, KeyValuePair<Character, Vector3>> data = new Dictionary<GameObject, KeyValuePair<Character, Vector3>>();
                            IEnumerable<Character> list = Character.GetAllCharacters()
                                .Where(d => Vector3.Distance(Player.m_localPlayer.transform.position, d.transform.position) <= 20f && !EnemyCondition(d));
                            foreach (Character VARIABLE in list)
                            {
                                GameObject go = Instantiate(VFX, Player.m_localPlayer.transform.position + Vector3.up,
                                    Quaternion.identity);
                                KeyValuePair<Character, Vector3> kvp = new KeyValuePair<Character, Vector3>(VARIABLE, go.transform.position);
                                data.Add(go, kvp); 
                            }
                            if (data.Count > 0)
                            {
                                _thistype.StartCoroutine(_thistype.HealEffect(data, Effect.Value / 100f * Player.m_localPlayer.GetMaxHealth()));
                            }
                        }
                    }
                }
            }


            static void Postfix()
            {
                Player.m_localPlayer.StartCoroutine(HealCoroutine());
            }
        }
    }
}
