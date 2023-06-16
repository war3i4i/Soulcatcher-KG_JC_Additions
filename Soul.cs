using Random = UnityEngine.Random;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static GameObject SoulPrefab;
    private static GameObject SoulTornado;
    private static GameObject SoulImpact;
    private static Material SoulMaterial;
    private static Material SoulMaterialTest;

    private static readonly Dictionary<string, Color> SoulColors = new();
    private static readonly Dictionary<string, string> SoulConvertions = new();


    private static void PrepareSoulComponent()
    {
        SoulPrefab = asset.LoadAsset<GameObject>("Soulcatcher-Soul");
        SoulMaterial = asset.LoadAsset<Material>("SoulcatcherMat");
        SoulMaterialTest = asset.LoadAsset<Material>("SoulcatcherMatTest");
        SoulTornado = asset.LoadAsset<GameObject>("SoulcatcherTornado");
        SoulImpact = asset.LoadAsset<GameObject>("SoulcatcherImpact");
        Special_VFX_ToObjectDB.Add(SoulPrefab);
        Special_VFX_ToObjectDB.Add(SoulTornado);
        Special_VFX_ToObjectDB.Add(SoulImpact);
        SoulPrefab.AddComponent<SoulComponent>();
    }


    public class SoulComponent : MonoBehaviour
    {
        private ZNetView znv;
        private ZSyncAnimation zanim;
        private bool IsSucking;
        private float Scale;
        private GameObject Tornado;
        private const float InitialScale = 1f;
        private float CaptureTime;
        public bool DoubleSoul;


        public string Prefab
        {
            get => znv.m_zdo.GetString("Prefab");
            set => znv.m_zdo.Set("Prefab", value);
        }

        private void Awake()
        {
            Scale = InitialScale;
            transform.localScale = new Vector3(Scale, Scale, Scale);
            znv = GetComponent<ZNetView>();
            zanim = GetComponent<ZSyncAnimation>();
            IsSucking = false;
            CaptureTime = Player.m_localPlayer ? 4f - Player.m_localPlayer.GetSkillFactor("Soulcatcher") * 2f : 4f;
            if (znv.IsOwner()) return;
            Setup(Prefab);
        }

        private void OnDestroy()
        {
            if (Tornado) ZNetScene.instance.Destroy(Tornado);
        }

        public void Setup(string prefab)
        {
            GameObject go = ZNetScene.instance.GetPrefab(prefab);
            if (!go) ZNetScene.instance.Destroy(gameObject);
            if (znv.IsOwner())
            {
                Prefab = prefab;
                DoubleSoul = Random.Range(0, 100) <= Player.m_localPlayer.GetSkillFactor("Soulcatcher") * 15;
            }

            Animator VisualTransform = go.GetComponentInChildren<Animator>();
            if (VisualTransform)
            {
                Animator soul = Instantiate(VisualTransform, gameObject.transform);
                foreach (Collider componentsInChild in soul.GetComponentsInChildren<Collider>())
                {
                    componentsInChild.enabled = false;
                }

                soul.transform.localPosition = Vector3.zero;
                soul.transform.localRotation = gameObject.transform.rotation;
                Replacematerial(SoulMaterial);
                zanim.m_animator = soul.GetComponent<Animator>();
            }
        }

        void Replacematerial(Material mat)
        {
            SkinnedMeshRenderer[] AllMaterials = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in AllMaterials)
            {
                int Materials = skinnedMeshRenderer.materials.Length;
                Material[] newMaterials = new Material[Materials];
                for (int i = 0; i < Materials; i++)
                {
                    newMaterials[i] = mat;
                }

                skinnedMeshRenderer.materials = newMaterials;

                if (znv.IsOwner() && DoubleSoul)
                {
                    foreach (Material material in skinnedMeshRenderer.materials)
                    {
                        Color c = material.GetColor(TintColor);
                        c.r = 1f;
                        c.g = 1f;
                        material.SetColor(TintColor, c);
                        c = material.GetColor(CoreColor);
                        c.r = 1f;
                        c.g = 1f;
                        material.SetColor(CoreColor, c);
                    }
                }
                
                if (!znv.IsOwner())
                {
                    foreach (Material material in skinnedMeshRenderer.materials)
                    {
                        Color c = material.GetColor(TintColor);
                        c.r = 1f;
                        material.SetColor(TintColor, c);
                        c = material.GetColor(CoreColor);
                        c.r = 1f;
                        material.SetColor(CoreColor, c);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (znv.m_zdo == null || !znv.IsOwner() || !Player.m_localPlayer) return;
            IsSucking = false;
            if (!LanternMainCondition)
            {
                if (Tornado) ZNetScene.instance.Destroy(Tornado);
                return;
            }

            float maxDistance = Player.m_localPlayer.m_seman.GetStatusEffect("SoulRing".GetStableHashCode()) ? 40f : 20f;

            bool IsGameObjectInFront = Vector3.Angle(Player.m_localPlayer.transform.forward,
                (gameObject.transform.position - Player.m_localPlayer.transform.position).normalized) < 60f;

            if (Vector3.Distance(Player.m_localPlayer.transform.position, transform.position) < maxDistance &&
                IsGameObjectInFront)
            {
                IsSucking = true;
                if (!Tornado)
                {
                    Tornado = Instantiate(SoulTornado, LanternTransform.position, Quaternion.identity);
                    Tornado.transform.Find("SFX").gameObject.SetActive(true);
                }

                TornadoPositioning();
            }
            else
            {
                IsSucking = false;
                if (Tornado) ZNetScene.instance.Destroy(Tornado);
            }
        }


        private void TornadoPositioning()
        {
            Vector3 initialTransform = LanternTransform.position;
            Vector3 finalTransform = gameObject.transform.position + Vector3.up * 0.5f;
            float distance = Vector3.Distance(initialTransform, finalTransform);
            Vector3 direction = (finalTransform - initialTransform).normalized;
            Tornado.transform.rotation = Quaternion.LookRotation(direction);
            Tornado.transform.position = initialTransform;
            Tornado.transform.localScale = new Vector3(1, 1, distance);
        }


        private bool MoveUp = true;
        private float MoveCount;
        private bool IsDone;

        private void Update()
        {
            if (znv.m_zdo == null || !znv.IsOwner() || IsDone) return;
            Player p = Player.m_localPlayer;
            if (!p) return;


            gameObject.transform.rotation =
                Quaternion.LookRotation((Player.m_localPlayer.transform.position - gameObject.transform.position)
                    .normalized);
            if (IsSucking)
            {
                Vector3 targetPoint = LanternTransform.position - Vector3.up * 0.2f +
                                      p.transform.forward * 0.5f;
                float dt = Time.deltaTime / CaptureTime;
                Scale -= dt;
                if (Scale <= 0)
                {
                    IsDone = true;
                    OnSoulGain(this);
                    Instantiate(SoulImpact, targetPoint, Quaternion.identity);
                    if (Tornado) ZNetScene.instance.Destroy(Tornado);
                    ZNetScene.instance.Destroy(gameObject);
                    return;
                }

                float clampedScale = Mathf.Clamp(Scale, 0.25f, 1.5f);
                gameObject.transform.localScale = new Vector3(clampedScale, clampedScale, clampedScale);

                float distance = Vector3.Distance(targetPoint, gameObject.transform.position);
                float speed = distance * dt / Scale;
                Vector3 newPosition = Vector3.MoveTowards(gameObject.transform.position, targetPoint, speed);
                gameObject.transform.position = newPosition;
                TornadoPositioning();
                zanim.SetFloat(Character.s_forwardSpeed, 1.5f);
            }
            else
            {
                zanim.SetFloat(Character.s_forwardSpeed, 0);
                float add = MoveUp ? 0.005f : -0.005f;
                transform.position += new Vector3(0, add, 0);
                MoveCount += Time.deltaTime;
                if (MoveCount > 2)
                {
                    MoveUp = !MoveUp;
                    MoveCount = 0;
                }
            }
        }
    }


    private const int Chance = 10;
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");
    private static readonly int CoreColor = Shader.PropertyToID("_CoreColor");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private static void SoulCreation(string prefab, Vector3 pos, int level)
    {
        //print($"Just killed: {prefab}");
        string soulspawn = null;
        if (!Player.m_localPlayer || !HasLantern()) return;
        if (!SoulConvertions.ContainsKey(prefab) &&
            !_soulcatcher_soulspawn_additions.TryGetValue(prefab, out soulspawn)) return;
        int random = Player.m_debugMode ? 0 : Random.Range(0, 101);
        int AdditionalChance = level * 1 + (int)(Player.m_localPlayer.GetSkillFactor("Soulcatcher") * 5f);
        if (Player.m_localPlayer.m_seman.GetStatusEffect("SoulNecklace".GetStableHashCode()))
        {
            AdditionalChance += 10;
        }

        if (random > Chance + AdditionalChance) return;
        if (!string.IsNullOrEmpty(soulspawn))
        {
            prefab = soulspawn;
            if (!SoulConvertions.ContainsKey(prefab)) return;
        }

        GameObject go = Instantiate(SoulPrefab, pos + Vector3.up * 1.5f, Quaternion.identity);
        go.GetComponent<SoulComponent>().Setup(prefab);
    }
}