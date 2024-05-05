using BridgeInspectionApp.ViewModels;

namespace BridgeInspectionApp.Views;

public partial class DefectAddPage : ContentPage
{
    public DefectAddPage(DefectViewModel defectViewModel, BridgeViewModel bridgeViewModel)
    {
        InitializeComponent();
        BindingContext = defectViewModel;
    }
}