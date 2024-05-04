
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
        _viewModel = new BridgeListViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadBridgesCommand.Execute(null); // ȷ��ҳ��ÿ����ʾʱ�����¼�������
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

    private async void OnManageDefectsClicked(object sender, EventArgs e)
    {
        //var button = sender as Button;
        //var bridgeViewModel = button?.BindingContext as BridgeViewModel;
        //if (bridgeViewModel != null)
        //{
        //    var defectsPage = new DefectsListPage(bridgeViewModel.Bridge);
        //    await Navigation.PushAsync(defectsPage);
        //}
    }
}