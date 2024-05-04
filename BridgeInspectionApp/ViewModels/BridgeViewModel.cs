using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.Data;
using System.Threading.Tasks;
using BridgeInspectionApp.Models;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;

namespace BridgeInspectionApp.ViewModels;

public class BridgeViewModel : BaseViewModel
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

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

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
        SaveCommand = new Command(async () => await SaveBridge());
        DeleteCommand = new Command(async () => await DeleteBridge());
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
    private async Task SaveBridge()
    {
        using (var db = new BridgeContext())
        {
            var existingBridge = await db.Bridges.FindAsync(_bridge.Id);
            if (existingBridge == null)
            {
                db.Bridges.Add(_bridge);
            }
            else
            {
                db.Entry(existingBridge).CurrentValues.SetValues(_bridge);
                existingBridge.Defects = _bridge.Defects; // Ensure defects are updated if needed
            }
            await db.SaveChangesAsync();
        }
    }

    private async Task DeleteBridge()
    {
        using (var db = new BridgeContext())
        {
            var existingBridge = await db.Bridges.FindAsync(_bridge.Id);
            if (existingBridge != null)
            {
                db.Bridges.Remove(existingBridge);
                await db.SaveChangesAsync();
            }
        }
    }
}
