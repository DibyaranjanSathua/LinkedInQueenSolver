// See https://aka.ms/new-console-template for more information

using LinkedInQueenSolverBackend.Z3Solver;

EquationSolver.Solve();
string[,] board =
{
    {"P", "P", "P", "P", "P", "P", "P", "P"},
    {"P", "O", "P", "P", "P", "B", "B", "P"},
    {"O", "O", "G", "G", "W", "W", "B", "B"},
    {"O", "O", "O", "G", "W", "B", "B", "B"},
    {"O", "Pi", "O", "C", "R", "B", "B", "B"},
    {"Pi", "Pi", "C", "C", "R", "R", "R", "B"},
    {"Pi", "Pi", "Pi", "Pi", "Pi", "Pi", "R", "B"},
    {"Pi", "B", "B", "B", "B", "B", "B", "B"}
};
Dictionary<string, List<RowColPair>> colorRegions = ColorRegionUtils.Convert(board);
QueenSolver.Solve(board.GetLength(0), colorRegions);
