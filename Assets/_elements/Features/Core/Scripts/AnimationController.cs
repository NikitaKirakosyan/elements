using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    
    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }
    
    
    public void PlayIdle()
    {
        _animator.Play("Idle");
    }
    
    public void PlayDestroy(Action onComplete)
    {
        _animator.Play("Destroy");
        Invoke(nameof(Finish), _animator.GetCurrentAnimatorStateInfo(0).length);
        return;
        
        void Finish() => onComplete?.Invoke();
    }
}