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
using System.IO.Compression;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace BridgeInspectionApp.ViewModels;

public partial class BridgeListViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<BridgeViewModel> bridges;
    [ObservableProperty]

    public ObservableCollection<BridgeViewModel> selectedBridges;
    public ICommand LoadBridgesCommand { get; }
    public ICommand BridgeAddCommand { get; }
    public ICommand PackSelectedCommand { get; }
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
        WeakReferenceMessenger.Default.Register<Messages.BridgeUpdatedMessage>(this, (recipient, message) =>
        {
            LoadBridges();
        });

        LoadBridges();

        LoadBridgesCommand = new Command(async () => await LoadBridgesAsync());
        BridgeAddCommand = new RelayCommand(async () => await ExecuteBridgeAddCommand());
        PackSelectedCommand = new Command(async () => await PackSelected());

    }
    public void FilterBridges(string searchText)
    {
        using var db = new BridgeContext();
        var allBridges =  db.Bridges
                            .Include(b => b.Defects)
                            .ThenInclude(d => d.Photos)
                            .ToList(); // 确保从数据库加载所有桥梁和相关的病害及照片

        if (string.IsNullOrEmpty(searchText))
        {
            // 如果搜索文本为空，显示所有桥梁
            Bridges = new ObservableCollection<BridgeViewModel>(allBridges.Select(b => new BridgeViewModel(b)));
        }
        else
        {
            // 否则，只显示名称包含搜索文本的桥梁
            Bridges = new ObservableCollection<BridgeViewModel>(allBridges.Where(b => b.Name.Contains(searchText)).Select(b => new BridgeViewModel(b)));
        }
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
        await Application.Current.MainPage.Navigation.PushAsync(new BridgeAddPage (new BridgeViewModel()));
    }

    private async Task PackSelected()
    {

        // 获取所选桥梁的所有照片，按桥梁分组
        var selectedBridgePhotos = GetSelectedBridgePhotos().GroupBy(p => p.Key);

        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "桥梁巡查", "打包");
        Directory.CreateDirectory(folderPath); // 确保目录存在

        List<string> bridgeZipFiles = new List<string>();

        // 对于每个选中的桥梁，创建一个以桥梁名称命名的压缩文件
        // 对于每个选中的桥梁，创建一个以桥梁名称命名的压缩文件
        foreach (var bridgePhotos in selectedBridgePhotos)
        {
            string bridgeZipFilePath = Path.Combine(folderPath, $"{bridgePhotos.Key}.zip");
            using var bridgeArchive = ZipFile.Open(bridgeZipFilePath, ZipArchiveMode.Create);
            foreach (var bridgePhotoPair in bridgePhotos)
            {
                foreach (var photo in bridgePhotoPair.Value)
                {
                    bridgeArchive.CreateEntryFromFile(photo.FilePath, Path.GetFileName(photo.FilePath));
                }
            }
            bridgeZipFiles.Add(bridgeZipFilePath);
        }

        // 创建一个新的压缩文件，将所有桥梁的压缩文件添加到这个新的压缩文件中
        string zipFilePath = Path.Combine(folderPath, "选中的桥梁照片.zip");
        if (File.Exists(zipFilePath))
        {
            bool overwrite = await Application.Current.MainPage.DisplayAlert("提示", "文件已经存在，是否要覆盖？", "是", "否");
            if (!overwrite)
            {
                return;
            }
            else
            {
                File.Delete(zipFilePath);
            }
        }
        using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
        foreach (var bridgeZipFile in bridgeZipFiles)
        {
            archive.CreateEntryFromFile(bridgeZipFile, Path.GetFileName(bridgeZipFile));
        }

        archive.Dispose(); // 显式释放ZipArchive对象
        await ShareFile(zipFilePath);
    }

    public async Task ShareFile(string filePath)
    {
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "分享桥梁照片压缩包",
            File = new ShareFile(filePath)
        });
    }
    private Dictionary<string, List<BridgePhoto>> GetSelectedBridgePhotos()
    {
        SelectedBridges = new ObservableCollection<BridgeViewModel>(Bridges.Where(b => b.IsSelected).ToList());
        var selectedBridgePhotos = new Dictionary<string, List<BridgePhoto>>();

        foreach (var bridge in SelectedBridges)
        {
            var bridgePhotos = new List<BridgePhoto>();
            foreach (var defect in bridge.Defects)
            {
                foreach (var photo in defect.Photos)
                {
                    bridgePhotos.Add(new BridgePhoto
                    {
                        FilePath = photo.FilePath,
                    });
                }
            }
            selectedBridgePhotos.Add(bridge.Name, bridgePhotos);
        }

        return selectedBridgePhotos;
    }
    public class BridgePhoto
    {
        public string FilePath { get; set; }    //包含了文件名
    }
}
