using System.Collections.Generic;
using UnityEngine;

public static class MatchDetector
{
    public static List<BlockController> FindMatches(BlockController[,] grid, int cols, int rows)
    {
        var initial = new HashSet<BlockController>();
        
        // 1) Find all horizontal segments ≥3
        for(var y = 0; y < rows; y++)
        {
            var runLen = 1;
            for(var x = 1; x <= cols; x++)
            {
                var same = x < cols
                           && grid[x, y] != null
                           && grid[x - 1, y] != null
                           && grid[x, y].Type == grid[x - 1, y].Type;
                
                if(same)
                {
                    runLen++;
                    continue;
                }
                
                if(runLen >= 3)
                {
                    for(var k = x - runLen; k < x; k++)
                        initial.Add(grid[k, y]);
                }
                
                runLen = 1;
            }
        }
        
        // 2) Find all vertical segments ≥3
        for(var x = 0; x < cols; x++)
        {
            var runLen = 1;
            for(var y = 1; y <= rows; y++)
            {
                var same = y < rows
                           && grid[x, y] != null
                           && grid[x, y - 1] != null
                           && grid[x, y].Type == grid[x, y - 1].Type;
                
                if(same)
                {
                    runLen++;
                    continue;
                }
                
                if(runLen >= 3)
                {
                    for(var k = y - runLen; k < y; k++)
                        initial.Add(grid[x, k]);
                }
                
                runLen = 1;
            }
        }
        
        // 3) Expand the area from each “tie” block
        var result  = new List<BlockController>();
        var visited = new HashSet<BlockController>();
        var queue = new Queue<BlockController>();
        
        foreach(var start in initial)
        {
            if(!visited.Add(start))
                continue;
            
            queue.Clear();
            queue.Enqueue(start);
            
            while(queue.Count > 0)
            {
                var b = queue.Dequeue();
                result.Add(b);
                var pos = b.PositionOnGrid;
                
                var neighbourPositions = new[]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1)
                };
                
                foreach (var d in neighbourPositions)
                {
                    int nx = pos.x + d.x, ny = pos.y + d.y;
                    if(nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                        continue;
                    
                    var nb = grid[nx, ny];
                    
                    if(nb == null || visited.Contains(nb))
                        continue;
                    
                    if(nb.Type != start.Type)
                        continue;
                    
                    visited.Add(nb);
                    queue.Enqueue(nb);
                }
            }
        }
        
        return result;
    }
}