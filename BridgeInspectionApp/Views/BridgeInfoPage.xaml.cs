using BridgeInspectionApp.Models;

namespace BridgeInspectionApp.Views;

public partial class BridgeInfoPage : ContentPage
{
	public BridgeInfoPage()
	{
		InitializeComponent();
	}
    private async void OnSaveInfoClicked(object sender, EventArgs e)
    {
        var bridgeInfo = new BridgeInfo
        {
            Name = NameEntry.Text,
            ComponentPart = ComponentPartEntry.Text,
            DefectType = DefectTypeEntry.Text,
            DefectLocation = DefectLocationEntry.Text,
            DefectSeverity = DefectSeverityEntry.Text
        };

        // ���� bridgeInfo �������ݿ�򱾵ش洢
    }
}