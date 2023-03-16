namespace Soulcatcher_KG_JC_Additions;

public partial class Soulcatcher
{
    private static AudioSource AUsrc;
    private static AudioClip AdditionalCraftSound;


    [HarmonyPatch(typeof(AudioMan), nameof(AudioMan.Awake))]
    static class AudioMan_Awake_Patch
    {
        static void Postfix(AudioMan __instance) 
        {
            AUsrc = Chainloader.ManagerObject.AddComponent<AudioSource>();
            AUsrc.clip = asset.LoadAsset<AudioClip>("AltarClick");
            AdditionalCraftSound = asset.LoadAsset<AudioClip>("AltarStartSound");
            AUsrc.spatialBlend = 1;
            AUsrc.volume = 0.8f;
            AUsrc.maxDistance = 10f;
            AUsrc.outputAudioMixerGroup = __instance.m_masterMixer.outputAudioMixerGroup;
            Stopwatch watch = new();
            watch.Start();  
            foreach (GameObject allAsset in asset.LoadAllAssets<GameObject>())  
            {
                foreach (AudioSource audioSource in allAsset.GetComponentsInChildren<AudioSource>(true))
                {
                    audioSource.outputAudioMixerGroup = __instance.m_masterMixer.outputAudioMixerGroup;
                }
            }
 
            watch.Stop();
            //print($"Audio Manager took {watch.ElapsedMilliseconds} ms");
        }
    }
}