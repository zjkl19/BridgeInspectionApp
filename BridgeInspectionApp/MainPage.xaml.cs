using BridgeInspectionApp.Views;

namespace BridgeInspectionApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnManagePhotosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PhotoManagementPage()); // 导航到照片管理页面
        }

        private async void OnRecordInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BridgeInfoPage()); // 导航到信息记录页面
        }

        private async void OnViewInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BridgeInfoViewPage()); // 假设您有一个用于查看信息的页面
        }
    }


}
