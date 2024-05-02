
using BridgeInspectionApp.Data;
using Microsoft.Maui.Controls;
using System.Linq;
namespace BridgeInspectionApp.Views;

public partial class BridgeListViewPage : ContentPage
{
    public BridgeListViewPage()
    {
        InitializeComponent();
        LoadBridges();
    }

    private void LoadBridges()
    {
        using var db = new BridgeContext();
        var bridges = db.Bridges.ToList();
        bridgesCollection.ItemsSource = bridges;
    }
}