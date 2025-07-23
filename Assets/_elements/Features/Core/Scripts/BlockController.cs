using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private BlockType _type;
    [SerializeField] private float _minSwipe = 0.5f;
    [SerializeField] private float _moveDuration = 0.2f;
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private Vector2 _touchStart;
    private GridManager _grid;
    
    public BlockType Type => _type;
    public Vector2Int PositionOnGrid { get; private set; }
    
    
    private void Reset()
    {
        _animationController = GetComponent<AnimationController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    
    void IPointerDownHandler.OnPointerDown(PointerEventData data)
    {
        _touchStart = Input.mousePosition;
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData data)
    {
        //Do swipe logic
        var diff = data.position - _touchStart;
        if (diff.magnitude < _minSwipe * Screen.dpi)
            return;
        
        diff.Normalize();
        
        var dx = 0;
        var dy = 0;
        
        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            dx = diff.x > 0 ? 1 : -1;
        else
            dy = diff.y > 0 ? 1 : -1;
        
        var nx = PositionOnGrid.x + dx;
        var ny = PositionOnGrid.y + dy;
        
        if(nx >= 0 && nx < _grid.Columns && ny >= 0 && ny < _grid.Rows)
            _grid.SwapCells(PositionOnGrid.x, PositionOnGrid.y, nx, ny);
    }
    
    
    public void Init(Vector2Int positionOnGrid, GridManager grid)
    {
        PositionOnGrid = positionOnGrid;
        _grid = grid;
        UpdateSorting();
    }
    
    public Tween MoveToCell(Vector2Int positionOnGrid)
    {
        PositionOnGrid = positionOnGrid;
        UpdateSorting();
        
        var targetLocal = new Vector3(_grid.Origin.x + PositionOnGrid.x * _grid.CellSize, _grid.Origin.y + PositionOnGrid.y * _grid.CellSize, 0f);
        return transform.DOLocalMove(targetLocal, _moveDuration);
    }
    
    public Tween Die()
    {
        return _animationController.PlayDestroy(() => Destroy(gameObject));
    }
    
    
    private void UpdateSorting()
    {
        _spriteRenderer.sortingOrder = (_grid.Rows + PositionOnGrid.y) * _grid.Columns + PositionOnGrid.x;
    }
}