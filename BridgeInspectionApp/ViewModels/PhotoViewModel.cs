using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BridgeInspectionApp.ViewModels;

public partial class PhotoViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid id;
    [ObservableProperty] 
    private Guid defectId;
    [ObservableProperty]
    private string? filePath;
    [ObservableProperty]
    private string? note;


    public PhotoViewModel()
    {

    }
    public PhotoViewModel(string filePath, string note = "")
    {
        this.filePath = filePath;
        this.note = note;
    }
}
