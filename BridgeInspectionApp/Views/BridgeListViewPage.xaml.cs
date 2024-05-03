
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Linq;
namespace BridgeInspectionApp.Views;

public partial class BridgeListViewPage : ContentPage
{
    public BridgeListViewPage()
    {
        InitializeComponent();
        // ʹ���첽��������
        LoadBridgesAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadBridgesAsync(); // ���¼������������Ը���UI
    }
    private async Task LoadBridgesAsync()
    {
        try
        {
            using var db = new BridgeContext();
            var bridges = await db.Bridges.ToListAsync(); // ʹ���첽������������
            bridgesCollection.ItemsSource = bridges;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load bridges: {ex.Message}");
            await DisplayAlert("���ش���", $"�޷������������ݡ� {ex.Message}", "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bridge = button?.BindingContext as Bridge;
        if (bridge != null)
        {
            await Navigation.PushAsync(new BridgeEditPage(bridge));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("ɾ��ȷ��", "ɾ��������ͬʱɾ��������صĲ�������Ƭ���˲������ɻָ����Ƿ������", "��", "��");
        if (answer)
        {
            var button = sender as Button;
            var bridge = button?.BindingContext as Bridge;

            if (bridge != null)
            {
                using var db = new BridgeContext();
                var bridgeToDelete = db.Bridges.Include(b => b.Defects).ThenInclude(d => d.Photos).FirstOrDefault(b => b.Id == bridge.Id);

                if (bridgeToDelete != null)
                {
                    // ɾ�����й�����Ƭ�ļ�
                    foreach (var defect in bridgeToDelete.Defects)
                    {
                        foreach (var photo in defect.Photos)
                        {
                            var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FilePath);
                            if (File.Exists(photoPath))
                            {
                                File.Delete(photoPath);
                            }
                        }
                    }

                    // ɾ��������¼
                    db.Bridges.Remove(bridgeToDelete);
                    await db.SaveChangesAsync();

                    // ˢ�������б�
                    LoadBridgesAsync();
                }
            }
        }
    }


    private void OnManageDefectsClicked(object sender, EventArgs e)
    {
        // �������߼�
    }
}