
using BridgeInspectionApp.Data;
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
}