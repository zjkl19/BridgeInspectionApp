using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BridgeInspectionApp.ViewModels;

public class BridgeListViewModel : ObservableObject
{
    public ObservableCollection<BridgeViewModel> Bridges { get; private set; }
    public ICommand LoadBridgesCommand { get; }
    public ICommand BridgeAddCommand { get; }


    public BridgeListViewModel()
    {
        Bridges = [];
        WeakReferenceMessenger.Default.Register<Messages.BridgeDeletedMessage>(this, (recipient, message) =>
        {
            var bridge = Bridges.FirstOrDefault(b => b.Id == message.BridgeId);
            if (bridge != null)
            {
                Bridges.Remove(bridge);
            }
        });
        LoadBridgesCommand = new Command(async () => await LoadBridgesAsync());
        LoadBridges();
        BridgeAddCommand = new RelayCommand(async () => await ExecuteBridgeAddCommand());

    }

    private void LoadBridges()
    {
        try
        {
            using var db = new BridgeContext();
            var bridges = db.Bridges
                            .Include(b => b.Defects)
                            .ThenInclude(d => d.Photos)
                            .ToList(); // 确保从数据库加载所有桥梁和相关的病害及照片

            Bridges.Clear(); // 清除现有集合内容，避免重复添加
            foreach (var bridge in bridges)
            {
                Bridges.Add(new BridgeViewModel(bridge)); // 将加载的桥梁添加到集合中
            }
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("错误", $"加载桥梁数据时发生错误: {ex.Message}", "确定");
        }
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
    private async Task ExecuteBridgeAddCommand()
    {
        // 打开添加桥梁页面
        await Application.Current.MainPage.Navigation.PushAsync(new BridgeAddPage());
    }



}
