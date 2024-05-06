namespace BridgeInspectionApp.Views;

public partial class ImageFullScreenPage : ContentPage
{
    public string ImagePath { get; }

    public ImageFullScreenPage(string imagePath)
    {
        InitializeComponent();
        ImagePath = imagePath;
        BindingContext = this;

        // 点击整个页面关闭
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await Navigation.PopModalAsync();
        Content.GestureRecognizers.Add(tapGesture);
    }
}
