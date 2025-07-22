using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalloonMover : MonoBehaviour
{
    public Action OnDisappeared;
    
    private float _speed;
    private float _phase;
    
    
    private void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
        transform.position += Vector3.right * Mathf.Sin(Time.time + _phase) * 0.5f * Time.deltaTime;
        
        if(transform.position.y > Camera.main.orthographicSize + 1)
        {
            Destroy(gameObject); //We can make here a pool system but isn't it need?
            OnDisappeared?.Invoke();
            OnDisappeared = null;
        }
    }
    
    
    public void Init(float s)
    {
        _speed = s;
        _phase = Random.value * 2 * Mathf.PI;
    }
}
