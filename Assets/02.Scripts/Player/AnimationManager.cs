using UnityEngine;
using System.Collections.Generic;
public class AnimationManager : MonoBehaviour
{
    Animator _animator;
    Animator animator
    {
        set 
        { 
            if(_animator == null)
                _animator = GetComponentInChildren<Animator>();
            _animator = value;
        }
        get
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            return _animator;
        }
    }
    bool isOnPlay;
    [HideInInspector] public float animationElapsedTime;
    public float speed
    {
        set { _animator.speed = value; }
        get { return animator.speed; }
    }
    Dictionary<string, AnimationElements> animationElementsDictionary = new Dictionary<string, AnimationElements>();
    string currentAnimationName;
    private void Awake()
    {
        animator.speed = 1;
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
    public float GetAnimationTime(string name)
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