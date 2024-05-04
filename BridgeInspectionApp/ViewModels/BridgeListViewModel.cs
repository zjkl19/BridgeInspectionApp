using BridgeInspectionApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BridgeInspectionApp.ViewModels;

public class BridgeListViewModel
{
    public ICommand AddBridgeCommand { get; }

    public BridgeListViewModel()
    {
        AddBridgeCommand = new Command(async () => await ExecuteAddBridgeCommand());
    }

    private async Task ExecuteAddBridgeCommand()
    {
        // 这里假设你有一个 NavigationService 来处理页面导航，或者你可以在这里直接使用 Shell.Current.GoToAsync 等方法
        await Application.Current.MainPage.Navigation.PushAsync(new BridgeAddPage());
    }
}
