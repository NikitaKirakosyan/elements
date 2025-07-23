using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Can be replaced to DI container like Zenject if need
// MonoBehaviour inheriting logic is necessary without DI
public class GridManager : MonoBehaviour
{
    public event Action OnLevelCompleted;
    
    [SerializeField] private float _cellSize = 1;
    [SerializeField] private Vector2 _origin = Vector2.zero;
    
    private BlockController[,] _grid;
    private int _columns;
    private int _rows;
    private int _currentLevelIndex;
    
    public int Columns => _columns;
    public int Rows => _rows;
    public float CellSize => _cellSize;
    public Vector2 Origin => _origin;
    
    
    private void Awake()
    {
        Play();
    }
    
    
    public void Play()
    {
        _currentLevelIndex = SaveManager.GetLastCompletedLevel();
        
        if(SaveManager.TryLoad(out var lastLevelIndex, out var types))
        {
            if(_currentLevelIndex > lastLevelIndex)
                LoadLevel(_currentLevelIndex);
            else
                LoadLevel(lastLevelIndex, types);
        }
        else
        {
            LoadLevel(0);
        }
        
        StartCoroutine(NormalizeFieldRoutine());
    }
    
    public void SwapCells(int x1, int y1, int x2, int y2)
    {
        var b1 = _grid[x1, y1];
        var b2 = _grid[x2, y2];
        if (b1 == null || b2 == null)
            return;
        
        _grid[x2, y2] = b1;
        _grid[x1, y1] = b2;
        b1.MoveToCell(new Vector2Int(x2, y2));
        b2.MoveToCell(new Vector2Int(x1, y1));
        NormalizeFieldRoutine();
    }
    
    
    private void LoadLevel(int levelIndex, BlockType[,] types = null)
    {
        var levelsCount = LevelData.GetLevelsCount();
        levelIndex = Mathf.Clamp(levelIndex, 0, levelsCount - 1);
        
        var levelData = LevelData.Load(levelIndex);
        
        _columns = levelData.Columns;
        _rows = levelData.Rows;
        _grid = new BlockController[_columns, _rows];
        
        for (var x = 0; x < _columns; x++)
        {
            for (var y = 0; y < _rows; y++)
            {
                var type = types != null ? types[x,y] : levelData.GetTypeAt(x, y);
                if(type == BlockType.None)
                {
                    _grid[x, y] = null;
                    continue;
                }
                
                var pos = new Vector3(_origin.x + x * _cellSize, _origin.y + y * _cellSize, 0);
                var blockPrefab = levelData.GetBlockByType(type);
                var block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                block.Init(new Vector2Int(x, y), this);
                _grid[x, y] = block;
            }
        }
    }
    
    private IEnumerator NormalizeFieldRoutine()
    {
        var moveTweens = new List<Tween>();
        
        // Lower down blocks
        for(var x = 0; x < _columns; x++)
        {
            for(var y = 0; y < _rows; y++)
            {
                if(_grid[x, y] == null)
                {
                    for(var k = y + 1; k < _rows; k++)
                    {
                        if(_grid[x, k] != null)
                        {
                            var block = _grid[x, k];
                            _grid[x, y] = block;
                            _grid[x, k] = null;
                            var moveTween = block.MoveToCell(new Vector2Int(x, y));
                            moveTweens.Add(moveTween.OnComplete(() => moveTweens.Remove(moveTween)));
                            break;
                        }
                    }
                }
            }
        }
        
        yield return new WaitWhile(() => moveTweens.Count > 0);
        
        // Find and remove matches
        var matches = MatchDetector.FindMatches(_grid, _columns, _rows);
        if(matches.Count > 0)
        {
            var destroyTweens = new List<Tween>();
            foreach(var matchedBlock in matches)
            {
                _grid[matchedBlock.PositionOnGrid.x, matchedBlock.PositionOnGrid.y] = null;
                var destroyTween = matchedBlock.Die();
                destroyTweens.Add(destroyTween.OnComplete(() => destroyTweens.Remove(destroyTween)));
            }
            
            SaveManager.SaveLastCompletedLevel(_currentLevelIndex);
            SaveManager.SaveState(_currentLevelIndex, _grid);
            
            yield return new WaitWhile(() => destroyTweens.Count > 0);
            yield return NormalizeFieldRoutine();
            yield break;
        }
        
        var anyLeft = false;
        for(var x = 0; x < _columns && !anyLeft; x++)
        for(var y = 0; y < _rows; y++)
            if(_grid[x, y] != null)
            {
                anyLeft = true;
                break;
            }
        
        if(!anyLeft)
        {
            SaveManager.SaveLastCompletedLevel(_currentLevelIndex + 1);
            OnLevelCompleted?.Invoke();
            Play();
        }
    }
}