using System;
using System.Linq;
using UnityEngine;


public class EventfulAnimator : MonoBehaviour
{
    // Actions
    public event Action<AnimationClip /* Clip */> OnAnimationStarted;
    public event Action<AnimationClip /* Clip */> OnAnimationEnded;


    // Public Properties
    public RuntimeAnimatorController runtimeAnimatorController;
    public AnimationClip currentAnimation { private set; get; } = null;
    public Animator animatorComponent;


    // Public Methods
    public void Play(AnimationClip clip)
    {
        // Return if animator is missing
        if (animatorComponent == null)
        {
            Debug.LogWarning("[EventfulAnimator] Play() failed, Animator component is null!");
            return;
        }


        // Return if clip is null
        if (clip == null)
        {
            Debug.LogWarning("[EventfulAnimator] Play() failed, Clip is null!");
            return;
        }


        // Retrun if same clip
        if (clip == currentAnimation)
        {
            return;
        }


        // Play animation
        animatorComponent.enabled = true;
        animatorComponent.Play(clip.name, 0);
    }
    public void Stop()
    {
        // Return if animator is missing
        if (animatorComponent == null)
        {
            Debug.LogWarning("[EventfulAnimator] Stop() failed, Animator component is null!");
            return;
        }


        // Return if no clip is playing
        if (currentAnimation == null)
        {
            return;
        }


        // Stop playing animation
        // animatorComponent.Play(currentAnimation.name, 0f);
        animatorComponent.enabled = false;
        currentAnimation = null;  // reset so same clip can replay
    }
    public void Resume()
    {
        // Return if animator is missing
        if (animatorComponent == null)
        {
            Debug.LogWarning("[EventfulAnimator] Resume() failed, Animator component is null!");
            return;
        }

        animatorComponent.enabled = true;
    }
    public bool IsPlaying()
    {
        return currentAnimation != null;
    }
    public bool IsPlaying(AnimationClip clip)
    {
        return clip != null && currentAnimation == clip;
    }


    // Private Methods
    private void OnClipStarted(AnimationEvent animationEvent)
    {
        // Return if clip reference is unresolved
        AnimationClip clip = animationEvent.objectReferenceParameter as AnimationClip;
        if (clip == null)
        {
            Debug.LogWarning("[EventfulAnimator] OnClipStarted() failed, Clip reference is null!");
            return;
        }


        currentAnimation = clip;
        OnAnimationStarted?.Invoke(clip);
    }
    private void OnClipEnded(AnimationEvent animationEvent)
    {
        // Return if clip reference is unresolved
        AnimationClip clip = animationEvent.objectReferenceParameter as AnimationClip;
        if (clip == null)
        {
            Debug.LogWarning("[EventfulAnimator] OnClipEnded() failed, Clip reference is null!");
            return;
        }


        currentAnimation = null;
        OnAnimationEnded?.Invoke(clip);
    }



    // Override Methods
    void Awake()
    {
        // Setup animator component
        if (animatorComponent == null)
        {
            animatorComponent = GetComponent<Animator>() ?? gameObject.AddComponent<Animator>();
        }


        // Iterate and prepare each clip
        foreach (var clip in runtimeAnimatorController.animationClips)
        {
            // Remove only this scripts previously added events, keep custom ones
            string startedName = nameof(OnClipStarted);
            string endedName = nameof(OnClipEnded);
            clip.events = clip.events.Where(e => e.functionName != startedName && e.functionName != endedName).ToArray();


            // Add start & end events
            clip.AddEvent(new AnimationEvent { functionName = nameof(OnClipStarted), objectReferenceParameter = clip, time = 0f });
            clip.AddEvent(new AnimationEvent { functionName = nameof(OnClipEnded), objectReferenceParameter = clip, time = clip.length });
        }
    }
}
