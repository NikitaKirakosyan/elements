using System;
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
    
    public void PlayDestroy(Action onComplete)
    {
        onDestroyAnimationCompleteCallback = onComplete;
        _animator.Play("Destroy");
        Invoke(nameof(OnDestroyAnimationFinished), _animator.GetCurrentAnimatorStateInfo(0).length);
    }
    
    
    private void OnDestroyAnimationFinished()
    {
        onDestroyAnimationCompleteCallback?.Invoke();
    }
}