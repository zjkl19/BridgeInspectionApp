using BridgeInspectionApp.Data;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using BridgeInspectionApp.Models;


namespace BridgeInspectionApp.Views;

public partial class DefectsListPage : ContentPage
{
    public ObservableCollection<DefectViewModel> Defects { get; set; } = new ObservableCollection<DefectViewModel>();

    public DefectsListPage()
    {
        InitializeComponent();
        LoadDefects();
        BindingContext = this;
    }
    public DefectsListPage(Guid bridgeId)
    {
        InitializeComponent();
        LoadDefectsForBridge(bridgeId);
        BindingContext = this;
    }

    private async void LoadDefectsForBridge(Guid bridgeId)
    {
        using var context = new BridgeContext();
        var defects = await context.Defects
                                   .Where(d => d.BridgeId == bridgeId)
                                   .Include(d => d.Photos)
                                   .ToListAsync();
        foreach (var defect in defects)
        {
            Defects.Add(new DefectViewModel
            {
                Id = defect.Id,
                ComponentPart = defect.ComponentPart,
                DefectType = defect.DefectType,
                DefectLocation = defect.DefectLocation,
                DefectSeverity = defect.DefectSeverity,
                Note = defect.Note,
                Photos = new ObservableCollection<PhotoViewModel>(defect.Photos.Select(p => new PhotoViewModel
                {
                    Id = p.Id,
                    FilePath = p.FilePath,
                    Note = p.Note
                }))
            });
        }
    }

    private async void LoadDefects()
    {
        //defectsCollection.ItemsSource = db.Defects.ToList();
        using var context = new BridgeContext();
        var defects = await context.Defects
                                           .Include(d => d.Photos)
                                           .ToListAsync();

        foreach (var defect in defects)
        {
            Defects.Add(new DefectViewModel
            {
                Id = defect.Id,
                ComponentPart = defect.ComponentPart,
                DefectType = defect.DefectType,
                DefectLocation = defect.DefectLocation,
                DefectSeverity = defect.DefectSeverity,
                Note = defect.Note,
                Photos = new ObservableCollection<PhotoViewModel>(defect.Photos.Select(p => new PhotoViewModel
                {
                    Id = p.Id,
                    FilePath = p.FilePath,
                    Note = p.Note
                }))
            });
        }
    }

    private async void OnEditDefectClicked(object sender, EventArgs e)
    {
        // ±à¼­²¡º¦Âß¼­
    }

    private async void OnDeleteDefectClicked(object sender, EventArgs e)
    {
        // É¾³ý²¡º¦Âß¼­
    }

    private async void OnAddDefectClicked(object sender, EventArgs e)
    {
        // ÐÂÔö²¡º¦Âß¼­
    }

    private void OnImageTapped(object sender, EventArgs e)
    {
        // Logic to handle image tap for enlarging image
        var image = sender as Image;
        if (image != null)
        {
            var source = image.Source as FileImageSource;
            if (source != null)
            {
                // Show enlarged image
                // This might involve navigation to a new page with the image in a larger view
                Console.WriteLine("Image tapped: " + source.File);
            }
        }
    }


    public class DefectViewModel
    {
        public Guid Id { get; set; }
        public string ComponentPart { get; set; }
        public string DefectType { get; set; }
        public string DefectLocation { get; set; }
        public string DefectSeverity { get; set; }
        public string Note { get; set; }
        public ObservableCollection<PhotoViewModel> Photos { get; set; }

        public DefectViewModel()
        {
               Photos = new ObservableCollection<PhotoViewModel>();
            // Populate Photos with paths to images
        }
        private void LoadPhotos(List<Photo> photos)
        {
            foreach (var photo in photos)
            {
                Photos.Add(new PhotoViewModel
                {
                    Id = photo.Id,
                    FilePath = photo.FilePath,
                    Note = photo.Note
                });
            }
        }
    }

    public class PhotoViewModel
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string Note { get; set; }
    }
}