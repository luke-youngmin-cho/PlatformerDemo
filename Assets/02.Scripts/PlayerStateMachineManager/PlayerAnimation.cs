using UnityEngine;
using System.Collections.Generic;
class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    bool isOnPlay;
    [HideInInspector] public float animationElapsedTime;

    Dictionary<string,AnimationElements> animationElementsDictionary = new Dictionary<string,AnimationElements>();
    string currentAnimationName;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isOnPlay &&
            animationElementsDictionary[currentAnimationName].isLooping == false)
        {
            if (animationElapsedTime > animationElementsDictionary[currentAnimationName].time)
                isOnPlay = false;
            animationElapsedTime += Time.deltaTime;
        }
    }
    // Register method for state machines
    public void RegisterAnimationRequired(string animationName)
    {
        animationElementsDictionary.Add(animationName, new AnimationElements
        {
            time = GetAnimationTime(name),
            isLooping = GetAnimationIsLooping(name)
        });
    }
    public void ChangeAnimationState(string newAnimationName)
    {   
        if (currentAnimationName == newAnimationName) return;

        animator.Play(newAnimationName);
        currentAnimationName = newAnimationName;
        animationElapsedTime = 0f;
    }
    float GetAnimationTime(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }
    bool GetAnimationIsLooping(string name)
    {
        bool isLooping = false;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                isLooping = ac.animationClips[i].isLooping;
            }
        }
        return isLooping;
    }
}
public struct AnimationElements
{
    public float time;
    public bool isLooping;
}