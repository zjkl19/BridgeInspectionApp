using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;

using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace BridgeInspectionApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        CheckPermissions().ContinueWith(t =>
        {
            // 确保权限检查完成后再显示主页面
            MainPage = new NavigationPage(new MainPage());
        }, TaskScheduler.FromCurrentSynchronizationContext());  // 确保在主线程上更新UI
        using (var db = new BridgeContext())
        {
            db.Database.EnsureCreated();  // 确保数据库创建
            SeedDatabase(db);  // 填充测试数据
        }

        MainPage = new NavigationPage(new MainPage());
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
        if (!db.Bridges.Any())
        {
            db.Bridges.Add(new Bridge
            {
                Name = "Golden Gate Bridge",
                Location = "San Francisco, CA",
                Defects = new List<Defect>
            {
                new Defect
                {
                    ComponentPart = "Surface",
                    DefectType = "Crack",
                    DefectLocation = "North end",
                    DefectSeverity = "Moderate",
                    Description = "Surface crack on north end"
                },
                new Defect
                {
                    ComponentPart = "Cables",
                    DefectType = "Corrosion",
                    DefectLocation = "Main span",
                    DefectSeverity = "Severe",
                    Description = "Significant rusting on cables"
                }
            }
            });

            db.Bridges.Add(new Bridge
            {
                Name = "Brooklyn Bridge",
                Location = "New York, NY",
                Defects = new List<Defect>
            {
                new Defect
                {
                    ComponentPart = "Walkway",
                    DefectType = "Deformation",
                    DefectLocation = "East side",
                    DefectSeverity = "Low",
                    Description = "Minor deformation on walkway"
                },
                new Defect
                {
                    ComponentPart = "Pillar",
                    DefectType = "Crack",
                    DefectLocation = "Base",
                    DefectSeverity = "High",
                    Description = "Major crack in one of the pillars"
                }
            }
            });

            db.SaveChanges();
        }
    }

}
