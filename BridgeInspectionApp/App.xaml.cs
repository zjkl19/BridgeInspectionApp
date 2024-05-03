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
                var bridge1 = new Bridge
                {
                    Name = "Golden Gate Bridge",
                    Location = "San Francisco, CA",
                };

                var defect1 = new Defect
                {
                    ComponentPart = "Surface",
                    DefectType = "Crack",
                    DefectLocation = "North end",
                    DefectSeverity = "Moderate",
                    Note = "Requires immediate attention due to safety concerns.",
                    BridgeId = bridge1.Id
                };

                var photo1 = new Photo
                {
                    FilePath = "path/to/photo1.jpg",
                    Note = "Initial crack photo",
                    DefectId = defect1.Id
                };
                // 将缺陷关联到桥梁
                bridge1.Defects = new List<Defect> { defect1 };

                // 将照片关联到缺陷
                defect1.Photos = new List<Photo> { photo1 };
                db.Bridges.Add(bridge1);

                var bridge2 = new Bridge
                {
                    Name = "Golden Gate Bridge2",
                    Location = "San Francisco, CA",
                };

                var defect2 = new Defect
                {
                    ComponentPart = "Surface",
                    DefectType = "Crack",
                    DefectLocation = "North end",
                    DefectSeverity = "Moderate",
                    Note = "Requires immediate attention due to safety concerns.",
                    BridgeId = bridge2.Id
                };

                var photo2 = new Photo
                {
                    FilePath = "path/to/photo2.jpg",
                    Note = "Initial crack photo",
                    DefectId = defect2.Id
                };
                // 将缺陷关联到桥梁
                bridge2.Defects = new List<Defect> { defect2 };

                // 将照片关联到缺陷
                defect2.Photos = new List<Photo> { photo2 };
                db.Bridges.Add(bridge2);

                db.SaveChanges();

            }
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }


}
