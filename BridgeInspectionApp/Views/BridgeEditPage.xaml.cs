using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using BridgeInspectionApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BridgeInspectionApp.Views;

public partial class BridgeEditPage : ContentPage
{
    public BridgeEditPage(BridgeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;  // 设置绑定上下文为传入的 ViewModel
    }

    //private async void OnSaveClicked(object sender, EventArgs e)
    //{
    //    var bridge = BindingContext as Bridge;
    //    if (bridge != null)
    //    {
    //        using var db = new BridgeContext();
    //        // 检查是否存在具有相同名称但不同ID的桥梁
    //        bool nameExists = await db.Bridges.AnyAsync(b => b.Name == bridge.Name && b.Id != bridge.Id);
    //        if (nameExists)
    //        {
    //            await DisplayAlert("错误", "已存在同名的桥梁，请使用不同的名称。", "OK");
    //            return;
    //        }

    //        // 更新数据库中的桥梁信息
    //        db.Bridges.Update(bridge);
    //        await db.SaveChangesAsync();
    //        await DisplayAlert("成功", "桥梁信息已更新", "OK");
    //        // 返回上一页
    //        await Navigation.PopAsync();
    //    }
    //}
}