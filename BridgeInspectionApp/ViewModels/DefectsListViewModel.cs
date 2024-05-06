using BridgeInspectionApp.Data;
using BridgeInspectionApp.Messages;
using BridgeInspectionApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BridgeInspectionApp.ViewModels;

public partial class DefectsListViewModel : ObservableObject
{
    [ObservableProperty]
    public string bridgeName;
    private BridgeViewModel _bridgeViewModel;
    public ObservableCollection<DefectViewModel> Defects { get; set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand AddDefectCommand { get; private set; }
    public ICommand NavigateToFullScreenCommand { get; }
    public DefectsListViewModel()
    {

    }

    public DefectsListViewModel(BridgeViewModel bridgeViewModel)
    {
        bridgeName = bridgeViewModel.Name;
        _bridgeViewModel= bridgeViewModel;
        Defects = bridgeViewModel.Defects;

        NavigateToFullScreenCommand = new Command<string>(async (filePath) =>
        {
            var page = new ImageFullScreenPage(filePath);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        });

        RegisterMessages();
        AddDefectCommand = new Command(async () => await ExecuteAddDefectCommand());
    }
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<DefectDeletedMessage>(this, (recipient, message) =>
        {
            // 从集合中移除对应的 DefectViewModel
            var defectToRemove = Defects.FirstOrDefault(d => d.Id == message.DefectId);
            if (defectToRemove != null)
            {
                Defects.Remove(defectToRemove);
            }
        });
        WeakReferenceMessenger.Default.Register<DefectUpdatedMessage>(this, (recipient, message) =>
        {
            LoadDefects();  
        });


    }
    private void LoadDefects()
    {
        try
        {
            using var db = new BridgeContext();
            var defects = db.Defects.Include(d => d.Photos)
                                    .Where(d => d.BridgeId == _bridgeViewModel.Id) // 确保只加载当前桥梁的病害
                                    .ToList();

            Defects.Clear(); // 清空现有数据，防止数据重复加载

            foreach (var defect in defects)
            {
                Defects.Add(new DefectViewModel(defect));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load defects: {ex.Message}");
            Application.Current.MainPage.DisplayAlert("加载错误", "无法加载病害数据。", "OK");
        }
    }

    private async Task ExecuteAddDefectCommand()
    {
        var defectAddPage = new DefectAddPage(new DefectViewModel(new Models.Defect { BridgeId=_bridgeViewModel.Id }), _bridgeViewModel);
        await Application.Current.MainPage.Navigation.PushAsync(defectAddPage);
    }

}
