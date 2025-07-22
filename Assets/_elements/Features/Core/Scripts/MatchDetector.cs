using System.Collections.Generic;

public static class MatchDetector
{
    public static List<BlockController> FindMatches(BlockController[,] grid, int cols, int rows)
    {
        var result = new List<BlockController>();
        var used = new bool[cols, rows];
        
        for (var x = 0; x < cols; x++)
        for (var y = 0; y < rows; y++)
        {
            if (grid[x, y] == null || used[x, y])
                continue;
            
            var blockType = grid[x, y].Type;
            var queue = new Queue<(int, int)>();
            var region = new List<(int, int)>();
            queue.Enqueue((x, y));
            used[x, y] = true;
            
            while(queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();
                region.Add((cx, cy));
                
                foreach(var d in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    int nx = cx + d.Item1, ny = cy + d.Item2;
                    if(nx >= 0 && nx < cols && ny >= 0 && ny < rows && !used[nx, ny] && grid[nx, ny] != null && grid[nx, ny].Type == blockType)
                    {
                        used[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
            
            if(region.Count >= 3)
            {
                foreach(var (rx, ry) in region)
                    result.Add(grid[rx, ry]);
            }
        }
        
        return result;
    }
}