using System.Collections.Generic;
using System.Drawing;
using Microsoft.Maui.Storage;
using System.IO;
using SkiaSharp;

namespace BridgeInspectionApp.Services;

public static class PhotoService
{
    public static void GenerateTestPhotos()
    {
        var photoPaths = new List<string>
            {
                "photo1.jpg",
                "photo2.jpg",
                "photo3.jpg",
                "photo4.jpg",
                "photo5.jpg",
                "photo6.jpg"
            };

        foreach (var photoPath in photoPaths)
        {
            var fullPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, photoPath);
            CreateSampleImage(fullPath);
        }
    }

    private static void CreateSampleImage(string path)
    {
        int width = 100;
        int height = 100;
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Azure);

        var paint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 20,
            IsAntialias = true
        };
        canvas.DrawText("Test", 10, 35, paint);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }
}