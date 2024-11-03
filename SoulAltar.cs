using System.Runtime.CompilerServices;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static GameObject Altar;
    private static GameObject AltarExplosion;
    private static readonly int Crafting = Animator.StringToHash("Crafting");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");


    static void PrepareAltar()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Soulcatcher_KG_JC_Additions.libs.SoulcatcherScripts.dll");
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        Assembly.Load(buffer);
        stream.Dispose();
        Altar = asset.LoadAsset<GameObject>("SoulAltarStation");
        Altar.GetComponent<Piece>().m_icon =
            CreateDynamicSprite(Gem.PiecesTexture, Altar.GetComponent<Piece>().m_icon.texture, "SoulAltarStation");
        AltarExplosion = asset.LoadAsset<GameObject>("SoulAltarExplosion");
        Altar.AddComponent<SoulAltarComponent>();
    }


    public class SoulAltarComponent : MonoBehaviour, Hoverable, Interactable
    {
        public static readonly List<SoulAltarComponent> _soulAltarComponents = new();
        private ZNetView znv;
        private Animator _animator;

        private Color ConversionColor
        {
            get => Utils.Vec3ToColor(znv.m_zdo.GetVec3("Conversion_Color", Vector3.one));
            set => znv.m_zdo.Set("Conversion_Color", Utils.ColorToVec3(value));
        }

        private bool IsCrafting
        {
            get => znv.m_zdo.GetBool("IsCrafting");
            set => znv.m_zdo.Set("IsCrafting", value);
        }

        private long StartTime
        {
            get => znv.m_zdo.GetLong("StartTime");
            set => znv.m_zdo.Set("StartTime", value);
        }

        private int RequestedTime
        {
            get => znv.m_zdo.GetInt("RequestedTime");
            set => znv.m_zdo.Set("RequestedTime", value);
        }

        private string ResultPrefab
        {
            get => znv.m_zdo.GetString("ResultPrefab");
            set => znv.m_zdo.Set("ResultPrefab", value);
        }


        private readonly List<ParticleSystem> _particleSystems = new();
        private readonly List<Material> _materials = new();
        private Transform PS_Transform;
        private Text _text;
        private Image _icon;
        private Light Light; 

        private void Awake() 
        {
            _internalCounter = 0;
            znv = GetComponent<ZNetView>();
            if (!znv.IsValid()) return;
            _animator = GetComponentInChildren<Animator>();
            SoulAltarAnimEvent @event = GetComponentInChildren<SoulAltarAnimEvent>();
            @event.SetAction(AnimTrigger);
            _particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>(true));
            _materials.AddRange(GetComponentsInChildren<MeshRenderer>(true).Select(x => x.material));
            PS_Transform = Utils.FindChild(transform, "PS");
            _text = transform.Find("Canvas/Text").GetComponent<Text>();
            _icon = transform.Find("Canvas/Text/Icon").GetComponent<Image>();
            Light = transform.Find("Model/PS/light").GetComponent<Light>();
            znv.Register("Soulcatcher CraftStart", new Action<long, ZPackage>(_InternalStartCraft));
            znv.Register("Soulcatcher CraftEnd", _ => CraftEnd());
            transform.Find("Model/PS/LOOP").GetComponent<AudioSource>().outputAudioMixerGroup =
                AudioMan.instance.m_masterMixer.outputAudioMixerGroup;

            if (IsCrafting)
            {
                CraftStart(ConversionColor, ResultPrefab);
            }

            _soulAltarComponents.Add(this);
        } 

        private void OnDestroy()
        {
            _soulAltarComponents.Remove(this);
        }

        public void _CheatFinishTime()
        {
            RequestedTime = 1;
            GameObject effect = ZNetScene.instance.GetPrefab("fx_guardstone_activate");
            Instantiate(effect, transform.position, Quaternion.identity);
        }


        private void _InternalStartCraft(long sender, ZPackage pkg)
        {
            if (IsCrafting) return;
            string resultPrefab = pkg.ReadString();
            int time = pkg.ReadInt();
            Color c = Utils.Vec3ToColor(pkg.ReadVector3());
            if (znv.IsOwner())
            {
                ResultPrefab = resultPrefab;
                IsCrafting = true;
                StartTime = (long)EnvMan.instance.m_totalSeconds;
                RequestedTime = time;
                ConversionColor = c;
            }

            CraftStart(c, resultPrefab);
        }

        private float CraftCounter;
        private float _internalCounter;

        void Update()
        {
            _internalCounter += Time.deltaTime;
            if (znv.m_zdo == null || !IsCrafting || !Player.m_localPlayer) return;
            CraftCounter += Time.deltaTime;
            if (CraftCounter >= 1f)
            {
                CraftCounter = 0;
                long currentSeconds = (long)EnvMan.instance.m_totalSeconds;
                float abs = currentSeconds - StartTime;
                int Percent = (int)(abs / RequestedTime * 100f);
                Percent = Mathf.Clamp(Percent, 0, 100);
                _text.text = Percent + "%";
            }
        }

        private void FixedUpdate()
        {
            if (!_text) return;
            _text.transform.rotation = Quaternion.LookRotation(GameCamera.instance.transform.forward);
        }

        private void CraftEnd()
        {
            if (znv.IsOwner())
            {
                GameObject testgo = ZNetScene.instance.GetPrefab(ResultPrefab);
                if (testgo)
                {
                    Instantiate(testgo, transform.position + Vector3.up * 2f, Quaternion.identity);
                }

                ResultPrefab = "";
                IsCrafting = false;
                ConversionColor = Color.white;
            }

            _animator.SetBool(Crafting, false);
            foreach (Material material in _materials)
            {
                material.SetColor(EmissionColor, Color.white);
            }

            foreach (ParticleSystem particleSystem in _particleSystems)
            {
                particleSystem.startColor = Color.white;
            }

            PS_Transform.transform.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
            Light.gameObject.SetActive(false);
            _icon.gameObject.SetActive(false);
            _icon.sprite = NullSprite;
        }

        private void CraftStart(Color color, string prefab)
        {
            _animator.SetBool(Crafting, true);
            Light.gameObject.SetActive(true);
            Light.color = color;
            _text.gameObject.SetActive(true);
            _text.text = "0%";
            _text.color = color;
            Gem.ActiveIcons.TryGetValue(prefab, out var result);
            if (result)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = result;
            }
            else
            {
                _icon.gameObject.SetActive(false);
            }

            foreach (Material material in _materials)
            {
                material.SetColor(EmissionColor, color);
            }

            foreach (ParticleSystem particleSystem in _particleSystems)
            {
                particleSystem.startColor = color;
            }

            PS_Transform.transform.gameObject.SetActive(true);
        }

        public void AnimTrigger()
        {
            if (znv.m_zdo == null || !_animator.GetBool(Crafting)) return;
            GameObject go = Instantiate(AltarExplosion, transform.position + Vector3.up * 1f, Quaternion.identity);
            go.GetComponent<ParticleSystemRenderer>().materials[1].color = ConversionColor;
            foreach (ParticleSystem child in go.GetComponentsInChildren<ParticleSystem>())
            {
                child.startColor = ConversionColor;
            }

            long currentSeconds = (long)EnvMan.instance.m_totalSeconds;
            if (_internalCounter > 15f && znv.HasOwner() && znv.IsOwner() && currentSeconds >= StartTime + RequestedTime)
            {
                znv.InvokeRPC(ZNetView.Everybody, "Soulcatcher CraftEnd", new object[] { null });
            }
        }

        public void StartCraft(string resultPrefab, int time, Color color)
        {
            if (IsCrafting) return;
            ZPackage pkg = new();
            pkg.Write(resultPrefab);
            pkg.Write(time);
            pkg.Write(Utils.ColorToVec3(color));
            znv.InvokeRPC(ZNetView.Everybody, "Soulcatcher CraftStart", pkg, new object[] { null });
        }


        public string GetHoverText()
        {
            return Localization.instance.Localize("[<color=yellow><b>$KEY_Use</b></color>] $soul_altar_station");
        }

        public string GetHoverName()
        {
            return Localization.instance.Localize("$soul_altar_station");
        }

        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (IsCrafting) return false;
            SoulAltarUI.Show(this);
            return true;
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }
    }


    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Awake_Patch_Altar
    {
        static void Postfix(ZNetScene __instance)
        {
            __instance.m_prefabs.Add(Altar);
            __instance.m_namedPrefabs.Add(Altar.name.GetStableHashCode(), Altar);
            PieceTable hammer = ObjectDB.instance.GetItemPrefab("Hammer").GetComponent<ItemDrop>().m_itemData.m_shared
                .m_buildPieces;
            if (!hammer.m_pieces.Contains(Altar)) hammer.m_pieces.Add(Altar);
            Altar.GetComponent<Piece>().m_category = Piece.PieceCategory.Crafting;
            Altar.GetComponent<Piece>().m_resources = new[]
            {
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("Stone").GetComponent<ItemDrop>(),
                    m_amount = 50,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("GreydwarfEye").GetComponent<ItemDrop>(),
                    m_amount = 25,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("Crystal").GetComponent<ItemDrop>(),
                    m_amount = 20,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
                new Piece.Requirement
                {
                    m_resItem = __instance.GetPrefab("Iron").GetComponent<ItemDrop>(),
                    m_amount = 20,
                    m_amountPerLevel = 0,
                    m_recover = true
                },
            };
        }
    }
}