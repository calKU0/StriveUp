using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Authentication;

namespace StriveUp.MAUI.Platforms.Android
{
    [Activity(NoHistory = true, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "StriveUp",
        DataHost = "login-callback"
    )]
    public class StriveUpWebAuthCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}