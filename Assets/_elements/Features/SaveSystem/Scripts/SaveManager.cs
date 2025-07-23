using System;
using System.Text;
using UnityEngine;

public static class SaveManager
{
    private const string LastStartedLevelKey = "LastCompletedLevel";
    private const string SaveKey = "save";
    
    
    public static void SaveLastStartedLevel(int level)
    {
        PlayerPrefs.SetInt(LastStartedLevelKey, level);
        PlayerPrefs.Save();
    }
    
    public static int GetLastStartedLevel()
    {
        return PlayerPrefs.GetInt(LastStartedLevelKey, 0);
    }
    
    public static void SaveState(int level, BlockController[,] grid)
    {
        var cols = grid.GetLength(0);
        var rows = grid.GetLength(1);
        
        var sb = new StringBuilder();
        sb.Append(level);
        sb.Append(",");
        sb.Append(cols);
        sb.Append(",");
        sb.Append(rows);
        
        for(var x = 0; x < cols; x++)
        for(var y = 0; y < rows; y++)
        {
            sb.Append(",");
            sb.Append(grid[x, y]?.Type ?? BlockType.None);
        }
        
        PlayerPrefs.SetString(SaveKey, sb.ToString());
        PlayerPrefs.Save();
    }
    
    public static bool TryLoad(out int level, out BlockType[,] types)
    {
        var s = PlayerPrefs.GetString(SaveKey, null);
        if(string.IsNullOrEmpty(s))
        {
            level = 0;
            types = null;
            return false;
        }
        
        var parts = s.Split(',');
        level = int.Parse(parts[0]);
        
        var cols = int.Parse(parts[1]);
        var rows = int.Parse(parts[2]);
        types = new BlockType[cols, rows];
        
        var index = 3;
        for(var x = 0; x < cols; x++)
        for(var y = 0; y < rows; y++)
            types[x, y] = (BlockType)Enum.Parse(typeof(BlockType), parts[index++]);
        
        return true;
    }
    
    public static void ClearSaveState()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
    
#if UNITY_EDITOR
    [UnityEditor.MenuItem("SaveSystem/Delete All")]
    private static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}