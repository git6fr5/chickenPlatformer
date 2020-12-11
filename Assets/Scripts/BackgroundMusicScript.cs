using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicScript : MonoBehaviour
{

    /*private float duration;
    private float elapsedDuration = 0f;*/

    public int tempo = 120;
    [Range(0, 1)] public float volume = 1f;

    public bool intro = true;
    public bool intense = false;

    /*--- Audio ---*/

    public AudioClip mainMusic;
    public AudioClip introMusic;
    public AudioClip intenseMusic;

    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = volume;
        PlaySound();
        intro = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlaySound();
        }
        else
        {

        }
    }

    private void PlaySound()
    {
        bool sounded = false;

        /*--- High Priority ---*/
        if (intense && intenseMusic)
        {
            audioSource.clip = intenseMusic;
            audioSource.Play();
            sounded = true;
        }
        else if (intro && introMusic)
        {
            audioSource.clip = introMusic;
            audioSource.Play();
            sounded = true;
        }
        
        if (sounded) { return; }

        /*--- Low Priority ---*/

        audioSource.clip = mainMusic;
        audioSource.Play();
        return;
    }

}
