using BridgeInspectionApp.Views;

namespace BridgeInspectionApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }



    private async void SelectBridge_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BridgeSelectionPage());
    }

    private async void OnViewInfoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BridgeListViewPage()); 
    }
}



