using UnityEngine;
using UnityEngine.Audio;
namespace com.game
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        public const string Master_Volume = "masterVolume";
        public const string Music_Volume = "musicVolume";
        public const string Sound_FX_Volume = "soundFXVolume";

        void Start()
        {
            if (audioMixer == null)
                audioMixer = FindFirstObjectByType<AudioMixer>();
        }
        public void SetMasterVolume(float volumeLevel)
        {
            audioMixer.SetFloat(Master_Volume, VolumeToDecibel(volumeLevel));
        }
        public void SetMusicVolume(float volumeLevel)
        {
            audioMixer.SetFloat(Music_Volume, VolumeToDecibel(volumeLevel));
        }
        public void SetSoundFXVolume(float volumeLevel)
        {
            audioMixer.SetFloat(Sound_FX_Volume, VolumeToDecibel(volumeLevel));
        }

        private float VolumeToDecibel(float volume)
        {
            return Mathf.Log10(volume) * 20;
        }
        
    }
}
