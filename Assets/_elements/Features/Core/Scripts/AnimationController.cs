using System;
using DG.Tweening;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private Action onDestroyAnimationCompleteCallback;
    
    
    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void Awake()
    {
        PlayIdle();
    }
    
    
    public void PlayIdle()
    {
        _animator.Play("Idle");
    }
    
    public Tween PlayDestroy(Action onComplete)
    {
        onDestroyAnimationCompleteCallback = onComplete;
        _animator.Play("Destroy");
        return DOVirtual.DelayedCall(_animator.GetCurrentAnimatorStateInfo(0).length, OnDestroyAnimationFinished);
    }
    
    
    private void OnDestroyAnimationFinished()
    {
        onDestroyAnimationCompleteCallback?.Invoke();
    }
}