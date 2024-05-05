using BridgeInspectionApp.Views;
using System.Diagnostics;

namespace BridgeInspectionApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
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



