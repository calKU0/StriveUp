using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.ApplicationModel;

namespace StriveUp.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize |
                           ConfigChanges.Orientation |
                           ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize |
                           ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FFA726"));
        }

        RequestBlePermissions();

#if DEBUG
        Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif
    }

    private async void RequestBlePermissions()
    {
        var statusScan = await Permissions.RequestAsync<BluetoothScanPermission>();
        var statusConnect = await Permissions.RequestAsync<BluetoothConnectPermission>();

        // You can check the statuses here and handle denial if needed
    }

    public class BluetoothScanPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[]
            {
                (Android.Manifest.Permission.BluetoothScan, true)
            };
    }

    public class BluetoothConnectPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[]
            {
                (Android.Manifest.Permission.AccessFineLocation, true),
                (Android.Manifest.Permission.BluetoothAdmin, true),
                (Android.Manifest.Permission.BluetoothConnect, true),
                (Android.Manifest.Permission.Bluetooth, true)
            };
    }
}
