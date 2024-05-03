using BridgeInspectionApp.Data;

namespace BridgeInspectionApp.Views;

public partial class DefectsListPage : ContentPage
{

    public DefectsListPage()
    {
        InitializeComponent();
        LoadDefects();
    }

    private void LoadDefects()
    {
        using var db = new BridgeContext();
        defectsCollection.ItemsSource = db.Defects.ToList();
    }

    private async void OnEditDefectClicked(object sender, EventArgs e)
    {
        // ±à¼­²¡º¦Âß¼­
    }

    private async void OnDeleteDefectClicked(object sender, EventArgs e)
    {
        // É¾³ý²¡º¦Âß¼­
    }

    private async void OnAddDefectClicked(object sender, EventArgs e)
    {
        // ÐÂÔö²¡º¦Âß¼­
    }
}