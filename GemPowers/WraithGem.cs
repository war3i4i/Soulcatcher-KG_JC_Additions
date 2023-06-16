namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private IEnumerator DelayInvokeSMR(Player p)
    {
        yield return new WaitForEndOfFrame();
        if (!p) yield break;
        p.m_visual.SetActive(false);
        p.m_animator.SetBool(Wakeup, false);
    }
    
    private static readonly int Wakeup = Animator.StringToHash("wakeup");
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    private static class PLAYERHIDE
    {
        private static void Postfix(Player __instance)
        {
            __instance.m_nview.Register("HideWraithSoulcatcher", (long _, bool tf) =>
            {
                __instance.m_visual.SetActive(tf);
                __instance.m_animator.SetBool(Wakeup, false);
            });

            if (__instance.m_nview?.m_zdo?.GetBool("SoulcatcherHide") == true)
                _thistype.StartCoroutine(_thistype.DelayInvokeSMR(__instance));
        }
    }
    
    [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
    private static class PriestCancelTP
    {
        private static void Postfix(Character __instance)
        {
            if (WraithJump && __instance == Player.m_localPlayer)
            {
                __instance.m_body.useGravity = false;
                __instance.m_body.velocity = Vector3.zero;
                __instance.m_currentVel = Vector3.zero;
                __instance.m_body.angularVelocity = Vector3.zero;
            }
        }
    }

    private static bool WraithJump;

    private IEnumerator WraithMovement(Vector3 startPos, Vector3 targetPos)
    {
        GameObject mainEffect = Instantiate(Wraith_Soul_Power.VFX, startPos, Player.m_localPlayer.transform.rotation);
        WraithJump = true;
        Player p = Player.m_localPlayer;
        Instantiate(Wraith_Soul_Power.VFX2, p.transform.position + Vector3.up, p.transform.rotation);
        p.m_nview.InvokeRPC(ZNetView.Everybody, "HideWraithSoulcatcher", false);
        p.m_nview.m_zdo.Set("SoulcatcherHide", true);
        p.m_zanim.SetTrigger("emote_stop");
        p.m_collider.isTrigger = true;
        float distance = Utils.DistanceXZ(startPos, targetPos);
        float time = distance * 0.03f;
        float count = 0;
        Player.m_localPlayer.transform.rotation = Quaternion.LookRotation((targetPos - startPos).normalized);
        while (count < 1f)
        {
            if (!p || p.IsDead())
            {
                ZNetScene.instance.Destroy(mainEffect);
                WraithJump = false;
                yield break;
            }

            p.m_body.velocity = Vector3.zero;
            p.m_body.angularVelocity = Vector3.zero;
            p.m_lastGroundTouch = 0;
            p.m_maxAirAltitude = 0f;
            count += Time.deltaTime / time;
            Vector3 point = startPos + (targetPos - startPos) / 2 + Vector3.up;
            Vector3 m1 = Vector3.Lerp(startPos, point, count);
            Vector3 m2 = Vector3.Lerp(point, targetPos, count);
            p.transform.position = Vector3.Lerp(m1, m2, count);
            mainEffect.transform.position = p.transform.position;
            yield return null;
        }

        ZNetScene.instance.Destroy(mainEffect);
        WraithJump = false;
        p.m_collider.isTrigger = false;
        p.m_nview.m_zdo.Set("SoulcatcherHide", false);
        p.m_body.velocity = Vector3.zero;
        p.m_body.useGravity = true;
        p.m_lastGroundTouch = 0f;
        p.m_maxAirAltitude = 0f;
        p.m_nview.InvokeRPC(ZNetView.Everybody, "HideWraithSoulcatcher", true);
        Instantiate(Wraith_Soul_Power.VFX2, p.transform.position + Vector3.up, p.transform.rotation);
    }


    public static class Wraith_Soul_Power
    {
        public static GameObject VFX;
        public static GameObject VFX2;
        public static int JumpMask;

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class Resources
        {
            static void Postfix(ZNetScene __instance)
            {
                VFX = asset.LoadAsset<GameObject>("Wraith_VFX");
                VFX2 = asset.LoadAsset<GameObject>("Wraith_VFX2");
                __instance.m_prefabs.Add(VFX);
                __instance.m_namedPrefabs.Add(VFX.name.GetStableHashCode(), VFX);
                __instance.m_prefabs.Add(VFX2);
                __instance.m_namedPrefabs.Add(VFX2.name.GetStableHashCode(), VFX2);
            }
        }

        public struct Config
        {
            [MinPower] public float Cooldown;
        }
        

        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.SetTrigger))]
        public static class ZSyncAnimation_SetTrigger_Patch
        {
            static bool Prefix(ZSyncAnimation __instance, string name)
            {
                if (!Player.m_localPlayer || __instance.m_animator != Player.m_localPlayer.m_animator) return true;
                if (name == "dodge")
                {
                    Player p = Player.m_localPlayer;
                    Config Effect = p.GetEffectPower<Config>("Wraith Soul Power");
                    if (Effect.Cooldown > 0 && !p.m_seman.GetStatusEffect(Name_Cooldown.GetStableHashCode()))
                    {
                        bool castHit = Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward,
                            out RaycastHit raycast,
                            80f, JumpMask);
                        if (castHit && raycast.collider)
                        {
                            Vector3 target = raycast.point;
                            if (Vector3.Distance(target, p.transform.position) > 40f)
                            {
                                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                                    $"Too far");
                            }
                            else
                            {
                                StatusEffect cooldown = p.m_seman.AddStatusEffect(Name_Cooldown.GetStableHashCode());
                                if (cooldown) cooldown.m_ttl = Effect.Cooldown;
                                _thistype.StartCoroutine(_thistype.WraithMovement(p.transform.position, target));
                            }
                        }

                        return false;
                    }
                }

                return true;
            }
        }
        
        
        private const string Name_Cooldown = "SE_Soulcatcher_Cooldown_Wraith";
        private const string Name_Cooldown_Localize = "$soulcatcher_wraith_gem $soulcatcher_cooldown";

        private static void AddSE(ObjectDB odb)
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
                ObjectDB.instance.GetItemPrefab("Amber") == null) return;
            if (!odb.m_StatusEffects.Find(se => se.name == Name_Cooldown))
            {
                SE_GenericInstantiate se = ScriptableObject.CreateInstance<SE_GenericInstantiate>();
                se.name = Name_Cooldown;
                se.m_icon = Gem.CooldownIcons["WraithGem"];
                se.m_name = Name_Cooldown_Localize;
                odb.m_StatusEffects.Add(se);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDBAwake
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDBCopyOtherDB
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddSE(__instance);
            }
        }
        
    }
}