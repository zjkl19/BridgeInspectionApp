using BridgeInspectionApp.Data;
using BridgeInspectionApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BridgeInspectionApp.ViewModels;

public class BridgeListViewModel
{
    public ObservableCollection<BridgeViewModel> Bridges { get; private set; }
    public ICommand LoadBridgesCommand { get; }
    public ICommand AddBridgeCommand { get; }
    

    public BridgeListViewModel()
    {
        Bridges = [];
        //AddBridgeCommand = new Command(async () => await ExecuteAddBridgeCommand());

        LoadBridgesCommand = new Command(async () => await LoadBridgesAsync());
        LoadBridges();

    }

    private void LoadBridges()
    {
        // 模拟加载桥梁数据
        // 在实际应用中，这里应该调用数据库或服务获取桥梁数据
        using var db = new BridgeContext();
        var bridges = db.Bridges.ToList();
        foreach (var bridge in bridges)
        {
            Bridges.Add(new BridgeViewModel(bridge));
        }
        //try
        //{
        //    using var db = new BridgeContext();
        //    var bridges = await db.Bridges.ToListAsync(); // 使用异步方法加载数据
        //    bridgesCollection.ItemsSource = bridges;
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Failed to load bridges: {ex.Message}");
        //    await DisplayAlert("加载错误", $"无法加载桥梁数据。 {ex.Message}", "OK");
        //}
    }
    private async Task LoadBridgesAsync()
    {

        try
        {
            using var db = new BridgeContext();
            var bridges = await db.Bridges.ToListAsync();
            Bridges.Clear();
            foreach (var bridge in bridges)
            {
                Bridges.Add(new BridgeViewModel(bridge));
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to load bridges: " + ex.Message, "OK");
        }

    }
    private async Task ExecuteAddBridgeCommand()
    {
        // 打开添加桥梁页面
        await Application.Current.MainPage.Navigation.PushAsync(new BridgeAddPage());
    }

    

}
