
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Linq;
namespace BridgeInspectionApp.Views;

public partial class BridgeListViewPage : ContentPage
{
    public BridgeListViewPage()
    {
        InitializeComponent();
        // 使用异步加载数据
        LoadBridgesAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadBridgesAsync(); // 重新加载桥梁数据以更新UI
    }
    private async Task LoadBridgesAsync()
    {
        try
        {
            using var db = new BridgeContext();
            var bridges = await db.Bridges.ToListAsync(); // 使用异步方法加载数据
            bridgesCollection.ItemsSource = bridges;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load bridges: {ex.Message}");
            await DisplayAlert("加载错误", $"无法加载桥梁数据。 {ex.Message}", "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bridge = button?.BindingContext as Bridge;
        if (bridge != null)
        {
            await Navigation.PushAsync(new BridgeEditPage(bridge));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("删除确认", "删除桥梁将同时删除所有相关的病害和照片。此操作不可恢复，是否继续？", "是", "否");
        if (answer)
        {
            var button = sender as Button;
            var bridge = button?.BindingContext as Bridge;

            if (bridge != null)
            {
                using var db = new BridgeContext();
                var bridgeToDelete = db.Bridges.Include(b => b.Defects).ThenInclude(d => d.Photos).FirstOrDefault(b => b.Id == bridge.Id);

                if (bridgeToDelete != null)
                {
                    // 删除所有关联照片文件
                    foreach (var defect in bridgeToDelete.Defects)
                    {
                        foreach (var photo in defect.Photos)
                        {
                            var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FilePath);
                            if (File.Exists(photoPath))
                            {
                                File.Delete(photoPath);
                            }
                        }
                    }

                    // 删除桥梁记录
                    db.Bridges.Remove(bridgeToDelete);
                    await db.SaveChangesAsync();

                    // 刷新桥梁列表
                    LoadBridgesAsync();
                }
            }
        }
    }


    private void OnManageDefectsClicked(object sender, EventArgs e)
    {
        // 管理病害逻辑
    }
}