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

        // 保存 bridgeInfo 对象到数据库或本地存储
    }
}