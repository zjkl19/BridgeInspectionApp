using BridgeInspectionApp.Views;
using System.Diagnostics;

namespace BridgeInspectionApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnNoticeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("注意事项", 
            "1、照片的存放路径是在内部存储设备中“桥梁巡查”目录，这个地方照片不能删除，不能随意改名字。\n"+
            "2、单击照片可以放大查看。\n"+
            "3、在搜索框中输入关键词可以过滤信息",
            "OK");
    }
    private async void OnViewInfoClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new BridgeListViewPage());
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            Application.Current.MainPage.DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }
    }
}



