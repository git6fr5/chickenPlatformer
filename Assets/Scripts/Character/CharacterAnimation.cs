using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{

    //private bool DEBUG_clips = false;
    //private bool DEBUG_sound = false;

    public AnimationClip idleAnim;
    public AnimationClip runningAnim;
    public AnimationClip jumpingAnim;
    public AnimationClip crouchingAnim;
    public AnimationClip hurtAnim;
    public AnimationClip deathAnim;

    public Animator animator;
    private RuntimeAnimatorController animatorController;
    private List<AnimationClip> animationClips = new List<AnimationClip>();
    private AnimationClip[] controllerClips;

    [HideInInspector] public float x_speed = 0f;
    [HideInInspector] public bool inAir = false;
    [HideInInspector] public bool crouch = false;
    [HideInInspector] public bool hurt = false;
    [HideInInspector] public bool death = false;

    private bool overriding = false;
    private string prevAnim;
    public float hurtDuration = 0.2f;
    public float deathDuration = 0.4f;

    private float duration;
    private float elapsedDuration = 0f;

    /*--- Audio ---*/

    public AudioClip hurtAudio;
    public AudioClip deathAudio;

    public AudioSource audioSource;


    void Start()
    {
        /*animatorController = animator.runtimeAnimatorController;
        controllerClips = animatorController.animationClips; // for checking that these 2 are aligned
        animationClips.AddRange(new List<AnimationClip> { idleAnim, runningAnim, jumpingAnim, crouchingAnim, hurtAnim } );

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
            //print("sound is being played");
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

    public void SetAnimation()
    {
        bool animated = false;

        /*--- High Priority ---*/
        if (death && deathAnim)
        {
            animator.Play(deathAnim.name);
            /*overriding = true; prev_anim = "bob_hurt_anim";*/
            overriding = true; duration = deathDuration;
            animated = true;
        }
        else if (hurt && hurtAnim)
        {
            animator.Play(hurtAnim.name);
            /*overriding = true; prev_anim = "bob_hurt_anim";*/
            overriding = true; duration = hurtDuration;
            animated = true;
        }


        if (animated) { return; }

        /*--- Mid Priority ---*/
        if (crouch && crouchingAnim)
        {
            animator.Play(crouchingAnim.name);
            animated = true;
        }
        else if (inAir && jumpingAnim)
        {
            animator.Play(jumpingAnim.name);
            animated = true;
        }
        else if (x_speed > 0 && runningAnim)
        {
            animator.Play(runningAnim.name);
            animated = true;
        }

        if (animated) { return; }

        /*--- Low Priority ---*/

        animator.Play(idleAnim.name);
        return;
    }

    public void OverrideAnimation()
    {
        string currAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //curr_anim = animator.GetCurrentAnimatorStateInfo();
        if (currAnim != prevAnim)
        {
            overriding = false;
        }
        print(prevAnim + ", " + currAnim);
        prevAnim = currAnim;
    }

    public void OverrideAnimationForDuration(float elapsedDuration, float duration)
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
        //print("attempting to play sound");
        if (death && deathAudio)
        {
            audioSource.clip = deathAudio;
            audioSource.Play();
            sounded = true;
        }
        else if (hurt && hurtAudio)
        {
            audioSource.clip = hurtAudio;
            audioSource.Play();
            sounded = true;
        }
        if (sounded) { return; }

        /*--- Low Priority ---*/
        return;
    }

    private void DisableHighPrio()
    {
        hurt = false;
        death = false;
    }
}
