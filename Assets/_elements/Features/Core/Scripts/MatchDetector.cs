using System.Collections.Generic;

public static class MatchDetector
{
    public static List<BlockController> FindMatches(BlockController[,] grid, int cols, int rows)
    {
        var matches = new HashSet<BlockController>();
        
        for(var y = 0; y < rows; y++)
        {
            var runLen = 1;
            for(var x = 1; x <= cols; x++)
            {
                var same = x < cols
                           && grid[x, y] != null
                           && grid[x-1, y] != null
                           && grid[x, y].Type == grid[x-1, y].Type;
                
                if(same)
                {
                    runLen++;
                }
                else
                {
                    if(runLen >= 3)
                    {
                        for (var k = x - runLen; k < x; k++)
                            matches.Add(grid[k, y]);
                    }
                    
                    runLen = 1;
                }
            }
        }
        
        for(var x = 0; x < cols; x++)
        {
            var runLen = 1;
            for(var y = 1; y <= rows; y++)
            {
                var same = y < rows
                           && grid[x, y] != null
                           && grid[x, y-1] != null
                           && grid[x, y].Type == grid[x, y-1].Type;
                
                if(same)
                {
                    runLen++;
                }
                else
                {
                    if(runLen >= 3)
                    {
                        for (var k = y - runLen; k < y; k++)
                            matches.Add(grid[x, k]);
                    }
                    
                    runLen = 1;
                }
            }
        }
        
        return new List<BlockController>(matches);
    }
}