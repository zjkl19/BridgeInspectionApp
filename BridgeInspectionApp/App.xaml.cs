using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace BridgeInspectionApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        DeleteExistingDatabase();
        SetupDatabase();
        MainPage = new NavigationPage(new MainPage());
    }
    private void DeleteExistingDatabase()
    {
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "BridgeDatabase.db");
        if (File.Exists(databasePath))
        {
            File.Delete(databasePath);
        }
    }
    private void SetupDatabase()
    {
        CheckPermissions().ContinueWith(t =>
        {
            // 确保权限检查完成后再显示主页面
            MainPage = new NavigationPage(new MainPage());
        }, TaskScheduler.FromCurrentSynchronizationContext());  // 确保在主线程上更新UI
        PhotoService.GenerateTestPhotos();
        using var db = new BridgeContext();
        
        db.Database.EnsureDeleted();  // 确保删除旧数据库
        db.Database.EnsureCreated();  // 创建新数据库
        SeedDatabase(db);  // 填充测试数据
    }

    public async Task CheckPermissions()
    {
        var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (writeStatus != PermissionStatus.Granted)
        {
            writeStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
        }

        var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        if (readStatus != PermissionStatus.Granted)
        {
            readStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
        }

        if (writeStatus == PermissionStatus.Granted && readStatus == PermissionStatus.Granted)
        {
            // 权限被授予，可以进行文件操作
        }
        else
        {
            // 权限被拒绝，提示用户
            await MainThread.InvokeOnMainThreadAsync(() =>
                Current.MainPage.DisplayAlert("权限拒绝", "无法访问存储。", "确定"));
        }
    }
    private void SeedDatabase(BridgeContext db)
    {
        try
        {
            if (!db.Bridges.Any())
            {
                // 第一座桥梁 - 武汉长江大桥
                var bridge1 = new Bridge
                {
                    Name = "武汉长江大桥",
                    Location = "武汉"
                };
                // 第一个病害 - 裂缝
                var defect1 = new Defect
                {
                    ComponentPart = "主梁",
                    DefectType = "裂缝",
                    DefectLocation = "中部",
                    DefectSeverity = "中等",
                    Note = "裂缝长度约1米，需观察。",
                    BridgeId = bridge1.Id
                };

                var photo1 = new Photo
                {
                    FilePath = CreateTextPhoto(Path.Combine(FileSystem.AppDataDirectory, "武汉长江大桥裂缝.jpg"), "大桥裂缝"),
                    Note = "裂缝照片",
                    DefectId = defect1.Id
                };

                defect1.Photos = [photo1];

                // 第二个病害 - 锈蚀
                var defect2 = new Defect
                {
                    ComponentPart = "承重结构",
                    DefectType = "锈蚀",
                    DefectLocation = "桥底",
                    DefectSeverity = "轻微",
                    Note = "检测到初期锈蚀，建议密切关注。",
                    BridgeId = bridge1.Id
                };

                var photo2 = new Photo
                {
                    FilePath = CreateTextPhoto(Path.Combine(FileSystem.AppDataDirectory, "武汉长江大桥锈蚀.jpg"), "初期锈蚀"),
                    Note = "锈蚀照片",
                    DefectId = defect2.Id
                };

                defect2.Photos = [photo2];

                bridge1.Defects = [defect1, defect2];
                // 第二座桥梁 - 南京长江大桥
                var bridge2 = new Bridge
                {
                    Name = "南京长江大桥",
                    Location = "南京"
                };

                var defect3 = new Defect
                {
                    ComponentPart = "支柱",
                    DefectType = "腐蚀",
                    DefectLocation = "基座",
                    DefectSeverity = "重度",
                    Note = "严重腐蚀，可能影响结构安全。",
                    BridgeId = bridge2.Id
                };

                var photo3 = new Photo
                {
                    FilePath = CreateTextPhoto(Path.Combine(FileSystem.AppDataDirectory, "南京长江大桥腐蚀.jpg"), "支柱腐蚀"),
                    Note = "支柱腐蚀照片",
                    DefectId = defect3.Id
                };
                defect3.Photos = [photo3];
                // 第二个病害 - 裂缝
                var defect4 = new Defect
                {
                    ComponentPart = "路面",
                    DefectType = "裂缝",
                    DefectLocation = "接缝处",
                    DefectSeverity = "轻度",
                    Note = "路面接缝处出现裂缝，需要修补。",
                    BridgeId = bridge2.Id
                };

                var photo4 = new Photo
                {
                    FilePath = CreateTextPhoto(Path.Combine(FileSystem.AppDataDirectory, "南京长江大桥路面裂缝1.jpg"), "路面裂缝"),
                    Note = "路面裂缝照片",
                    DefectId = defect4.Id
                };
                var photo5 = new Photo
                {
                    FilePath = CreateTextPhoto(Path.Combine(FileSystem.AppDataDirectory, "南京长江大桥路面裂缝2.jpg"), "路裂缝2"),
                    Note = "路面裂缝照片",
                    DefectId = defect4.Id
                };
                defect4.Photos = [photo4, photo5];

                // 第3个病害 - 裂缝
                var defect5 = new Defect
                {
                    ComponentPart = "路面3",
                    DefectType = "裂缝",
                    DefectLocation = "接缝处",
                    DefectSeverity = "轻度",
                    Note = "路面接缝处出现裂缝，需要修补。",
                    BridgeId = bridge2.Id
                };

                bridge2.Defects = [defect3, defect4, defect5];

                db.Bridges.Add(bridge1);
                db.Bridges.Add(bridge2);
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            throw;
        }
    }

    private string CreateTestPhoto(string filePath)
    {
        // Check if file already exists to avoid overwriting
        if (!File.Exists(filePath))
        {
            // Example of creating a simple image using SkiaSharp or any suitable graphic library
            using var bitmap = new SkiaSharp.SKBitmap(100, 100);
            using (var canvas = new SkiaSharp.SKCanvas(bitmap))
            {
                canvas.Clear(SkiaSharp.SKColors.White);
                var paint = new SkiaSharp.SKPaint
                {
                    Color = SkiaSharp.SKColors.Black,
                    StrokeWidth = 3
                };
                canvas.DrawLine(0, 0, 100, 100, paint);
                canvas.DrawLine(0, 100, 100, 0, paint);
            }

            using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);
        }

        // Return the file path whether it was just created or already existed
        return filePath;
    }

    private string CreateTextPhoto(string filePath, string text, bool overwrite = true)
    {
        // Check if file already exists to decide on overwriting
        if (overwrite || !File.Exists(filePath))
        {
            // Example of creating an image with custom text using SkiaSharp
            using var bitmap = new SkiaSharp.SKBitmap(200, 100);
            using (var canvas = new SkiaSharp.SKCanvas(bitmap))
            {
                // Set the background color to white
                canvas.Clear(SkiaSharp.SKColors.White);

                // Create a paint object for drawing the text
                var paint = new SkiaSharp.SKPaint
                {
                    Color = SkiaSharp.SKColors.Black,
                    TextSize = 24, // Set the text size
                    IsAntialias = true, // Enable anti-aliasing for smoother text
                    TextAlign = SkiaSharp.SKTextAlign.Center // Align text in the center
                };

                // Calculate the text coordinates to center it
                float x = bitmap.Width / 2;
                float y = (bitmap.Height + paint.TextSize) / 2; // Adjust for baseline

                // Draw the text
                canvas.DrawText(text, x, y, paint);
            }

            using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);
        }

        // Return the file path whether it was just created or already existed
        return filePath;
    }




}
