using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.Data;
using System.Threading.Tasks;
using BridgeInspectionApp.Models;

namespace BridgeInspectionApp.ViewModels;

public class DefectViewModel : BaseViewModel
{
    private Defect _defect;
    private string? _componentPart;
    private string? _defectType;
    private string? _defectLocation;
    private string? _defectSeverity;
    private string? _note;

    public Guid Id
    {
        get => _defect.Id;
        set
        {
            if (_defect.Id != value)
            {
                _defect.Id = value;
                OnPropertyChanged();
            }
        }
    }

    public string? ComponentPart
    {
        get => _componentPart;
        set => SetProperty(ref _componentPart, value);
    }

    public string? DefectType
    {
        get => _defectType;
        set => SetProperty(ref _defectType, value);
    }

    public string? DefectLocation
    {
        get => _defectLocation;
        set => SetProperty(ref _defectLocation, value);
    }

    public string? DefectSeverity
    {
        get => _defectSeverity;
        set => SetProperty(ref _defectSeverity, value);
    }

    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public ObservableCollection<Photo> Photos { get; set; }

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public DefectViewModel(Defect defect)
    {
        _defect = defect ?? new Defect();
        Photos = new ObservableCollection<Photo>(_defect.Photos ?? new List<Photo>());
        _componentPart = defect.ComponentPart;
        _defectType = defect.DefectType;
        _defectLocation = defect.DefectLocation;
        _defectSeverity = defect.DefectSeverity;
        _note = defect.Note;
        SaveCommand = new Command(async () => await SaveDefect());
        DeleteCommand = new Command(async () => await DeleteDefect());
    }

    private async Task SaveDefect()
    {
        using (var db = new BridgeContext())
        {
            var existingDefect = await db.Defects.FindAsync(_defect.Id);
            if (existingDefect == null)
            {
                db.Defects.Add(_defect);
            }
            else
            {
                db.Entry(existingDefect).CurrentValues.SetValues(_defect);
            }
            await db.SaveChangesAsync();
        }
    }

    private async Task DeleteDefect()
    {
        using (var db = new BridgeContext())
        {
            var existingDefect = await db.Defects.FindAsync(_defect.Id);
            if (existingDefect != null)
            {
                db.Defects.Remove(existingDefect);
                await db.SaveChangesAsync();
            }
        }
    }
}
