using BridgeInspectionApp.Data;
using BridgeInspectionApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BridgeInspectionApp.ViewModels;

public partial class DefectsListViewModel : ObservableObject
{
    [ObservableProperty]
    public string bridgeName;
    private BridgeViewModel _bridgeViewModel;
    public ObservableCollection<DefectViewModel> Defects { get; set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand AddDefectCommand { get; private set; }
    public DefectsListViewModel()
    {

    }

    public DefectsListViewModel(BridgeViewModel bridgeViewModel)
    {
        bridgeName = bridgeViewModel.Name;
        _bridgeViewModel= bridgeViewModel;
        Defects = bridgeViewModel.Defects;
        RegisterMessages();
        AddDefectCommand = new Command(async () => await ExecuteAddDefectCommand());
    }
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<Messages.DefectDeletedMessage>(this, (recipient, message) =>
        {
            // 从集合中移除对应的 DefectViewModel
            var defectToRemove = Defects.FirstOrDefault(d => d.Id == message.DefectId);
            if (defectToRemove != null)
            {
                Defects.Remove(defectToRemove);
            }
        });

    }

    private async Task ExecuteAddDefectCommand()
    {
        var defectAddPage = new DefectAddPage(new DefectViewModel(new Models.Defect { BridgeId=_bridgeViewModel.Id }), _bridgeViewModel);
        await Application.Current.MainPage.Navigation.PushAsync(defectAddPage);
    }
    //private async Task ExecuteDeleteCommand(DefectViewModel defectViewModel)
    //{
    //    bool answer = await Application.Current.MainPage.DisplayAlert("删除确认", "删除病害将同时删除所有相关的照片。此操作不可恢复，是否继续？", "是", "否");
    //    if (answer)
    //    {
    //        using var db = new BridgeContext();
    //        var defectEntity = await db.Defects.Include(d => d.Photos).FirstOrDefaultAsync(d => d.Id == defectViewModel.Id);
    //        if (defectEntity != null)
    //        {
    //            // 删除关联的照片文件
    //            foreach (var photo in defectEntity.Photos)
    //            {
    //                var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FilePath);
    //                if (File.Exists(photoPath))
    //                {
    //                    File.Delete(photoPath);
    //                }
    //            }

    //            db.Defects.Remove(defectEntity);
    //            await db.SaveChangesAsync();

    //            // 更新 UI 或通知用户
    //            await Application.Current.MainPage.DisplayAlert("成功", "病害及其照片已成功删除。", "OK");
    //            // 可以添加逻辑来更新界面或返回上一层
    //        }
    //    }
    //}


}
