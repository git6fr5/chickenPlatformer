using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAnimation : MonoBehaviour
{

    //private bool DEBUG_clips = true;
    //private bool DEBUG_audio = true;

    /* --- Animation ---*/

    public AnimationClip idleAnim;
    public AnimationClip activatingAnim;
    public AnimationClip activeAnim;

    public Animator animator;
    private RuntimeAnimatorController animatorController;
    private List<AnimationClip> animationClips = new List<AnimationClip>();
    private AnimationClip[] controllerClips;

    public bool active;
    public bool activating;

    private bool overriding = false;
    public float activatingDuration = 0.33f;

    private float duration;
    private float elapsedDuration = 0f;

    /*--- Audio ---*/

    public AudioClip activatingAudio;

    public AudioSource audioSource;


    void Start()
    {
        /*animatorController = animator.runtimeAnimatorController;
        controllerClips = animatorController.animationClips; // for checking that these 2 are aligned
        animationClips.AddRange(new List<AnimationClip> { idleAnim, activatingAnim, activeAnim });

        if (DEBUG_clips)
        {
            foreach (AnimationClip animationClip in controllerClips)
            {
                print(animationClip.name);
            }
        }*/
    }

    void Update()
    {
        if (overriding)
        {
            //OverrideAnimation();
        }
        else
        {
            SetAnimation();
            elapsedDuration = 0;
        }

        if (!audioSource.isPlaying)
        {
            PlaySound();
        }
        else
        {
            print("sound is being played");
        }
        DisableHighPrio();
    }

    void FixedUpdate()
    {
        if (overriding)
        {
            elapsedDuration = elapsedDuration + Time.fixedDeltaTime;
            OverrideAnimationForDuration(elapsedDuration, duration);
        }
    }

    private void SetAnimation()
    {
        bool animated = false;

        /*--- High Priority ---*/
        if (activating && activatingAnim)
        {
            animator.Play(activatingAnim.name);
            /*overriding = true; prev_anim = "bob_hurt_anim";*/
            overriding = true; duration = activatingDuration;
            animated = true;
        }
        if (animated) { return; }

        /*--- Mid Priority ---*/
        if (active && activeAnim)
        {
            animator.Play(activeAnim.name);
            animated = true;
        }
        if (animated) { return; }

        /*--- Low Priority ---*/

        animator.Play(idleAnim.name);
        return;
    }

    private void OverrideAnimationForDuration(float elapsedDuration, float duration)
    {
        if (elapsedDuration > duration)
        {
            overriding = false;
        }
    }

    private void PlaySound()
    {
        bool sounded = false;

        /*--- High Priority ---*/
        print("attempting to play sound");
        if (activating && activatingAudio)
        {
            audioSource.clip = activatingAudio;
            audioSource.Play();
            sounded = true;
        }
        if (sounded) { return; }

        /*--- Low Priority ---*/
        return;
    }

    private void DisableHighPrio()
    {
        activating = false;
    }
}