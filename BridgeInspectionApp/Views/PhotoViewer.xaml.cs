namespace BridgeInspectionApp.Views;

// PhotoViewer.xaml.cs
public partial class PhotoViewer : ContentView
{
    public static readonly BindableProperty FilePathProperty = BindableProperty.Create(
        nameof(FilePath), typeof(string), typeof(PhotoViewer));

    public string FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public PhotoViewer()
    {
        InitializeComponent();
    }

    private async void OnImageTapped(object sender, EventArgs e)
    {
        var imagePage = new ImageFullScreenPage(FilePath);
        await Application.Current.MainPage.Navigation.PushModalAsync(imagePage);
    }
}
