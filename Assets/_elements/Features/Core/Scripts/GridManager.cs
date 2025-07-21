using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private  int _columns = 6;
    [SerializeField] private  int _rows = 10;
    [SerializeField] private  float _cellSize = 1;
    [SerializeField] private Vector2 _origin = Vector2.zero;
    
    private BlockController[,] _grid;
    
    
    private void Awake()
    {
        InitGrid();
        LoadLevel(1); //TODO
        NormalizeField();
    }
    
    
    public void LoadLevel(int levelIndex)
    {
        //TODO
    }
    
    public void NormalizeField()
    {
        //Lower the hanging blocks
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
        
        //Find & Remove matches
        var matches = MatchDetector.FindMatches(_grid, _columns, _rows);
        if (matches.Count > 0)
        {
            foreach (var b in matches)
            {
                b.Die();
                _grid[b.PositionOnGrid.x, b.PositionOnGrid.y] = null;
            }
            
            //TODO do save system
            NormalizeField();
        }
    }
    
    public void SwapCells(int x1, int y1, int x2, int y2)
    {
        var b1 = _grid[x1, y1];
        var b2 = _grid[x2, y2];
        if (b1 == null || b2 == null) return;
        _grid[x2, y2] = b1;
        _grid[x1, y1] = b2;
        b1.SetCoords(new Vector2Int(x2, y2));
        b2.SetCoords(new Vector2Int(x1, y2));
        NormalizeField();
    }
    
    
    private void InitGrid()
    {
        _grid = new BlockController[_columns, _rows];
    }
}