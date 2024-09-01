namespace ColorGridDetection;

public static class ColorIslands
{
    private static readonly int _proximityFactor = 1;

    public static string[,] GetConnectedComponents(int[,] colorValueGrid)
    {
        int rows = colorValueGrid.GetLength(0);
        int cols = colorValueGrid.GetLength(1);
        bool[,] visited = new bool[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                visited[i, j] = false;
            }
        }

        int colorNumber = 1;
        string[,] colorNameGrid = new string[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (!visited[i, j])
                {
                    colorNameGrid[i, j] = $"C{colorNumber++}";
                    DepthFirstSearch(colorValueGrid, i, j, visited, colorNameGrid);
                }
            }
        }

        return colorNameGrid;
    }

    private static void DepthFirstSearch(int[,] colorValueGrid, int row, int col, bool[,] visited, string[,] colorNameGrid)
    {
        // These arrays are used to get row and column numbers of 8 neighbours of a given cell.
        int[] rowNbr = [-1, -1, -1, 0, 0, 1, 1, 1];
        int[] colNbr = [-1, 0, 1, -1, 1, -1, 0, 1];

        // Mark this cell as visited
        visited[row, col] = true;

        // Recur for all connected neighbours
        for (int i = 0; i < 8; i++)
        {
            int newRow = row + rowNbr[i];
            int newCol = col + colNbr[i];
            if (IsValidMove(colorValueGrid, newRow, newCol, colorValueGrid[row, col], visited))
            {
                colorNameGrid[newRow, newCol] = colorNameGrid[row, col];
                DepthFirstSearch(colorValueGrid, newRow, newCol, visited, colorNameGrid);
            }
        }
    }

    /// <summary>
    /// Components are connected if they are adjacent to each other and have nearly same color value.
    /// </summary>
    /// <param name="colorValueGrid"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="currentValue"></param>
    /// <param name="visited"></param>
    /// <returns></returns>
    private static bool IsValidMove(int[,] colorValueGrid, int row, int col, int currentValue, bool[,] visited)
    {
        return row >= 0 && row < colorValueGrid.GetLength(0)
                        && col >= 0 && col < colorValueGrid.GetLength(1)
                        && Math.Abs(colorValueGrid[row, col] - currentValue) <= _proximityFactor
                        && !visited[row, col];
    }
}
