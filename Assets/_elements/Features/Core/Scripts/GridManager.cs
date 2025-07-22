using UnityEngine;

// Can be replaced to DI container like Zenject if need
// MonoBehaviour inheriting logic is necessary without DI
public class GridManager : MonoBehaviour
{
    [SerializeField] private float _cellSize = 1;
    [SerializeField] private Vector2 _origin = Vector2.zero;
    
    private BlockController[,] _grid;
    private int _columns;
    private int _rows;
    
    public int Columns => _columns;
    public int Rows => _rows;
    public float CellSize => _cellSize;
    public Vector2 Origin => _origin;
    
    
    private void Awake()
    {
        LoadLevel(1); //TODO
        NormalizeField();
    }
    
    public void LoadLevel(int levelIndex)
    {
        var levelData = LevelData.Load(levelIndex);
        
        _columns = levelData.Columns;
        _rows = levelData.Rows;
        _grid = new BlockController[_columns, _rows];
        
        for (var x = 0; x < _columns; x++)
        {
            for (var y = 0; y < _rows; y++)
            {
                var type = levelData.GetTypeAt(x, y);
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
    
    public void NormalizeField()
    {
        // Lower down blocks
        for (var x = 0; x < _columns; x++)
        {
            for (var y = 0; y < _rows; y++)
            {
                if (_grid[x, y] == null)
                {
                    for (var k = y + 1; k < _rows; k++)
                    {
                        if (_grid[x, k] != null)
                        {
                            _grid[x, k].MoveToCell(new Vector2Int(x, y));
                            _grid[x, y] = _grid[x, k];
                            _grid[x, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        
        // Find and remove matches
        var matches = MatchDetector.FindMatches(_grid, _columns, _rows);
        if (matches.Count > 0)
        {
            foreach (var b in matches)
            {
                b.Die();
                _grid[b.PositionOnGrid.x, b.PositionOnGrid.y] = null;
            }
            
            //TODO add save system
            NormalizeField();
        }
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
        NormalizeField();
    }
}