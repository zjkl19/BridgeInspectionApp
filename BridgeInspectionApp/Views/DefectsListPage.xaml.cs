using BridgeInspectionApp.Data;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.ViewModels;


namespace BridgeInspectionApp.Views;

public partial class DefectsListPage : ContentPage
{
    public string BridgeName { get; private set; }
    public Guid BridgeId { get; private set; }
    public ObservableCollection<DefectViewModel> Defects { get; set; } = new ObservableCollection<DefectViewModel>();

    public DefectsListPage()
    {
        InitializeComponent();
        //LoadDefects();
        BindingContext = this;
    }
    public DefectsListPage(Guid bridgeId)
    {
        InitializeComponent();
        //LoadDefectsForBridge(bridgeId);
        BindingContext = this;
    }
    public DefectsListPage(Guid bridgeId, string bridgeName)
    {
        InitializeComponent();
        BridgeId = bridgeId;
        BridgeName = bridgeName;
        //LoadDefectsForBridge(bridgeId);
        BindingContext = this;
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        // 确保页面宽度和高度非零
        if (width > 0 && height > 0)
        {
            // 根据页面的总高度减去其他元素的预估占用空间，来动态设置 CollectionView 的高度
            double otherElementsHeight = 200; // 假设其他元素总共占用200像素高度
            double collectionViewHeight = height - otherElementsHeight;

            defectsCollection.HeightRequest = collectionViewHeight;
        }
    }
    //private async void LoadDefectsForBridge(Guid bridgeId)
    //{
    //    using var context = new BridgeContext();
    //    var defects = await context.Defects
    //                               .Where(d => d.BridgeId == bridgeId)
    //                               .Include(d => d.Photos)
    //                               .ToListAsync();
    //    foreach (var defect in defects)
    //    {
    //        Defects.Add(new DefectViewModel
    //        {
    //            Id = defect.Id,
    //            ComponentPart = defect.ComponentPart,
    //            DefectType = defect.DefectType,
    //            DefectLocation = defect.DefectLocation,
    //            DefectSeverity = defect.DefectSeverity,
    //            Note = defect.Note,
    //            Photos = new ObservableCollection<PhotoViewModel>(defect.Photos.Select(p => new PhotoViewModel
    //            {
    //                Id = p.Id,
    //                FilePath = p.FilePath,
    //                Note = p.Note
    //            }))
    //        });
    //    }
    //}

    //private async void LoadDefects()
    //{
    //    //defectsCollection.ItemsSource = db.Defects.ToList();
    //    using var context = new BridgeContext();
    //    var defects = await context.Defects
    //                                       .Include(d => d.Photos)
    //                                       .ToListAsync();

    //    foreach (var defect in defects)
    //    {
    //        Defects.Add(new DefectViewModel
    //        {
    //            Id = defect.Id,
    //            ComponentPart = defect.ComponentPart,
    //            DefectType = defect.DefectType,
    //            DefectLocation = defect.DefectLocation,
    //            DefectSeverity = defect.DefectSeverity,
    //            Note = defect.Note,
    //            Photos = new ObservableCollection<PhotoViewModel>(defect.Photos.Select(p => new PhotoViewModel
    //            {
    //                Id = p.Id,
    //                FilePath = p.FilePath,
    //                Note = p.Note
    //            }))
    //        });
    //    }
    //}

    private async void OnEditDefectClicked(object sender, EventArgs e)
    {
        // 编辑病害逻辑
    }

    private async void OnDeleteDefectClicked(object sender, EventArgs e)
    {
        // 删除病害逻辑
    }

    private async void OnAddDefectClicked(object sender, EventArgs e)
    {
        // 新增病害逻辑
    }

    private void OnImageTapped(object sender, EventArgs e)
    {
        // Logic to handle image tap for enlarging image
        var image = sender as Image;
        if (image != null)
        {
            var source = image.Source as FileImageSource;
            if (source != null)
            {
                // Show enlarged image
                // This might involve navigation to a new page with the image in a larger view
                Console.WriteLine("Image tapped: " + source.File);
            }
        }
    }


    
}