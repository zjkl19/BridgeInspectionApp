namespace BridgeInspectionApp.Views;

public partial class ImageFullScreenPage : ContentPage
{
    public string ImagePath { get; }

    public ImageFullScreenPage(string imagePath)
    {
        InitializeComponent();
        ImagePath = imagePath;
        BindingContext = this;

        // �������ҳ��ر�
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await Navigation.PopModalAsync();
        Content.GestureRecognizers.Add(tapGesture);
    }
}
