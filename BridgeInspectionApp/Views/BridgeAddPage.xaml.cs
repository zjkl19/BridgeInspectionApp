
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
namespace BridgeInspectionApp.Views;

public partial class BridgeAddPage : ContentPage
{
    public ICommand AddBridgeCommand { get; private set; }

    public BridgeAddPage()
    {
        InitializeComponent();
        AddBridgeCommand = new Command(async () => await ExecuteAddBridgeCommand());
        BindingContext = this;
    }

    private async Task ExecuteAddBridgeCommand()
    {
        var bridgeName = bridgeNameEntry.Text;
        var bridgeLocation = bridgeLocationEntry.Text;

        if (string.IsNullOrWhiteSpace(bridgeName))
        {
            await DisplayAlert("错误", "桥梁名称必须填写。", "OK");
            return;
        }

        using (var db = new BridgeContext())
        {
            // 检查数据库中是否已存在同名桥梁
            var existingBridge = await db.Bridges
                                         .FirstOrDefaultAsync(b => b.Name == bridgeName);
            if (existingBridge != null)
            {
                await DisplayAlert("错误", "桥梁名称已存在，请使用不同的名称。", "OK");
                return;
            }

            var newBridge = new Bridge
            {
                Name = bridgeName,
                Location = bridgeLocation
            };

            db.Bridges.Add(newBridge);
            await db.SaveChangesAsync();
        }

        await DisplayAlert("成功", "桥梁已成功添加。", "OK");
        await Navigation.PopAsync();
    }

}