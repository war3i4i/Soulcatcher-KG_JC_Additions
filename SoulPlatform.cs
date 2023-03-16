namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static GameObject SoulPlatform;

    private static void InitSoulPlatform()
    {
        SoulPlatform = asset.LoadAsset<GameObject>("SoulPlatform");
        SoulPlatform.GetComponent<Piece>().m_icon = CreateDynamicSprite(Gem.PiecesTexture, SoulPlatform.GetComponent<Piece>().m_icon.texture, "SoulPlatform");
        SoulPlatform.AddComponent<SoulPlatformComponent>();
    }

    public class SoulPlatformComponent : MonoBehaviour, Interactable, Hoverable
    {
        private ZNetView znet;
        private static int Enumerator;

        private string GetCurrentInsert
        {
            get => znet.m_zdo.GetString("SoulPlatform CurrentInsert");
            set => znet.m_zdo.Set("SoulPlatform CurrentInsert", value);
        }

        private float GetCurrentRotation
        {
            get => znet.m_zdo.GetFloat("SoulPlatform CurrentRotation");
            set => znet.m_zdo.Set("SoulPlatform CurrentRotation", value);
        }

        private Transform SpawnPoint;
        private GameObject CurrentSpawnedObject;
        private Transform PS_Transform;
        private ParticleSystem[] PS_List;

        private void Awake()
        {
            znet = GetComponent<ZNetView>();
            if (znet.m_zdo == null) return;
            SpawnPoint = transform.Find("SpawnPoint");
            znet.Register("SoulPlatform Insert", new Action<long, string, float>(Internal_InsertSoul));
            string insert = GetCurrentInsert;
            PS_Transform = transform.Find("Model1/CircleItemWhite");
            PS_List = transform.GetComponentsInChildren<ParticleSystem>(true);
            if (!string.IsNullOrEmpty(insert))
            {
                Internal_InsertSoul(0, insert, GetCurrentRotation);
            }
        }

        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (user.IsCrouching() && !string.IsNullOrEmpty(GetCurrentInsert))
            {
                Vector3 rotationCrouch = (Player.m_localPlayer.transform.position - transform.position).normalized;
                rotationCrouch.y = 0;

                znet.InvokeRPC(ZNetView.Everybody, "SoulPlatform Insert", GetCurrentInsert,
                    Quaternion.LookRotation(rotationCrouch).eulerAngles.y);
                return true;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                ++Enumerator;
                return true;
            }

            LanternComponent Lantern = FindLantern(out _);
            if (!Lantern)
            {
                return false;
            }

            Dictionary<string, int> souls = Lantern.GetSouls();
            int count = souls.Count;
            if (count == 0)
            {
                return false;
            }

            if (Enumerator >= count) Enumerator = 0;
            if (Enumerator < 0) Enumerator = count - 1;
            KeyValuePair<string, int> choosenSoul = souls.ElementAt(Enumerator);
            Lantern.RemoveSouls(choosenSoul.Key, 1);

            Vector3 rotation = (Player.m_localPlayer.transform.position - transform.position).normalized;
            rotation.y = 0;

            znet.InvokeRPC(ZNetView.Everybody, "SoulPlatform Insert", choosenSoul.Key,
                Quaternion.LookRotation(rotation).eulerAngles.y);
            return true;
        }


        private void Internal_InsertSoul(long sender, string prefab, float rotation)
        {
            PS_Transform.gameObject.SetActive(false);
            if (CurrentSpawnedObject) Destroy(CurrentSpawnedObject);
            SpawnPoint.transform.localScale = Vector3.one;
            if (znet.IsOwner())
            {
                GetCurrentInsert = prefab;
                GetCurrentRotation = rotation;
            }

            GameObject go = ZNetScene.instance.GetPrefab(prefab);
            Animator VisualTransform = go?.GetComponentInChildren<Animator>();
            if (!VisualTransform) return;
            CurrentSpawnedObject = Instantiate(VisualTransform, SpawnPoint).gameObject;
            CurrentSpawnedObject.GetComponent<Animator>().Update(0);
            CurrentSpawnedObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            foreach (Collider componentsInChild in CurrentSpawnedObject.GetComponentsInChildren<Collider>())
            {
                componentsInChild.enabled = false;
            }

            foreach (ParticleSystem componentsInChild in CurrentSpawnedObject.GetComponentsInChildren<ParticleSystem>())
            {
                componentsInChild.gameObject.SetActive(false);
            }

            foreach (Light componentsInChild in CurrentSpawnedObject.GetComponentsInChildren<Light>())
            {
                componentsInChild.gameObject.SetActive(false);
            }

            Quaternion rot = CurrentSpawnedObject.transform.rotation;
            Vector3 newSize = NormalizeSize(CurrentSpawnedObject);
            CurrentSpawnedObject.transform.position = SpawnPoint.position;
            CurrentSpawnedObject.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rotation, rot.eulerAngles.z);
            SpawnPoint.transform.localScale = newSize;
            PS_Transform.gameObject.SetActive(true);
            foreach (ParticleSystem particleSystem in PS_List)
            {
                particleSystem.startColor = SoulColors[prefab];
            }
        }
        

        private Vector3 NormalizeSize(GameObject go)
        {
            go.transform.position = Vector3.zero;
            Vector3 min = new Vector3(100000f, 100000f, 100000f);
            Vector3 max = new Vector3(-100000f, -100000f, -100000f);
            foreach (SkinnedMeshRenderer meshRenderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                meshRenderer.updateWhenOffscreen = true;
                min = Vector3.Min(min, meshRenderer.bounds.min);
                max = Vector3.Max(max, meshRenderer.bounds.max);
                meshRenderer.updateWhenOffscreen = false;
            }
 
            Vector3 size = new Vector3(Mathf.Abs(min.x) + Mathf.Abs(max.x), Mathf.Abs(min.y) + Mathf.Abs(max.y),
                Mathf.Abs(min.z) + Mathf.Abs(max.z));
            float Xdiff = 3f / size.x;
            float Ydiff = 3f / size.y;
            float Zdiff = 3f / size.z;
            float maxDiff = Mathf.Min(Xdiff, Ydiff, Zdiff);
            return maxDiff >= 1f ? Vector3.one : new Vector3(maxDiff, maxDiff, maxDiff);
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public string GetHoverText()
        {
            if (Player.m_localPlayer.IsCrouching() && !string.IsNullOrEmpty(GetCurrentInsert))
            {
                return Localization.instance.Localize(
                    $"[<color=yellow><b>$KEY_Use</b></color>] <color=#00FFFF>Rotate</color>");
            }

            LanternComponent Lantern = FindLantern(out _);
            if (!Lantern)
            {
                return "<color=#00FFFF>Equip Lantern to use platform</color>";
            }

            Dictionary<string, int> souls = Lantern.GetSouls();
            int count = souls.Count;
            if (count == 0)
            {
                return "<color=#00FFFF>No Souls</color>";
            }

            if (Enumerator >= count) Enumerator = 0;
            if (Enumerator < 0) Enumerator = count - 1;

            KeyValuePair<string, int> choosenSoul = souls.ElementAt(Enumerator);


            string mainStr = Localization.instance.Localize(
                $"[<color=yellow><b>$KEY_Use</b></color>] <color=#00FFFF>insert</color> <color={SoulColors[choosenSoul.Key].ToHex()}>{LanternComponent.GetLocalizedName(choosenSoul.Key)}</color>");
            string addStr = Localization.instance.Localize(
                $"\n[<color=yellow><b>L Shift + $KEY_Use</b></color>] <color=#00FFFF>Next Soul</color>");
            return mainStr + addStr;
        }

        public string GetHoverName()
        {
            return "";
        }
    }


    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_SoulPlatform
    {
        static void Postfix(ZNetScene __instance)
        {
            __instance.m_prefabs.Add(SoulPlatform);
            __instance.m_namedPrefabs.Add(SoulPlatform.name.GetStableHashCode(), SoulPlatform);
            PieceTable hammer = ObjectDB.instance.GetItemPrefab("Hammer").GetComponent<ItemDrop>().m_itemData.m_shared
                .m_buildPieces;
            if (!hammer.m_pieces.Contains(SoulPlatform)) hammer.m_pieces.Add(SoulPlatform);
            SoulPlatform.GetComponent<Piece>().m_category = Piece.PieceCategory.Furniture;
            SoulPlatform.GetComponent<Piece>().m_resources = new[]
            {
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("Stone").GetComponent<ItemDrop>(),
                    m_amount = 10,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("GreydwarfEye").GetComponent<ItemDrop>(),
                    m_amount = 5,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("Crystal").GetComponent<ItemDrop>(),
                    m_amount = 1,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
            };
        }
    }
}