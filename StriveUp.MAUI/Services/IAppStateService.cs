using StriveUp.Shared.Interfaces;

namespace StriveUp.MAUI.Services
{
    public class AppStateService : IAppStateService
    {
        private bool _isInBackground;

        public bool IsInBackground => _isInBackground;

        public void SetBackgroundState(bool isInBackground)
        {
            _isInBackground = isInBackground;
        }
    }
}