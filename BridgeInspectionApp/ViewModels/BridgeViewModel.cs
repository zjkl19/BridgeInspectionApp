using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.Data;
using System.Threading.Tasks;
using BridgeInspectionApp.Models;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using BridgeInspectionApp.Views;

namespace BridgeInspectionApp.ViewModels;

public partial class BridgeViewModel : BaseViewModel
{
    public ICommand DeleteBridgeCommand { get; }
    
    private Bridge _bridge;
    private string _name;
    private string? _location;
    private string? _mapId;

    public Guid Id
    {
        get => _bridge.Id;
        set
        {
            if (_bridge.Id != value)
            {
                _bridge.Id = value;
                OnPropertyChanged();
            }
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public string? MapId
    {
        get => _mapId;
        set => SetProperty(ref _mapId, value);
    }

    public ObservableCollection<DefectViewModel> Defects { get; set; }
    public ICommand EditBridgeCommand { get; }
    public ICommand EditConfirmedCommand { get;}
    public ICommand EditCancelCommand { get; }
    public BridgeViewModel()
    { }
    public BridgeViewModel(Bridge bridge)
    {
        _bridge = bridge ?? new Bridge { Defects = [] };
        _name = bridge.Name;
        _location = bridge.Location;
        _mapId = bridge.MapId;

        //Defects = [];
        //foreach (var defect in bridge.Defects)
        //{
        //    Defects.Add(new DefectViewModel(defect));
        //}
        DeleteBridgeCommand = new Command<BridgeViewModel>(async (bridgeViewModel) => await ExecuteDeleteBridgeCommand(bridgeViewModel));
        EditBridgeCommand = new RelayCommand(ExecuteEditBridgeCommand);
        EditConfirmedCommand = new Command<BridgeViewModel>(async (bridgeViewModel) => await ExecuteEditConfirmedCommand(bridgeViewModel));
        EditCancelCommand = new Command(async () => await ExecuteEditCancelCommand());
    }
    private async Task ExecuteDeleteBridgeCommand(BridgeViewModel bridgeViewModel)
    {
        bool answer = await Application.Current.MainPage.DisplayAlert("删除确认", "删除桥梁将同时删除所有相关的病害和照片。此操作不可恢复，是否继续？", "是", "否");
        if (answer && bridgeViewModel != null)
        {
            using var db = new BridgeContext();
            var bridgeToDelete = await db.Bridges
                .Include(b => b.Defects)
                .ThenInclude(d => d.Photos)
                .FirstOrDefaultAsync(b => b.Id == bridgeViewModel.Id);

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
                // 发送消息通知桥梁已删除
                WeakReferenceMessenger.Default.Send(new Messages.BridgeDeletedMessage(Id));
            }
        }
    }

    private void ExecuteEditBridgeCommand()
    {
        var navigation = Application.Current.MainPage.Navigation;
        // 假设 BridgeEditPage 是编辑桥梁的页面
        navigation.PushAsync(new BridgeEditPage(new BridgeViewModel(_bridge)));
    }
    [RelayCommand]
    private async Task ExecuteEditConfirmedCommand(BridgeViewModel bridgeViewModel)
    {
        using var db = new BridgeContext();
        // 检查数据库中是否存在同名但不同 ID 的桥梁
        var duplicateBridge = await db.Bridges
            .FirstOrDefaultAsync(b => b.Name == bridgeViewModel.Name && b.Id != bridgeViewModel.Id);

        if (duplicateBridge != null)
        {
            await Application.Current.MainPage.DisplayAlert("错误", "存在同名的桥梁，请更换桥梁名称后重试。", "OK");
            return;
        }

        var storedBridge = await db.Bridges.FindAsync(bridgeViewModel.Id);
        if (storedBridge != null)
        {
            storedBridge.Name = bridgeViewModel.Name;
            storedBridge.Location = bridgeViewModel.Location;
            storedBridge.MapId = bridgeViewModel.MapId;
            await db.SaveChangesAsync();
            await Application.Current.MainPage.DisplayAlert("成功", "桥梁信息已更新", "OK");
        }
    }
    [RelayCommand]
    private async Task ExecuteEditCancelCommand()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}
