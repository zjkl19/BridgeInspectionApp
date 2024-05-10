
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Linq;
namespace BridgeInspectionApp.Views;

public partial class BridgeListViewPage : ContentPage
{
    private BridgeListViewModel _viewModel;

    public BridgeListViewPage()
    {
        InitializeComponent();
        BindingContext = new BridgeListViewModel();
    }


    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = BindingContext as BridgeListViewModel;
        viewModel?.FilterBridges(e.NewTextValue);
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

            bridgesCollection.HeightRequest = collectionViewHeight;
        }
    }

}