// See https://aka.ms/new-console-template for more information

using ColorGridDetection;
using LinkedInQueenSolverBackend.Z3Solver;

// ColorGridImageProcessing.Test();
// ColorGridImageProcessing.CannyEdgeDetection();
string imagePath = @"C:\Users\sathua\Downloads\Queen124.png";
int[,] colorGrid = ColorGridImageProcessing.DetectColorGrid(imagePath);
for (int row = 0; row < colorGrid.GetLength(0); row++)
{
    for (int col = 0; col < colorGrid.GetLength(1); col++)
    {
        Console.Write($"{colorGrid[row, col]} ");
    }
    Console.WriteLine();
}
string[,] connectedColorComponents = ColorIslands.GetConnectedComponents(colorGrid);
Console.WriteLine("Input Color Grid");
Console.WriteLine("**********************");
for (int row = 0; row < connectedColorComponents.GetLength(0); row++)
{
    for (int col = 0; col < connectedColorComponents.GetLength(1); col++)
    {
        Console.Write($"{connectedColorComponents[row, col]} ");
    }
    Console.WriteLine();
}
Console.WriteLine("**********************");

// Solve the Queen position
Dictionary<string, List<RowColPair>> colorRegions = ColorRegionUtils.Convert(connectedColorComponents);
QueenSolver.Solve(connectedColorComponents.GetLength(0), colorRegions);
