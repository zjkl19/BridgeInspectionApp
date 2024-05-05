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

    public ObservableCollection<PhotoViewModel> Photos { get; } = new ObservableCollection<PhotoViewModel>();
    public ICommand PickPhotoCommand { get; }
    public ICommand TakePhotoCommand { get; }
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
        //Photos = new ObservableCollection<PhotoViewModel>(defect.Photos ?? new List<Photo>());
        componentPart = defect?.ComponentPart;
        defectType = defect?.DefectType;
        defectLocation = defect?.DefectLocation;
        defectSeverity = defect?.DefectSeverity;
        note = defect?.Note;

        // 转换 Photo 到 PhotoViewModel 并填充 Photos 集合
        Photos = new ObservableCollection<PhotoViewModel>();
        if (defect.Photos != null)
        {
            foreach (var photo in defect.Photos)
            {
                Photos.Add(new PhotoViewModel(photo.FilePath, photo.Note));
            }
        }

        LoadBridgeName();

        SaveCommand = new Command(async () => await SaveDefectAsync());
        DeleteCommand = new Command<DefectViewModel>(async (viewModel) => await ExecuteDeleteCommand(viewModel));

        PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        TakePhotoCommand = new Command(async () => await TakePhotoAsync());
    }
    private async Task PickPhotoAsync()
    {
        // 使用 MediaPicker 选择图片
        var fileResult = await MediaPicker.PickPhotoAsync();
        if (fileResult != null)
        {
            var filePath = await LoadPhotoAsync(fileResult);
            Photos.Add(new PhotoViewModel(filePath));
        }
    }

    private async Task TakePhotoAsync()
    {
        // 使用 MediaPicker 拍照
        var fileResult = await MediaPicker.CapturePhotoAsync();
        if (fileResult != null)
        {
            var filePath = await LoadPhotoAsync(fileResult);
            Photos.Add(new PhotoViewModel(filePath));
        }
    }

    private async Task<string> LoadPhotoAsync(FileResult photo)
    {
        // 存储拍摄的照片到适当位置并返回文件路径
        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(newFile))
        {
            await stream.CopyToAsync(newStream);
        }
        return newFile;
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
