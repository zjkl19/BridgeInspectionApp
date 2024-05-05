using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using BridgeInspectionApp.Data;
using System.Threading.Tasks;
using BridgeInspectionApp.Models;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
namespace BridgeInspectionApp.ViewModels;

public partial class DefectViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid id;
    [ObservableProperty]
    private Defect defect;
    [ObservableProperty]
    private string? componentPart;
    [ObservableProperty]
    private string? defectType;
    [ObservableProperty]
    private string? defectLocation;
    [ObservableProperty]
    private string? defectSeverity;
    [ObservableProperty]
    private string? note;

    public ObservableCollection<Photo> Photos { get; set; }

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public DefectViewModel(Defect defect)
    {
        defect ??= new Defect();
        Id= defect.Id;
        Photos = new ObservableCollection<Photo>(defect.Photos ?? new List<Photo>());
        componentPart = defect?.ComponentPart;
        defectType = defect?.DefectType;
        defectLocation = defect?.DefectLocation;
        defectSeverity = defect?.DefectSeverity;
        note = defect?.Note;
        SaveCommand = new Command(async () => await SaveDefect());
        DeleteCommand = new Command<DefectViewModel>(async (viewModel) => await ExecuteDeleteCommand(viewModel));
    }
    private async Task ExecuteDeleteCommand(DefectViewModel viewModel)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "删除确认",
            "删除病害将同时删除所有相关的照片。此操作不可恢复，是否继续？",
            "是",
            "否");

        if (isConfirmed)
        {
            using var db = new BridgeContext();
            var defectToDelete = await db.Defects.Include(d => d.Photos)
                                                 .FirstOrDefaultAsync(d => d.Id == viewModel.Id);

            if (defectToDelete != null)
            {
                // 删除关联的照片文件
                foreach (var photo in defectToDelete.Photos)
                {
                    var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FilePath);
                    if (File.Exists(photoPath))
                    {
                        File.Delete(photoPath);
                    }
                }

                db.Defects.Remove(defectToDelete);
                await db.SaveChangesAsync();
                await Application.Current.MainPage.DisplayAlert("成功", "病害及其照片已成功删除。", "OK");
                WeakReferenceMessenger.Default.Send(new Messages.DefectDeletedMessage(Id));
            }
        }
    }
    private async Task SaveDefect()
    {
        using (var db = new BridgeContext())
        {
            var existingDefect = await db.Defects.FindAsync(defect.Id);
            if (existingDefect == null)
            {
                db.Defects.Add(defect);
            }
            else
            {
                db.Entry(existingDefect).CurrentValues.SetValues(defect);
            }
            await db.SaveChangesAsync();
        }
    }

    private async Task DeleteDefect()
    {
        using (var db = new BridgeContext())
        {
            var existingDefect = await db.Defects.FindAsync(defect.Id);
            if (existingDefect != null)
            {
                db.Defects.Remove(existingDefect);
                await db.SaveChangesAsync();
            }
        }
    }
}
