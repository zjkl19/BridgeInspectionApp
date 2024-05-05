using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.Data;
using System.Threading.Tasks;
using BridgeInspectionApp.Models;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BridgeInspectionApp.Messages;
namespace BridgeInspectionApp.ViewModels;

public partial class DefectViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid id;
    [ObservableProperty]
    private Guid bridgeId;
    [ObservableProperty]
    private string bridgeName;
    [ObservableProperty]
    private Defect defect;
    [ObservableProperty]
    private string? componentPart;
    [ObservableProperty]
    private string? defectType;
    [ObservableProperty]
    private string? defectLocation;
    [ObservableProperty]
    private string? defectSeverity;
    [ObservableProperty]
    private string? note;

    public ObservableCollection<Photo> Photos { get; set; }

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public DefectViewModel()
    {
        SaveCommand = new Command(async () => await SaveDefectAsync());
    }
    public DefectViewModel(Defect defect)
    {
        defect ??= new Defect();
        Id = defect.Id;
        BridgeId = defect.BridgeId;
        Photos = new ObservableCollection<Photo>(defect.Photos ?? new List<Photo>());
        componentPart = defect?.ComponentPart;
        defectType = defect?.DefectType;
        defectLocation = defect?.DefectLocation;
        defectSeverity = defect?.DefectSeverity;
        note = defect?.Note;

        LoadBridgeName();

        SaveCommand = new Command(async () => await SaveDefectAsync());
        DeleteCommand = new Command<DefectViewModel>(async (viewModel) => await ExecuteDeleteCommand(viewModel));
    }
    private async void LoadBridgeName()
    {
        using var db = new BridgeContext();
        var bridge = await db.Bridges.FindAsync(BridgeId);
        if (bridge != null)
        {
            BridgeName = bridge.Name;
        }
    }
    private async Task ExecuteDeleteCommand(DefectViewModel viewModel)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "删除确认",
            "删除病害将同时删除所有相关的照片。此操作不可恢复，是否继续？",
            "是",
            "否");

        if (isConfirmed)
        {
            using var db = new BridgeContext();
            var defectToDelete = await db.Defects.Include(d => d.Photos)
                                                 .FirstOrDefaultAsync(d => d.Id == viewModel.Id);

            if (defectToDelete != null)
            {
                // 删除关联的照片文件
                foreach (var photo in defectToDelete.Photos)
                {
                    var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FilePath);
                    if (File.Exists(photoPath))
                    {
                        File.Delete(photoPath);
                    }
                }

                db.Defects.Remove(defectToDelete);
                await db.SaveChangesAsync();
                await Application.Current.MainPage.DisplayAlert("成功", "病害及其照片已成功删除。", "OK");
                WeakReferenceMessenger.Default.Send(new Messages.DefectDeletedMessage(Id));
            }
        }
    }
    private async Task SaveDefectAsync()
    {
        try
        {
            using var db = new BridgeContext();
            var newDefect = new Defect
            {
                BridgeId = BridgeId, // 确保病害关联到正确的桥梁
                ComponentPart = ComponentPart,
                DefectType = DefectType,
                DefectLocation = DefectLocation,
                DefectSeverity = DefectSeverity,
                Note = Note,
                Photos = new List<Photo>() // 初始化照片列表
            };

            // 假设你已有照片处理逻辑
            foreach (var photo in Photos)
            {
                newDefect.Photos.Add(new Photo
                {
                    FilePath = photo.FilePath, // 或者是上传后的文件路径
                    Note = photo.Note
                });
            }

            db.Defects.Add(newDefect);
            await db.SaveChangesAsync();
            await Application.Current.MainPage.DisplayAlert("成功", "病害信息已保存", "OK");
            // 发送消息通知列表页面更新
            WeakReferenceMessenger.Default.Send(new DefectUpdatedMessage(Id));

            // 返回上一页
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("错误", $"保存病害出错: {ex.Message}", "OK");
        }
    }

}
