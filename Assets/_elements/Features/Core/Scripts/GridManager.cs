using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Can be replaced to DI container like Zenject if need
// MonoBehaviour inheriting logic is necessary without DI
public class GridManager : MonoBehaviour
{
    [SerializeField] private float _cellSize = 1;
    [SerializeField] private float _marginLeft   = 1;
    [SerializeField] private float _marginRight  = 1;
    [SerializeField] private float _marginTop;
    [SerializeField] private float _marginBottom = 3.5f;
    
    private Vector2 _origin;
    private BlockController[,] _grid;
    private int _columns;
    private int _rows;
    private int _currentLevelIndex;
    
    public int Columns => _columns;
    public int Rows => _rows;
    public float CellSize => _cellSize;
    public Vector2 Origin => _origin;
    public int CurrentLevelIndex => _currentLevelIndex;
    
    
    private void Awake()
    {
        if(SaveManager.TryLoad(out var lastLevelIndex, out var types))
        {
            if(_currentLevelIndex > lastLevelIndex)
                LoadLevel(_currentLevelIndex);
            else
                LoadLevel(lastLevelIndex, types);
        }
        else
        {
            Play(0);
        }
        
        PlayLastStartedLevel();
    }
    
    
    public void Play(int levelIndex)
    {
        StopAllCoroutines();
        
        foreach(Transform t in transform)
            Destroy(t.gameObject);
        
        LoadLevel(levelIndex);
        StartCoroutine(NormalizeFieldRoutine());
    }
    
    public void Restart()
    {
        SaveManager.ClearSaveState();
        PlayLastStartedLevel();
    }
    
    public void SwapCells(int x1, int y1, int x2, int y2)
    {
        var b1 = _grid[x1, y1];
        if(b1 == null)
            return;
        
        var b2 = _grid[x2, y2];
        
        _grid[x2, y2] = b1;
        _grid[x1, y1] = b2;
        
        b1.MoveToCell(new Vector2Int(x2, y2));
        if (b2 != null)
            b2.MoveToCell(new Vector2Int(x1, y1));
        
        StartCoroutine(NormalizeFieldRoutine());
    }
    
    
    private void FitGrid()
    {
        var cam = Camera.main;
        var worldH = cam.orthographicSize * 2f;
        var worldW = worldH * cam.aspect;
        
        var gridW = _columns * _cellSize;
        var gridH = _rows * _cellSize;
        
        var availW = worldW - _marginLeft - _marginRight;
        var availH = worldH - _marginTop - _marginBottom;
        var scale  = Mathf.Min(availW / gridW, availH / gridH);
        transform.localScale = Vector3.one * scale;
        
        var camBottom = cam.transform.position.y - cam.orthographicSize;
        
        var yPos = camBottom + _marginBottom + (gridH * scale) * 0.5f;
        var xPos = cam.transform.position.x;
        
        transform.position = new Vector3(xPos, yPos, transform.position.z);
        
        _origin = new Vector2(-gridW * 0.5f + _cellSize * 0.5f, -gridH * 0.5f + _cellSize * 0.5f);
    }
    
    private void PlayLastStartedLevel()
    {
        _currentLevelIndex = SaveManager.GetLastStartedLevel();
        Play(_currentLevelIndex);
    }
    
    private void LoadLevel(int levelIndex, BlockType[,] types = null)
    {
        var levelsCount = LevelData.GetLevelsCount();
        if(levelIndex >= levelsCount - 1)
            levelIndex = 0;
        
        _currentLevelIndex = levelIndex;
        SaveManager.SaveLastStartedLevel(_currentLevelIndex);
        
        var levelData = LevelData.Load(_currentLevelIndex);
        
        _columns = levelData.Columns;
        _rows = levelData.Rows;
        _grid = new BlockController[_columns, _rows];
        
        FitGrid();
        
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
                
                var blockPrefab = levelData.GetBlockByType(type);
                var block = Instantiate(blockPrefab);
                block.transform.SetParent(transform, false);
                block.transform.localPosition = new Vector3(
                               _origin.x + x * _cellSize,
                               _origin.y + y * _cellSize,
                               0f);
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
            
            yield return new WaitWhile(() => destroyTweens.Count > 0);
            yield return NormalizeFieldRoutine();
            yield break;
        }
        
        SaveManager.SaveState(_currentLevelIndex, _grid);
        
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
            _currentLevelIndex++;
            SaveManager.SaveLastStartedLevel(_currentLevelIndex);
            
            Play(_currentLevelIndex);
        }
    }
}