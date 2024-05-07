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
using BridgeInspectionApp.Views;
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
    public ICommand RemovePhotoCommand { get; }
    public ICommand NavigateToFullScreenCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand EditConfirmedCommand { get; }
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

        EditCommand = new Command(async () => await EditDefectAsync());
        UpdateCommandCommand = new Command(async () => await UpdateDefectAsync());
        SaveCommand = new Command(async () => await SaveDefectAsync());
        DeleteCommand = new Command<DefectViewModel>(async (viewModel) => await ExecuteDeleteCommand(viewModel));

        PickPhotoCommand = new Command(async () => await PickPhotoAsync());
        TakePhotoCommand = new Command(async () => await TakePhotoAsync());
        RemovePhotoCommand = new Command<PhotoViewModel>(RemovePhoto);

        //NavigateToFullScreenCommand = new Command<string>(async (filePath) =>
        //{
        //    await Shell.Current.GoToAsync($"{nameof(ImageFullScreenPage)}?ImagePath={filePath}");
        //});
        //NavigateToFullScreenCommand = new Command<string>(async (filePath) =>
        //{
        //    await Shell.Current.GoToAsync(nameof(ImageFullScreenPage), true, new Dictionary<string, object>
        //{
        //    { "ImagePath", filePath }
        //});
        //});

        NavigateToFullScreenCommand = new Command<string>(async (filePath) =>
        {
            var page = new ImageFullScreenPage(filePath);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        });


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
    private void RemovePhoto(PhotoViewModel photoViewModel)
    {
        if (photoViewModel != null)
        {
            Photos.Remove(photoViewModel);
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

    private async Task EditDefectAsync()
    {
        // 为编辑创建并导航到 DefectEditPage，同时传递当前 ViewModel
        await Application.Current.MainPage.Navigation.PushAsync(new DefectEditPage(this));
    }
    private async Task UpdateDefectAsync()
    {
        if (string.IsNullOrWhiteSpace(ComponentPart))
        {
            await Application.Current.MainPage.DisplayAlert("错误", "请填写构件部位", "OK");
            return; // 终止操作
        }
        try
        {
            using var db = new BridgeContext();
            var defectToUpdate = await db.Defects.FindAsync(Id);
            if (defectToUpdate != null)
            {
                defectToUpdate.ComponentPart = ComponentPart;
                defectToUpdate.DefectType = DefectType;
                defectToUpdate.DefectLocation = DefectLocation;
                defectToUpdate.DefectSeverity = DefectSeverity;
                defectToUpdate.Note = Note;

                // 更新照片列表，假设已经处理好了照片的添加和删除
                db.Update(defectToUpdate);
                await db.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("成功", "病害信息已更新", "OK");
                // 发送消息通知列表页面更新
                WeakReferenceMessenger.Default.Send(new DefectUpdatedMessage(Id));

                // 返回上一页
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("错误", $"更新病害出错: {ex.Message}", "OK");
        }
    }
    private async Task SaveDefectAsync()
    {
        if (string.IsNullOrWhiteSpace(ComponentPart))
        {
            await Application.Current.MainPage.DisplayAlert("错误", "请填写构件部位", "OK");
            return; // 终止操作
        }
        try
        {
            using var db = new BridgeContext();
            var newDefect = new Defect
            {
                BridgeId = BridgeId,
                ComponentPart = ComponentPart,
                DefectType = DefectType,
                DefectLocation = DefectLocation,
                DefectSeverity = DefectSeverity,
                Note = Note,
                Photos = new List<Photo>() // 初始化照片列表
            };

            db.Defects.Add(newDefect);
            await db.SaveChangesAsync();
            // 保存照片文件并更新数据库
            await SavePhotoFilesAsync(db, newDefect);

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
    private async Task SavePhotoFilesAsync(BridgeContext db, Defect newDefect)
    {
        foreach (var photoViewModel in Photos)
        {
            try
            {
                if (!string.IsNullOrEmpty(photoViewModel.FilePath))
                {
                    // 假设 FilePath 是临时存储路径
                    //var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MyAppPhotos", Path.GetFileName(photoViewModel.FilePath));
                    var targetPath = GetPhotoFilePath();
                    // 确保目标目录存在
                    var directory = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    // 检查文件是否已存在
                    if (File.Exists(targetPath))
                    {
                        bool overwrite = await Application.Current.MainPage.DisplayAlert("覆盖文件", $"文件 {Path.GetFileName(photoViewModel.FilePath)} 已存在。是否覆盖？", "是", "否");
                        if (!overwrite)
                        {
                            continue; // 跳过此文件
                        }
                    }

                    // 尝试将文件从临时位置移动到目标位置
                    File.Move(photoViewModel.FilePath, targetPath, overwrite: true);
                    var newPhoto = new Photo
                    {
                        FilePath = targetPath,
                        Note = photoViewModel.Note,
                        DefectId = newDefect.Id
                    };
                    db.Photos.Add(newPhoto);  // 确保新照片被添加到数据库上下文中
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // 日志或处理文件操作中的异常，这样即使有照片处理失败，也不会影响其他照片的保存
                Console.WriteLine($"Error processing photo {photoViewModel.FilePath}: {ex.Message}");
                // 可以选择在这里通知用户某个特定的照片保存失败
                await Application.Current.MainPage.DisplayAlert("照片保存错误", $"无法保存照片 {Path.GetFileName(photoViewModel.FilePath)}: {ex.Message}", "OK");
            }
        }
    }

    private string GeneratePhotoFileName(string timestamp)
    {
        // 创建基于时间的文件名
        string fileName = $"IMG_{timestamp}.jpg";

        return fileName;
    }

    private string GetPhotoFilePath()
    {
        string folderName = "桥梁巡查"; // 应用的名称，用于创建子文件夹
        string folderPath;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            folderPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath, folderName);
        }
        else if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), folderName);
        }
        else
        {
            throw new NotSupportedException("Platform not supported");
        }

        // 获取当前时间并转化为字符串（ISO 8601 格式）
        string timestamp = DateTime.Now.ToString("yyyyMMddTHHmmssfff");  // 加上毫秒确保唯一性

        // 生成文件名
        string newFileName = GeneratePhotoFileName(timestamp);
        string fullPath = Path.Combine(folderPath, newFileName);

        // 检查文件是否已存在，如果存在则添加后缀
        int counter = 1;
        while (File.Exists(fullPath))
        {
            newFileName = $"IMG_{timestamp}_{counter++}.jpg";
            fullPath = Path.Combine(folderPath, newFileName);
        }

        return fullPath;
    }
}
