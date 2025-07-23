using UnityEngine;

[CreateAssetMenu(fileName = "level_", menuName = "Elements/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private BlockController _fireBlockPrefab;
    [SerializeField] private BlockController _waterBlockPrefab;
    [SerializeField] private int _columns = 6;
    [SerializeField] private int _rows = 10;
    [SerializeField] private BlockType[] _types; // length = columns*rows
    
    private static int LevelsCount = -1;
    
    public int Columns => _columns;
    public int Rows => _rows;
    
    
    public static LevelData Load(int index)
    {
        return Resources.Load<LevelData>($"Levels/level_{index:000}");
    }
    
    public static int GetLevelsCount()
    {
        if(LevelsCount == -1)
            LevelsCount = Resources.LoadAll<LevelData>("Levels/").Length;
        
        return LevelsCount;
    }
    
    
    public BlockType GetTypeAt(int x, int y) => _types[y * _columns + x];
    
    public BlockController GetBlockByType(BlockType type)
    {
        switch(type)
        {
            case BlockType.Fire:
                return _fireBlockPrefab;
            
            case BlockType.Water:
                return _waterBlockPrefab;
        }
        
        return null;
    }
}