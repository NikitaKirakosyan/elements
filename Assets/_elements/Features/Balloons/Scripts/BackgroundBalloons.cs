using UnityEngine;

public class BackgroundBalloons : MonoBehaviour
{
    [SerializeField] private int _maxBubbles = 3;
    [SerializeField] private float _delayBetweenSpawn = 0.1f;
    [SerializeField] private float _speedMin = 0.5f;
    [SerializeField] private float _speedMax = 1.5f;
    [SerializeField] private BalloonMover[] _balloonPrefabs;
    [SerializeField] private Vector2 _spawnMin;
    [SerializeField] private Vector2 _spawnMax;
    
    private int _count;
    private float _delayTimer;
    
    
    private void Update()
    {
        if(_count >= _maxBubbles)
            return;
        
        if(_delayTimer <= 0)
        {
            SpawnBubble();
            _delayTimer = _delayBetweenSpawn;
            return;
        }
        
        _delayTimer -= Time.deltaTime;
    }
    
    
    private void SpawnBubble()
    {
        var x = Random.Range(_spawnMin.x, _spawnMax.x);
        var y = Random.Range(_spawnMin.y, _spawnMax.y);
        
        var bubbleMover = Instantiate(_balloonPrefabs.GetRandomElement(), new Vector3(x, y, 1), Quaternion.identity, transform);
        bubbleMover.Init(Random.Range(_speedMin, _speedMax));
        bubbleMover.OnDisappeared += OnBalloonDisappeared;
        
        _count++;
        return;
        
        void OnBalloonDisappeared()
        {
            bubbleMover.OnDisappeared -= OnBalloonDisappeared;
            _count--;
        }
    }
}