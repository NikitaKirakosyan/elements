using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _minSwipe = 0.5f;
    
    private Vector2 _touchStart;
    
    public int TypeId { get; private set; }
    public Vector2Int PositionOnGrid { get; private set; }
    
    
    void IPointerDownHandler.OnPointerDown(PointerEventData data)
    {
        _touchStart = data.position;
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData data)
    {
        //Do swipe logic
        var diff = data.position - _touchStart;
        if (diff.magnitude < _minSwipe * Screen.dpi)
            return;
        
        diff.Normalize();
    }
    
    
    public void Init(int typeId, Vector2Int positionOnGrid)
    {
        TypeId = typeId;
        PositionOnGrid = positionOnGrid;
    }
    
    public void MoveToCell(Vector2Int positionOnGrid)
    {
        PositionOnGrid = positionOnGrid;
        //TODO do move
    }
    
    public void SetCoords(Vector2Int positionOnGrid)
    {
        PositionOnGrid = positionOnGrid;
    }
    
    public void Die()
    {
        Destroy(gameObject); //TODO add animation
    }
}