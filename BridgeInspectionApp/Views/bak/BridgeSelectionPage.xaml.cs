using System.Collections.ObjectModel;

namespace BridgeInspectionApp.Views;

public partial class BridgeSelectionPage : ContentPage
{
    public ObservableCollection<string> Bridges { get; set; }

    public BridgeSelectionPage()
    {
        InitializeComponent();
        Bridges = new ObservableCollection<string>() { "����1", "����2" }; // ʾ������
        bridgeList.ItemsSource = Bridges;
    }

    private async void BridgeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedBridge = e.CurrentSelection[0] as string;
        if (selectedBridge != null)
        {
            // �洢ѡ�е��������򴫵ݸ���һ��ҳ��
            await Navigation.PushAsync(new BridgeInfoPage(selectedBridge));
        }
    }

    private async void AddNewBridge_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("������", "����������������:");
        if (!string.IsNullOrEmpty(result))
        {
            Bridges.Add(result);
        }
    }
}