using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
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
            if (db.Bridges.Any())
            {
                return; // 如果数据库已有数据，则不执行初始化
            }

            // 初始化数据
            List<Bridge> bridges = InitializeBridgesData();

            // 添加到数据库
            db.Bridges.AddRange(bridges);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            // 更详细的异常处理
            Console.WriteLine($"Error seeding database: {ex.Message}");
            throw; // 或处理异常，决定是否重新抛出
        }
    }

    private List<Bridge> InitializeBridgesData()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddTHHmmssfff");  // 加上毫秒确保唯一性
        // 创建武汉长江大桥和南京长江大桥的数据
        var bridge1 = new Bridge
        {
            Name = "武汉长江大桥",
            Location = "武汉",
            Defects = new List<Defect>
        {
            new Defect
            {
                ComponentPart = "主梁",
                DefectType = "裂缝",
                DefectLocation = "中部",
                DefectSeverity = "中等",
                Note = "裂缝长度约1米，需观察。",
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        FilePath = CreateTextPhoto(CreatePublicPhotoPath(timestamp), "大桥裂缝"),
                        Note = "裂缝照片"
                    }
                }
            },
            new Defect
            {
                ComponentPart = "承重结构",
                DefectType = "锈蚀",
                DefectLocation = "桥底",
                DefectSeverity = "轻微",
                Note = "检测到初期锈蚀，建议密切关注。",
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        FilePath = CreateTextPhoto(CreatePublicPhotoPath(timestamp), "初期锈蚀"),
                        Note = "锈蚀照片"
                    }
                }
            }
        }
        };

        var bridge2 = new Bridge
        {
            Name = "南京长江大桥",
            Location = "南京",
            Defects = new List<Defect>
        {
            new Defect
            {
                ComponentPart = "支柱",
                DefectType = "腐蚀",
                DefectLocation = "基座",
                DefectSeverity = "重度",
                Note = "严重腐蚀，可能影响结构安全。",
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        FilePath = CreateTextPhoto(CreatePublicPhotoPath(timestamp),"腐蚀"),
                        Note = "支柱腐蚀照片"
                    }
                }
            },
            new Defect
            {
                ComponentPart = "路面",
                DefectType = "裂缝",
                DefectLocation = "接缝处",
                DefectSeverity = "轻度",
                Note = "路面接缝处出现裂缝，需要修补。",
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        FilePath = CreateTextPhoto(CreatePublicPhotoPath(timestamp),"l1"),
                        Note = "路面裂缝照片1"
                    },
                    new Photo
                    {
                        FilePath = CreateTextPhoto(CreatePublicPhotoPath(timestamp),"l2"),
                        Note = "路面裂缝照片2"
                    }
                }
            },
            new Defect
            {
                ComponentPart = "路面3",
                DefectType = "裂缝",
                DefectLocation = "接缝处",
                DefectSeverity = "轻度",
                Note = "路面接缝处出现裂缝，需要修补。",
            }
        }
        };

        return new List<Bridge> { bridge1, bridge2 };
    }
    public string CreatePublicPhotoPath(string filename)
    {
        string folderName = "桥梁巡查"; // 应用的名称，用于创建子文件夹
        string folderPath;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            folderPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath, folderName);
        }
        else if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), folderName);
        }
        else
        {
            throw new NotSupportedException("Platform not supported");
        }

        // 获取当前时间并转化为字符串（ISO 8601 格式）
        string timestamp = DateTime.Now.ToString("yyyyMMddTHHmmssfff");  // 加上毫秒确保唯一性

        // 生成文件名
        string newFileName = SetPhotoFileName(timestamp);
        string fullPath = Path.Combine(folderPath, newFileName);

        // 检查文件是否已存在，如果存在则添加后缀
        int counter = 1;
        while (File.Exists(fullPath))
        {
            newFileName = $"IMG_{timestamp}_{counter++}.jpg";
            fullPath = Path.Combine(folderPath, newFileName);
        }

        return fullPath;
    }

    private string SetPhotoFileName(string timestamp)
    {
        // 创建基于时间的文件名
        string fileName = $"IMG_{timestamp}.jpg";

        return fileName;
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
