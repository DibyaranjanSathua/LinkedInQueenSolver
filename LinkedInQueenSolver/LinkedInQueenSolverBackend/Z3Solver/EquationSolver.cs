using Microsoft.Z3;

namespace LinkedInQueenSolverBackend.Z3Solver;

/// <summary>
/// Solve the following equation:
/// x > 0
/// y = x + 1
/// y < 3
/// </summary>
public class EquationSolver
{
    public static void Solve()
    {
        using Context ctx = new();
        Expr x = ctx.MkConst("x", ctx.MkIntSort());
        Expr y = ctx.MkConst("y", ctx.MkIntSort());
        Expr zero = ctx.MkNumeral(0, ctx.MkIntSort());
        Expr one = ctx.MkNumeral(1, ctx.MkIntSort());
        Expr three = ctx.MkNumeral(3, ctx.MkIntSort());

        Solver solver = ctx.MkSolver();
        BoolExpr equation1 = ctx.MkGt((ArithExpr)x, (ArithExpr)zero);
        BoolExpr equation2 = ctx.MkEq((ArithExpr)y, ctx.MkAdd((ArithExpr)x, (ArithExpr)one));
        BoolExpr equation3 = ctx.MkLt((ArithExpr)y, (ArithExpr)three);
        solver.Add(equation1, equation2, equation3);
        Console.WriteLine(solver.Check());

        Model model = solver.Model;
        foreach (FuncDecl modelDecl in model.Decls)
        {
            Console.WriteLine($"{modelDecl.Name} = {model.ConstInterp(modelDecl)}");
        }
    }
}
