using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Authentication;

namespace StriveUp.MAUI.Platforms.Android
{
    [Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "striveup",
        DataHost = "login-callback"
    )]
    public class StriveUpWebAuthCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}