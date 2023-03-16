namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static Sprite PLACEHOLDERMONSTERICON;
    private static PrefabScrenshotManager ScreenshotManager;
    private static PrefabScrenshotManager ScreenshotManager_Ghost;
    private static readonly int Hue = Shader.PropertyToID("_Hue");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");
    private static readonly int Value = Shader.PropertyToID("_Value");


    private class PrefabScrenshotManager
    {
        private readonly GameObject INACTIVE;
        private readonly Camera rendererCamera;
        private readonly Light Light;
        private readonly Dictionary<long, Sprite> CachedSprites = new();
        private static readonly Vector3 SpawnPoint = new Vector3(10000f, 10000f, 10000f); 


        public Sprite GetSprite(string prefab, Sprite defaultValue)
        {
            int initHashcode = prefab.GetStableHashCode();
            if (CachedSprites.ContainsKey(initHashcode))
                return CachedSprites[initHashcode];
            return defaultValue;
        }

        private class RenderObject
        {
            public readonly GameObject Spawn;
            public readonly Vector3 Size;
            public RenderRequest Request;

            public RenderObject(GameObject spawn, Vector3 size)
            {
                Spawn = spawn;
                Size = size;
            }
        }

        private class RenderRequest
        {
            public readonly GameObject Target;
            public int Width { get; set; } = 128;
            public int Height { get; set; } = 128;
            public Quaternion Rotation { get; set; } = Quaternion.Euler(0f, -24f, 0); //25.8f);
            public float FieldOfView { get; set; } = 0.5f;
            public float offset = 0;

            public float DistanceMultiplier { get; set; } = 1f;

            public RenderRequest(GameObject target)
            {
                Target = target;
            }
        }

        public PrefabScrenshotManager()
        {
            Texture2D monsterTex = new Texture2D(1, 1);
            monsterTex.LoadImage(Convert.FromBase64String(PlaceholderMonster));
            PLACEHOLDERMONSTERICON = Sprite.Create(monsterTex, new Rect(0, 0, monsterTex.width, monsterTex.height),
                Vector2.zero);

            INACTIVE = new GameObject("INACTIVEscreenshotHelper")
            {
                layer = 3,
                transform =
                {
                    localScale = Vector3.one
                }
            };
            INACTIVE.SetActive(false);
            DontDestroyOnLoad(INACTIVE);
            rendererCamera = new GameObject("RenderStuff", typeof(Camera)).GetComponent<Camera>();
            rendererCamera.backgroundColor = new Color(0, 0, 0, 0);
            rendererCamera.clearFlags = CameraClearFlags.SolidColor;
            rendererCamera.transform.position = SpawnPoint;
            rendererCamera.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            rendererCamera.fieldOfView = 0.5f;
            rendererCamera.farClipPlane = 100000;
            rendererCamera.cullingMask = 8;
            DontDestroyOnLoad(rendererCamera);

            Light = new GameObject("Light", typeof(Light)).GetComponent<Light>();
            Light.transform.position = SpawnPoint;
            Light.transform.rotation = Quaternion.Euler(5f, 180f, 5f);
            Light.type = LightType.Directional;
            Light.intensity = 30f;
            Light.cullingMask = 1 << 8;
            DontDestroyOnLoad(Light);

            rendererCamera.gameObject.SetActive(false);
            Light.gameObject.SetActive(false);
        }

        private void ClearRendering()
        {
            rendererCamera.gameObject.SetActive(false);
            Light.gameObject.SetActive(false);
        }

        private static bool IsVisualComponent(Component component)
        {
            return component is Renderer or MeshFilter or Transform or Animator
                or LevelEffects;
        }


        private GameObject SpawnAndRemoveComponents(RenderRequest obj)
        {
            GameObject tempObj = Instantiate(obj.Target, INACTIVE.transform);
            List<Component> components = tempObj.GetComponents<Component>().ToList();
            components.AddRange(tempObj.GetComponentsInChildren<Component>(true));
            List<Component> ToRemove = new List<Component>();
            foreach (Component comp in components)
            {
                if (!IsVisualComponent(comp))
                {
                    ToRemove.Add(comp);
                }
            }

            ToRemove.Reverse();
            ToRemove.ForEach(DestroyImmediate);
            GameObject retObj = Instantiate(tempObj);
            retObj.layer = 3;
            foreach (Transform VARIABLE in retObj.GetComponentsInChildren<Transform>())
            {
                VARIABLE.gameObject.layer = 3;
            }

            Animator animator = retObj.GetComponentInChildren<Animator>();
            if (animator)
            {
                if (animator.HasState(0, Movement))
                    animator.Play(Movement);
                animator.Update(0f);
            }

            retObj.SetActive(true);
            retObj.name = obj.Target.name;
            Destroy(tempObj);
            return retObj;
        }

        private void SETLEVEL(LevelEffects effects, int level, string prefab)
        {
            if (effects.m_levelSetups.Count >= level - 1)
            {
                LevelEffects.LevelSetup levelSetup = effects.m_levelSetups[level - 2];
                if (effects.m_mainRender)
                {
                    string key = prefab + level;
                    if (LevelEffects.m_materials.TryGetValue(key, out Material material))
                    {
                        Material[] sharedMaterials = effects.m_mainRender.sharedMaterials;
                        sharedMaterials[0] = material;
                        effects.m_mainRender.sharedMaterials = sharedMaterials;
                    }
                    else
                    {
                        Material[] sharedMaterials2 = effects.m_mainRender.sharedMaterials;
                        sharedMaterials2[0] = new Material(sharedMaterials2[0]);
                        sharedMaterials2[0].SetFloat(Hue, levelSetup.m_hue);
                        sharedMaterials2[0].SetFloat(Saturation, levelSetup.m_saturation);
                        sharedMaterials2[0].SetFloat(Value, levelSetup.m_value);
                        effects.m_mainRender.sharedMaterials = sharedMaterials2;
                        LevelEffects.m_materials[key] = sharedMaterials2[0];
                    }
                }

                if (effects.m_baseEnableObject)
                {
                    effects.m_baseEnableObject.SetActive(false);
                }

                if (levelSetup.m_enableObject)
                {
                    levelSetup.m_enableObject.SetActive(true);
                }
            }
        }

        private static readonly int Movement = Animator.StringToHash("Movement");

        public void MakeSprite(GameObject prefabArg, bool isGhost = false)
        {
            try
            {
                const float scale = 0.6f;
                const float offset = 0.22f;
                int hashcode = prefabArg.name.GetStableHashCode();
                if (CachedSprites.ContainsKey(hashcode)) return;
                rendererCamera.gameObject.SetActive(true);
                Light.gameObject.SetActive(true);
                RenderRequest request = new(prefabArg) { DistanceMultiplier = scale, offset = offset };
                GameObject spawn = SpawnAndRemoveComponents(request);
                spawn.transform.position = Vector3.zero;
                spawn.transform.rotation = request.Rotation;
                if (isGhost)
                {
                    SkinnedMeshRenderer[] AllMaterials = spawn.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer skinnedMeshRenderer in AllMaterials)
                    {
                        int Materials = skinnedMeshRenderer.materials.Length;
                        Material[] newMaterials = new Material[Materials];
                        for (int i = 0; i < Materials; i++)
                        {
                            newMaterials[i] = SoulMaterialTest;
                        }

                        skinnedMeshRenderer.materials = newMaterials;
                    }
                }

                Vector3 min = new Vector3(1000f, 1000f, 1000f);
                Vector3 max = new Vector3(-1000f, -1000f, -1000f);

                foreach (Renderer meshRenderer in spawn.GetComponentsInChildren<Renderer>(true))
                {
                    if (meshRenderer is ParticleSystemRenderer) continue;
                    min = Vector3.Min(min, meshRenderer.bounds.min);
                    max = Vector3.Max(max, meshRenderer.bounds.max);
                }

                spawn.transform.position = SpawnPoint - (min + max) / 2f;
                Vector3 size = new Vector3(Mathf.Abs(min.x) + Mathf.Abs(max.x), Mathf.Abs(min.y) + Mathf.Abs(max.y), Mathf.Abs(min.z) + Mathf.Abs(max.z));
                TimedDestruction timedDestruction = spawn.AddComponent<TimedDestruction>();
                timedDestruction.Trigger(1f);

                RenderObject go = new RenderObject(spawn, size)
                {
                    Request = request
                };
                RenderSprite(go, isGhost);
                ClearRendering();
            }
            catch (Exception ex)
            {
                ClearRendering();
                print(ex);
                CachedSprites[prefabArg.name.GetStableHashCode()] = PLACEHOLDERMONSTERICON;
            }
        }


        private void RenderSprite(RenderObject renderObject, bool isGhost)
        {
            int width = renderObject.Request.Width;
            int height = renderObject.Request.Height;

            RenderTexture oldRenderTexture = RenderTexture.active;
            RenderTexture temp = RenderTexture.GetTemporary(width, height, 32);
            rendererCamera.targetTexture = temp;
            rendererCamera.fieldOfView = renderObject.Request.FieldOfView;
            RenderTexture.active = rendererCamera.targetTexture;

            renderObject.Spawn.SetActive(true);
            float maxMeshSize = Mathf.Max(renderObject.Size.x, renderObject.Size.y) + 0.1f;
            float distance = maxMeshSize / Mathf.Tan(rendererCamera.fieldOfView * Mathf.Deg2Rad) * renderObject.Request.DistanceMultiplier;
            rendererCamera.transform.position = SpawnPoint + new Vector3(0, renderObject.Request.offset, distance);

            rendererCamera.Render();

            renderObject.Spawn.SetActive(false);
            Destroy(renderObject.Spawn);

            Texture2D previewImage = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewImage.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            previewImage.Apply();

            RenderTexture.active = oldRenderTexture;
            rendererCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(temp);
            rendererCamera.gameObject.SetActive(false);
            Light.gameObject.SetActive(false);
            Sprite newSprite = Sprite.Create(previewImage, new Rect(0, 0, width, height), Vector2.one / 2f);
            CachedSprites[renderObject.Spawn.name.GetStableHashCode()] = newSprite;
            /*string name = "Lagoshi_"+ renderObject.Spawn.name + (isGhost ? "_Soul" : "");
            SaveCachedSprite(name, newSprite.texture);*/
        }
    }
}