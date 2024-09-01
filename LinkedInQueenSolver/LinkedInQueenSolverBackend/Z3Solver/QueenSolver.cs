using Microsoft.Z3;

namespace LinkedInQueenSolverBackend.Z3Solver;

public static class QueenSolver
{
    public static void Solve(int boardSize, Dictionary<string, List<RowColPair>> colorRegions)
    {
        if (colorRegions.Count != boardSize)
        {
            throw new ArgumentException("The number of colors must be equal to the board size.");
        }

        using Context ctx = new();
        Solver solver = ctx.MkSolver();
        // Create a 2D array of boolean variables representing the board
        BoolExpr[,] board = new BoolExpr[boardSize, boardSize];
        int[,] solution = new int[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                board[row, col] = ctx.MkBoolConst($"board_{row}_{col}");
            }
        }

        // Constraint: Each row must have exactly one queen
        solver.AddRowConstraints(board, ctx);
        // Constraint: Each column must have exactly one queen
        solver.AddColumnConstraints(board, ctx);
        // Constraint: No queen can be adjacent to another queen, including diagonally
        solver.AddAdjacentConstraints(board, ctx);
        // Constraint: Each colored region must have exactly one queen
        solver.AddcolorRegionConstraints(board, colorRegions, ctx);

        Status status = solver.Check();
        Console.WriteLine(status);

        if (status == Status.SATISFIABLE)
        {
            Model model = solver.Model;
            Console.WriteLine("Solution found:");
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Expr value = model.Evaluate(board[row, col]);
                    if (value.ToString() == "true")
                    {
                        Console.Write("Q ");
                    }
                    else
                    {
                        Console.Write("_ ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    private static void AddRowConstraints(this Solver solver, BoolExpr[,] board, Context ctx)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            BoolExpr rowConstraint = ctx.MkEq(ctx.MkAdd(board.GetRow(row, ctx)), ctx.MkNumeral(1, ctx.MkIntSort()));
            solver.Add(rowConstraint);
        }
    }

    private static void AddColumnConstraints(this Solver solver, BoolExpr[,] board, Context ctx)
    {
        for (int col = 0; col < board.GetLength(1); col++)
        {
            BoolExpr colConstraint = ctx.MkEq(ctx.MkAdd(board.GetColumn(col, ctx)), ctx.MkNumeral(1, ctx.MkIntSort()));
            solver.Add(colConstraint);
        }
    }

    private static void AddAdjacentConstraints(this Solver solver, BoolExpr[,] board, Context ctx)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                solver.AddLeftSideConstraint(board, row, col, ctx);
                solver.AddRightSideConstraint(board, row, col, ctx);
                solver.AddTopSideConstraint(board, row, col, ctx);
                solver.AddBottomSideConstraint(board, row, col, ctx);
                solver.AddTopLeftDiagonalConstraint(board, row, col, ctx);
                solver.AddTopRightDiagonalConstraint(board, row, col, ctx);
                solver.AddBottomLeftDiagonalConstraint(board, row, col, ctx);
                solver.AddBottomRightDiagonalConstraint(board, row, col, ctx);
            }
        }
    }

    private static void AddLeftSideConstraint(this Solver solver, BoolExpr[,] board, int row, int col, Context ctx)
    {
        if (col > 0)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row, col - 1])));
        }
    }

    private static void AddRightSideConstraint(this Solver solver, BoolExpr[,] board, int row, int col, Context ctx)
    {
        if (col < board.GetLength(1) - 1)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row, col + 1])));
        }
    }

    private static void AddTopSideConstraint(this Solver solver, BoolExpr[,] board, int row, int col, Context ctx)
    {
        if (row > 0)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row - 1, col])));
        }
    }

    private static void AddBottomSideConstraint(this Solver solver, BoolExpr[,] board, int row, int col, Context ctx)
    {
        if (row < board.GetLength(0) - 1)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row + 1, col])));
        }
    }

    private static void AddTopLeftDiagonalConstraint(this Solver solver, BoolExpr[,] board, int row, int col,
        Context ctx)
    {
        if (row > 0 && col > 0)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row - 1, col - 1])));
        }
    }

    private static void AddTopRightDiagonalConstraint(this Solver solver, BoolExpr[,] board, int row, int col,
        Context ctx)
    {
        if (row > 0 && col < board.GetLength(1) - 1)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row - 1, col + 1])));
        }
    }

    private static void AddBottomLeftDiagonalConstraint(this Solver solver, BoolExpr[,] board, int row, int col,
        Context ctx)
    {
        if (row < board.GetLength(0) - 1 && col > 0)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row + 1, col - 1])));
        }
    }

    private static void AddBottomRightDiagonalConstraint(this Solver solver, BoolExpr[,] board, int row, int col,
        Context ctx)
    {
        if (row < board.GetLength(0) - 1 && col < board.GetLength(1) - 1)
        {
            solver.Add(ctx.MkImplies(board[row, col], ctx.MkNot(board[row + 1, col + 1])));
        }
    }

    private static void AddcolorRegionConstraints(this Solver solver, BoolExpr[,] board,
        Dictionary<string, List<RowColPair>> colorRegions, Context ctx)
    {
        foreach (KeyValuePair<string, List<RowColPair>> colorRegion in colorRegions)
        {
            ArithExpr[] colorRegionExpr = colorRegion.Value
                .Select(x => (ArithExpr)ctx.MkITE(board[x.Row, x.Col], ctx.GetOne(), ctx.GetZero()))
                .ToArray();
            BoolExpr colorConstraint = ctx.MkEq(ctx.MkAdd(colorRegionExpr), ctx.MkNumeral(1, ctx.MkIntSort()));
            solver.Add(colorConstraint);
        }
    }

    private static ArithExpr[] GetRow(this BoolExpr[,] board, int row, Context ctx)
        => Enumerable.Range(0, board.GetLength(1))
            .Select(x => (ArithExpr)ctx.MkITE(board[row, x], ctx.GetOne(), ctx.GetZero()))
            .ToArray();

    private static ArithExpr[] GetColumn(this BoolExpr[,] board, int column, Context ctx)
        => Enumerable.Range(0, board.GetLength(0))
            .Select(x => (ArithExpr)ctx.MkITE(board[x, column], ctx.GetOne(), ctx.GetZero()))
            .ToArray();

    private static ArithExpr GetZero(this Context ctx)
        => (ArithExpr)ctx.MkNumeral(0, ctx.MkIntSort());

    private static ArithExpr GetOne(this Context ctx)
        => (ArithExpr)ctx.MkNumeral(1, ctx.MkIntSort());
}
