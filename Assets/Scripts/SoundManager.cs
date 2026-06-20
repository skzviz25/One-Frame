using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    
    [Header("Audio Clips")]
    public AudioClip musicClip;
    public AudioClip stepSoundClip;
    public AudioClip hitSoundClip;
    public AudioClip explosionSoundClip;
    public AudioClip starCollectSoundClip;
    public AudioClip enemyDisappear;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    public void PlayStepSound()
    {
        if (sfxSource != null && stepSoundClip != null)
            sfxSource.PlayOneShot(stepSoundClip);
    }

    public void PlayStarCollectSound()
    {
        if (sfxSource != null && starCollectSoundClip != null)
            sfxSource.PlayOneShot(starCollectSoundClip);
    }

    public void PlayExplosionSound()
    {
        if (sfxSource != null && explosionSoundClip != null)
            sfxSource.PlayOneShot(explosionSoundClip);
    }

    public void PlayHitSound()
    {
        if (sfxSource != null && hitSoundClip != null)
            sfxSource.PlayOneShot(hitSoundClip);
    }

    public void PlayEnemyDisappearSound()
    {
        if (sfxSource != null && enemyDisappear != null)
            sfxSource.PlayOneShot(enemyDisappear);
    }
}