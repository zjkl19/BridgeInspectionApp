using BridgeInspectionApp.ViewModels;

namespace BridgeInspectionApp.Views;

public partial class BridgeAddPage : ContentPage
{
	public BridgeAddPage(BridgeViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}