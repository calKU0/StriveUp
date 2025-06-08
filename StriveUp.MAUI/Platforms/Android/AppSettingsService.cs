using Android.Content;
using Android.OS;
using StriveUp.MAUI.Platforms.Android;
using StriveUp.Shared.Interfaces;
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

        public void PromptUserToAllowBackgroundActivity()
        {
            try
            {
                var intent = new Intent();
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetComponent(new ComponentName(
                    "com.coloros.oppoguardelf",
                    "com.coloros.powermanager.fuelgaue.PowerUsageModelActivity"
                ));
                AndroidApp.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open ColorOS battery settings, falling back to default: " + ex.Message);
                OpenDefaultAppSettings();
            }
        }

        private void OpenDefaultAppSettings()
        {
            try
            {
                var intent = new Intent(AndroidDevice.Provider.Settings.ActionApplicationDetailsSettings);
                intent.SetData(AndroidDevice.Net.Uri.Parse("package:" + AndroidApp.Context.PackageName));
                intent.AddFlags(ActivityFlags.NewTask);
                AndroidApp.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open app settings: " + ex.Message);
            }
        }

        public bool IsRealmeDevice()
        {
            return Build.Manufacturer.ToLower().Contains("realme");
        }
    }
}