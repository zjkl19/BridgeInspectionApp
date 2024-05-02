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
            // ���������Ƭ��
        }
    }

    private async void PickPhoto_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.PickPhotoAsync();
        if (photo != null)
        {
            // ���������Ƭ��
        }
    }

    private async void SaveInfo_Clicked(object sender, EventArgs e)
    {
        // ��������ȱ����Ϣ
        await DisplayAlert("����ɹ�", "����ȱ����Ϣ�ѱ��档", "OK");
    }
}