using System.Collections.ObjectModel;

namespace BridgeInspectionApp.Views;

public partial class BridgeSelectionPage : ContentPage
{
    public ObservableCollection<string> Bridges { get; set; }

    public BridgeSelectionPage()
    {
        InitializeComponent();
        Bridges = new ObservableCollection<string>() { "桥梁1", "桥梁2" }; // 示例数据
        bridgeList.ItemsSource = Bridges;
    }

    private async void BridgeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedBridge = e.CurrentSelection[0] as string;
        if (selectedBridge != null)
        {
            // 存储选中的桥梁名或传递给下一个页面
            await Navigation.PushAsync(new BridgeInfoPage(selectedBridge));
        }
    }

    private async void AddNewBridge_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("新桥梁", "输入新桥梁的名称:");
        if (!string.IsNullOrEmpty(result))
        {
            Bridges.Add(result);
        }
    }
}