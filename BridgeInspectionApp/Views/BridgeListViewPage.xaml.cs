
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


    private async void OnEditClicked(object sender, EventArgs e)
    {
        //var button = sender as Button;
        //var bridgeViewModel = button?.BindingContext as BridgeViewModel;
        //if (bridgeViewModel != null)
        //{
        //    // �������һ���༭����ҳ�棬������Ҫ����һ�� Bridge ������Ҫ�� ViewModel �л�ȡ
        //    await Navigation.PushAsync(new BridgeEditPage(bridgeViewModel.Bridge));
        //}
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        //bool answer = await DisplayAlert("ɾ��ȷ��", "ɾ��������ͬʱɾ��������صĲ�������Ƭ���˲������ɻָ����Ƿ������", "��", "��");
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