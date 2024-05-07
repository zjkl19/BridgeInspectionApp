using System;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.ViewModels;

namespace BridgeInspectionApp.Views;

public partial class DefectEditPage : ContentPage
{
    public DefectEditPage(DefectViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // 绑定到传入的ViewModel实例
    }
}