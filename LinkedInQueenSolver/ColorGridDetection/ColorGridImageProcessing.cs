using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace ColorGridDetection;

public static class ColorGridImageProcessing
{
    public static void Test()
    {
        Mat pic = CvInvoke.Imread(@"C:\Users\sathua\Downloads\color_grid.png", Emgu.CV.CvEnum.ImreadModes.AnyColor);

        Mat gaussianBlur = new();
        Mat sobelX = new();
        Mat sobelY = new();
        Mat sobelXY = new();

        pic.CopyTo(sobelX);
        pic.CopyTo(sobelY);
        pic.CopyTo(sobelXY);

        CvInvoke.GaussianBlur(pic, gaussianBlur, new System.Drawing.Size(3, 3), 5.0);
        CvInvoke.Sobel(gaussianBlur, sobelX, Emgu.CV.CvEnum.DepthType.Default, 1, 0, 5);
        CvInvoke.Sobel(gaussianBlur, sobelY, Emgu.CV.CvEnum.DepthType.Default, 0, 1, 5);
        CvInvoke.Sobel(gaussianBlur, sobelXY, Emgu.CV.CvEnum.DepthType.Default, 1, 1, 5);

        CvInvoke.Imshow("sobelX", sobelX);
        CvInvoke.Imshow("sobelY", sobelY);
        CvInvoke.Imshow("sobelXY", sobelXY);

        CvInvoke.WaitKey(0);
    }

    public static void CannyEdgeDetection()
    {
        Mat image = CvInvoke.Imread(@"C:\Users\sathua\Downloads\Queen123.png");

        Mat grayImage = new();
        CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

        Mat edges = new();
        CvInvoke.Canny(grayImage, edges, 50, 150);

        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        Console.WriteLine($"No of contours detected: {contours.Size}");
        for (int i = 0; i < contours.Size; i++)
        {
            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);
            int area = boundingBox.Width * boundingBox.Height;
            Console.WriteLine($"Bounding box area: {area}");
            if (area > 1000 && area < 5000)
            {
                CvInvoke.Rectangle(image, boundingBox, new MCvScalar(0, 0, 255), 2);
            }
        }

        CvInvoke.Imshow("edges", edges);
        CvInvoke.Imshow("image", image);
        CvInvoke.WaitKey(0);
        CvInvoke.DestroyAllWindows();
    }

    public static int[,] DetectColorGrid(string imagePath)
    {
        // Read input image
        Mat image = CvInvoke.Imread(imagePath);

        // Convert the image to grayscale
        Mat grayImage = new();
        CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

        // Apply edge detection to find the grid lines
        Mat edges = new();
        CvInvoke.Canny(grayImage, edges, 50, 150);

        // Find contours in the edged image
        VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        List<Rectangle> boundingBoxes = [];
        for (int i = 0; i < contours.Size; i++)
        {
            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);
            int area = boundingBox.Width * boundingBox.Height;
            if (area is > 1000 and < 5000)
            {
                boundingBoxes.Add(boundingBox);
            }
        }

        Console.WriteLine($"No of bounding boxes detected: {boundingBoxes.Count}");

        List<Rectangle> sortedBoundingBoxes = SortBoundingBoxes(boundingBoxes);
        Rectangle[,] gridCells = GetGridCells(sortedBoundingBoxes);
        Console.WriteLine($"Rows: {gridCells.GetLength(0)}, Cols: {gridCells.GetLength(1)}");
        int[,] colorValueGrid = new int[gridCells.GetLength(0), gridCells.GetLength(1)];
        for (int row = 0; row < gridCells.GetLength(0); row++)
        {
            for (int col = 0; col < gridCells.GetLength(1); col++)
            {
                int colorValue = GetColorValue(image, gridCells[row, col].ReduceSize(0.1f));
                colorValueGrid[row, col] = colorValue;
            }
        }

        return colorValueGrid;
    }

    private static List<Rectangle> SortBoundingBoxes(List<Rectangle> boundingBoxes)
        => boundingBoxes.OrderBy(b => b.Top).ThenBy(b => b.Left).ToList();

    /// <summary>
    /// Convert the list of bounding boxes into a 2D grid of cells.
    /// </summary>
    /// <param name="boundingBoxes"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static Rectangle[,] GetGridCells(List<Rectangle> boundingBoxes)
    {
        int rows = 0;
        int cols;
        int prevTop = -1;
        int prevLeft;
        foreach (Rectangle boundingBox in boundingBoxes)
        {
            if (prevTop == -1 || Math.Abs(boundingBox.Top - prevTop) > 10)
            {
                rows++;
            }

            prevTop = boundingBox.Top;
        }

        if (rows != 0)
        {
            cols = boundingBoxes.Count / rows;
        }
        else
        {
            throw new ArgumentException("No bounding boxes found on different rows.");
        }

        Console.WriteLine($"Rows: {rows}, Cols: {cols}");
        Rectangle[,] gridCells = new Rectangle[rows, cols];
        int row = -1;
        int col = -1;
        prevTop = -1;
        prevLeft = -1;
        foreach (Rectangle boundingBox in boundingBoxes)
        {
            if (prevTop == -1 || Math.Abs(boundingBox.Top - prevTop) > 10)
            {
                row++;
                col = -1;
            }

            if (prevLeft == -1 || Math.Abs(boundingBox.Left - prevLeft) > 10)
            {
                col++;
            }

            gridCells[row, col] = boundingBox;
            prevTop = boundingBox.Top;
            prevLeft = boundingBox.Left;
        }

        return gridCells;
    }

    private static Rectangle ReduceSize(this Rectangle boundingBox, float reductionPercent)
    {
        int reductionWidth = (int)(boundingBox.Width * reductionPercent);
        int reductionHeight = (int)(boundingBox.Height * reductionPercent);
        // Reduce each side by the given percentage
        return new Rectangle(boundingBox.X + reductionWidth, boundingBox.Y + reductionHeight,
            boundingBox.Width - 2 * reductionWidth, boundingBox.Height - 2 * reductionHeight);
    }

    private static int GetColorValue(Mat grayImage, Rectangle regionOfInterest)
    {
        Mat croppedImage = new(grayImage, regionOfInterest);
        return (int)CvInvoke.Mean(croppedImage).V0;
    }

}
