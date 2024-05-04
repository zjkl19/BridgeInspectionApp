
using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
namespace BridgeInspectionApp.Views;

public partial class BridgeAddPage : ContentPage
{
    public ICommand AddBridgeCommand { get; private set; }

    public BridgeAddPage()
    {
        InitializeComponent();
        AddBridgeCommand = new Command(async () => await ExecuteAddBridgeCommand());
        BindingContext = this;
    }

    private async Task ExecuteAddBridgeCommand()
    {
        var bridgeName = bridgeNameEntry.Text;
        var bridgeLocation = bridgeLocationEntry.Text;

        if (string.IsNullOrWhiteSpace(bridgeName))
        {
            await DisplayAlert("����", "�������Ʊ�����д��", "OK");
            return;
        }

        using (var db = new BridgeContext())
        {
            // ������ݿ����Ƿ��Ѵ���ͬ������
            var existingBridge = await db.Bridges
                                         .FirstOrDefaultAsync(b => b.Name == bridgeName);
            if (existingBridge != null)
            {
                await DisplayAlert("����", "���������Ѵ��ڣ���ʹ�ò�ͬ�����ơ�", "OK");
                return;
            }

            var newBridge = new Bridge
            {
                Name = bridgeName,
                Location = bridgeLocation
            };

            db.Bridges.Add(newBridge);
            await db.SaveChangesAsync();
        }

        await DisplayAlert("�ɹ�", "�����ѳɹ���ӡ�", "OK");
        await Navigation.PopAsync();
    }

}