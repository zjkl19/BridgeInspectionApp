namespace BridgeInspectionApp.Views;
using Microsoft.Maui.Media;
public partial class PhotoManagementPage : ContentPage
{
    public PhotoManagementPage()
    {
        InitializeComponent();
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                // 保存或处理照片流
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
        }
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.PickPhotoAsync();
            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                // 保存或处理照片流
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PickPhotoAsync THREW: {ex.Message}");
        }
    }
}
