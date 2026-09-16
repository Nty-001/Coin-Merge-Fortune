using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class PackagedAudio : MonoBehaviour
    {
        public AudioSource music,effects;
        public string[] paths;
        AudioClip[] clips;
        AudioClip Clip(int index)
        {if(clips==null)clips=new AudioClip[paths.Length];if(!clips[index])clips[index]=Resources.Load<AudioClip>(paths[index]);return clips[index];}
        public void SetMusic(bool enabled)
        {if(!enabled){music.Stop();return;}if(music.isPlaying)return;music.clip=Clip(0);music.Play();}
        public void Effect(int index,bool enabled){if(enabled){var clip=Clip(index);if(clip)effects.PlayOneShot(clip);}}
    }
}
