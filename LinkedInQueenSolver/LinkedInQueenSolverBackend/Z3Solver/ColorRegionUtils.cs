namespace LinkedInQueenSolverBackend.Z3Solver;

public static class ColorRegionUtils
{
    public static Dictionary<string, List<RowColPair>> Convert(string[,] board)
    {
        Dictionary<string, List<RowColPair>> colorRegions = [];
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                string color = board[row, col];
                if (!colorRegions.ContainsKey(color))
                {
                    colorRegions[color] = [];
                }
                colorRegions[color].Add(new RowColPair(row, col));
            }
        }
        return colorRegions;
    }
}
