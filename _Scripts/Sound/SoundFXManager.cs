using UnityEngine;
public class SoundFXManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [Header("Player Sounds")]
    [Header("Walk")]
    [SerializeField] public AudioClip[] walkOnGrassEffects;
    [SerializeField] public AudioClip[] walkOnConcreteEffects;
    [Header("Dash")]
    [SerializeField] public AudioClip[] dashSoundEffects;
    [Space]
    [Header("Orb Sounds")]
    [SerializeField] public AudioClip[] orbThrowEffects;
    [SerializeField] public AudioClip[] orbCallEffects;
    [SerializeField] public AudioClip[] orbReturnEffects;
    public void PlaySoundFXAtPosition(AudioClip audioClip, Transform spawnTransform, float volume = 0.5f)
    {
        AudioSource tempAudioSource = Instantiate(audioSource, spawnTransform.position, Quaternion.identity);
        this.audioSource.clip = audioClip;
        this.audioSource.volume = volume;
        this.audioSource.Play();
        float clipLength = audioClip.length;
        Destroy(tempAudioSource.gameObject, clipLength);
    }
    public void PlayRandomSoundFXAtPosition(AudioClip[] audioClip, Transform spawnTransform, float volume = 0.5f)
    {
        AudioSource tempAudioSource = Instantiate(audioSource, spawnTransform.position, Quaternion.identity);

        int randomIndex = Random.Range(0, audioClip.Length);
        tempAudioSource.clip = audioClip[randomIndex];
        tempAudioSource.volume = volume;
        tempAudioSource.Play();
        float clipLength = audioClip[randomIndex].length;
        Destroy(tempAudioSource.gameObject, clipLength);
    }
}
