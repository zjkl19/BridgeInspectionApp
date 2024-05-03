
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
        // ʹ���첽��������
        LoadBridgesAsync();
    }

    private async Task LoadBridgesAsync()
    {
        try
        {
            using var db = new BridgeContext();
            var bridges = await db.Bridges.ToListAsync(); // ʹ���첽������������
            bridgesCollection.ItemsSource = bridges;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load bridges: {ex.Message}");
            await DisplayAlert("���ش���", $"�޷������������ݡ� {ex.Message}", "OK");
        }
    }
}