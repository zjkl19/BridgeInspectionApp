
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
        _viewModel.LoadBridgesCommand.Execute(null); // 确保页面每次显示时都重新加载数据
    }


    private async void OnEditClicked(object sender, EventArgs e)
    {
        //var button = sender as Button;
        //var bridgeViewModel = button?.BindingContext as BridgeViewModel;
        //if (bridgeViewModel != null)
        //{
        //    // 假设存在一个编辑桥梁页面，我们需要传递一个 Bridge 对象，需要从 ViewModel 中获取
        //    await Navigation.PushAsync(new BridgeEditPage(bridgeViewModel.Bridge));
        //}
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        //bool answer = await DisplayAlert("删除确认", "删除桥梁将同时删除所有相关的病害和照片。此操作不可恢复，是否继续？", "是", "否");
        //if (answer)
        //{
        //    var button = sender as Button;
        //    var bridgeViewModel = button?.BindingContext as BridgeViewModel;
        //    if (bridgeViewModel != null)
        //    {
        //        _viewModel.DeleteBridgeCommand.Execute(bridgeViewModel);
        //    }
        //}
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