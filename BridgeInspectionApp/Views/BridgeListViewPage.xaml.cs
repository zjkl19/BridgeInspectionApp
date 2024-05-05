
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

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    _viewModel.LoadBridgesCommand.Execute(null);
    //}

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        // 确保页面宽度和高度非零
        if (width > 0 && height > 0)
        {
            // 根据页面的总高度减去其他元素的预估占用空间，来动态设置 CollectionView 的高度
            double otherElementsHeight = 200; // 假设其他元素总共占用200像素高度
            double collectionViewHeight = height - otherElementsHeight;

            bridgesCollection.HeightRequest = collectionViewHeight;
        }
    }

}