using TMPro;
using Random = UnityEngine.Random;

namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static void FloatingText(Color c, string text) 
    {
        float random = Random.Range(1.2f, 1.6f);
        float random2 = Random.Range(1.2f, 1.6f);
        
        DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance
        {
            m_worldPos = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.right * random + Vector3.up * random2,
            m_gui = Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform)
        };
        worldTextInstance.m_gui.GetComponent<RectTransform>().sizeDelta *= 2;
        worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<TMP_Text>();
        DamageText.instance.m_worldTexts.Add(worldTextInstance);
        worldTextInstance.m_textField.color = c; 
        worldTextInstance.m_textField.fontSize = 20;
        worldTextInstance.m_textField.text = text; 
        worldTextInstance.m_timer = -1f;
    }
 
     
    public static class Deathsquito_Soul_Power
    {
        public struct Config
        {
            [MaxPower] public float Value;
        }
        
        
        [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
        static class Character_Damage_Patch
        {
            static void Prefix(ref HitData hit)
            {
                if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return; 
                Config Effect = Player.m_localPlayer.GetEffectPower<Config>("Deathsquito Soul Power");
                if (Effect.Value > 0)
                {
                    int random = Random.Range(0, 101);
                    if (random <= Effect.Value)
                    {
                        hit.ApplyModifier(2f);
                        FloatingText(Color.red, "CRIT");
                    }
                }
            }
        }
        
    }
}