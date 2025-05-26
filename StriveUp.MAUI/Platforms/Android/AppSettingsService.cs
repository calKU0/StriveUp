using Android.Content;
using StriveUp.MAUI.Platforms.Android;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;
using AndroidDevice = Android;

[assembly: Dependency(typeof(AppSettingsService))]

namespace StriveUp.MAUI.Platforms.Android
{
    public class AppSettingsService : IAppSettingsService
    {
        public void OpenAppSettings()
        {
            var intent = new Intent(AndroidDevice.Provider.Settings.ActionApplicationDetailsSettings);
            intent.SetData(AndroidDevice.Net.Uri.Parse("package:" + AndroidApp.Context.PackageName));
            intent.SetFlags(ActivityFlags.NewTask);
            AndroidApp.Context.StartActivity(intent);
        }
    }
}