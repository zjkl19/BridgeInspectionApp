using BridgeInspectionApp.Data;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.ViewModels;


namespace BridgeInspectionApp.Views;

public partial class DefectsListPage : ContentPage
{
    
    public Guid BridgeId { get; private set; }
    public ObservableCollection<DefectViewModel> Defects { get; set; } = new ObservableCollection<DefectViewModel>();

    public DefectsListPage()
    {

    }
    public DefectsListPage(BridgeViewModel bridgeViewModel)
    {
        InitializeComponent();
        BindingContext =new DefectsListViewModel(bridgeViewModel);  
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