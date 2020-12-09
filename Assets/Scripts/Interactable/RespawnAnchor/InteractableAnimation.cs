using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAnimation : MonoBehaviour
{

    private bool DEBUG_clips = true;

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


    void Start()
    {
        animatorController = animator.runtimeAnimatorController;
        controllerClips = animatorController.animationClips; // for checking that these 2 are aligned
        animationClips.AddRange(new List<AnimationClip> { idleAnim, activatingAnim, activeAnim });

        if (DEBUG_clips)
        {
            foreach (AnimationClip animationClip in controllerClips)
            {
                print(animationClip.name);
            }
        }
    }

    void FixedUpdate()
    {
        if (overriding)
        {
            //OverrideAnimation();
            elapsedDuration = elapsedDuration + Time.fixedDeltaTime;
            OverrideAnimationForDuration(elapsedDuration, duration);
        }
        else
        {
            SetAnimation();
            elapsedDuration = 0;
        }
    }

    public void SetAnimation()
    {
        bool animated = false;

        /*--- High Priority ---*/
        if (activating && activatingAnim)
        {
            animator.Play(activatingAnim.name);
            /*overriding = true; prev_anim = "bob_hurt_anim";*/
            overriding = true; duration = activatingDuration;
            animated = true;
            activating = false;
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

    public void OverrideAnimationForDuration(float elapsedDuration, float duration)
    {
        if (elapsedDuration > duration)
        {
            overriding = false;
        }
    }
}