using BridgeInspectionApp.Data;
using BridgeInspectionApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BridgeInspectionApp.Views;

public partial class BridgeEditPage : ContentPage
{
    public BridgeEditPage(Bridge bridge)
    {
        InitializeComponent();
        BindingContext = bridge;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var bridge = BindingContext as Bridge;
        if (bridge != null)
        {
            using var db = new BridgeContext();
            // ����Ƿ���ھ�����ͬ���Ƶ���ͬID������
            bool nameExists = await db.Bridges.AnyAsync(b => b.Name == bridge.Name && b.Id != bridge.Id);
            if (nameExists)
            {
                await DisplayAlert("����", "�Ѵ���ͬ������������ʹ�ò�ͬ�����ơ�", "OK");
                return;
            }

            // �������ݿ��е�������Ϣ
            db.Bridges.Update(bridge);
            await db.SaveChangesAsync();
            await DisplayAlert("�ɹ�", "������Ϣ�Ѹ���", "OK");
            // ������һҳ
            await Navigation.PopAsync();
        }
    }
}