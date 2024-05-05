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
    public DefectsListPage(BridgeViewModel bridgeViewModel)
    {
        InitializeComponent();
        BindingContext = bridgeViewModel;  // ��ҳ�����������������Ϊ����� ViewModel

        // ������������Ӽ��ز������ݵ��߼������� bridgeViewModel �е���Ϣ
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
        // ȷ��ҳ���Ⱥ͸߶ȷ���
        if (width > 0 && height > 0)
        {
            // ����ҳ����ܸ߶ȼ�ȥ����Ԫ�ص�Ԥ��ռ�ÿռ䣬����̬���� CollectionView �ĸ߶�
            double otherElementsHeight = 200; // ��������Ԫ���ܹ�ռ��200���ظ߶�
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