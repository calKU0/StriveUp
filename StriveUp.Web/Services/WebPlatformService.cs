using StriveUp.Shared.Interfaces;

namespace StriveUp.Web.Services
{
    public class WebPlatformService : IPlatformService
    {
        public bool IsNativeApp() => false;
    }
}