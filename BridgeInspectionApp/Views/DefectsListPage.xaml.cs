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
        // �༭�����߼�
    }

    private async void OnDeleteDefectClicked(object sender, EventArgs e)
    {
        // ɾ�������߼�
    }

    private async void OnAddDefectClicked(object sender, EventArgs e)
    {
        // ���������߼�
    }
}