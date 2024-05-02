using BridgeInspectionApp.Models;

namespace BridgeInspectionApp.Views;

public partial class BridgeInfoPage : ContentPage
{
    private string bridgeName;

    public BridgeInfoPage(string bridgeName)
    {
        InitializeComponent();
        this.bridgeName = bridgeName;
    }

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo != null)
        {
            // 保存或处理照片流
        }
    }

    private async void PickPhoto_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.PickPhotoAsync();
        if (photo != null)
        {
            // 保存或处理照片流
        }
    }

    private async void SaveInfo_Clicked(object sender, EventArgs e)
    {
        // 保存桥梁缺损信息
        await DisplayAlert("保存成功", "桥梁缺损信息已保存。", "OK");
    }
}