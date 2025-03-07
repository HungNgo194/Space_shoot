using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource m_Source;
    [SerializeField] AudioSource SFXSource;
    public AudioClip shoot;
    public AudioClip hit;
    public AudioClip explode;
    public AudioClip collect;

    private void Start()
    {
        
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
