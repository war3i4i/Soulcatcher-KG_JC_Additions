namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private IEnumerator GreydwarfShamanGem_Grenade()
    {
        var grenade = Instantiate(GreydwarfShaman_Soul_Power.VFX,
            Player.m_localPlayer.transform.position + Vector3.up * 1.5f,
            Quaternion.identity);
        float count = 0;
        while (count <= 1f)
        {
            if (!Player.m_localPlayer || Player.m_localPlayer.IsDead())
            {
                if (grenade) ZNetScene.instance.Destroy(grenade);
                yield break;
            }

            grenade.transform.position += Vector3.up * 0.05f;
            count += Time.deltaTime;
            yield return null;
        }

        Instantiate(GreydwarfShaman_Soul_Power.VFX2, grenade.transform.position, Quaternion.identity);
        var list = new List<Character>();
        Character.GetCharactersInRange(grenade.transform.position, 10, list);
        foreach (var c in list)
            if (EnemyCondition(c))
                c.Stagger(Vector3.zero);
        ZNetScene.instance.Destroy(grenade);
    }


    public static class GreydwarfShaman_Soul_Power
    {
        public static GameObject VFX;
        public static GameObject VFX2;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("GreydwarfShaman_VFX");
                VFX2 = asset.LoadAsset<GameObject>("GreydwarfShaman_VFX2");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                __instance.m_prefabs.Add(VFX2);
                __instance.m_namedPrefabs.Add(VFX2.name.GetStableHashCode(), VFX2);
            }
        }


        public struct Config
        {
            [MinPower] public float Value;
        }


        [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
        static class Player_SetLocalPlayer_Patch
        {
            private static int CD;


            static IEnumerator Corout()
            {
                for (;;)
                {
                    yield return new WaitForSeconds(3f);
                    CD += 3;
                    float Effect = Player.m_localPlayer.GetEffectPower<Config>("GreydwarfShaman Soul Power").Value;
                    if (Effect > 0 && CD >= Effect)
                    {
                        CD = 0;
                        IEnumerable<Character> list = Character.GetAllCharacters().Where(c =>
                            EnemyCondition(c) &&
                            Vector3.Distance(c.transform.position, Player.m_localPlayer.transform.position) <= 9f);
                        if (list.Any())
                        {
                            Player.m_localPlayer.StartCoroutine(_thistype.GreydwarfShamanGem_Grenade());
                        }
                    }
                }
            }


            static void Postfix()
            {
                Player.m_localPlayer.StartCoroutine(Corout());
            }
        }
    }
}