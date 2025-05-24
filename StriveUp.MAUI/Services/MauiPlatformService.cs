using StriveUp.Shared.Interfaces;

namespace StriveUp.MAUI.Services
{
    public class MauiPlatformService : IPlatformService
    {
        public bool IsNativeApp() => true;
    }
}