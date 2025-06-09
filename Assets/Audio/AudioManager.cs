using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    [Header("Audio Clip")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip chest;
    public AudioClip wall;

    public void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PLaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip); 
    }

}
