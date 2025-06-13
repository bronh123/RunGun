using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource playerAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayAudioClipAtPlayer(AudioClip clip)
    {
        if (playerAudioSource) playerAudioSource.PlayOneShot(clip);
    }
}
