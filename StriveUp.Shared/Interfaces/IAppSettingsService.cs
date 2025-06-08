namespace StriveUp.Shared.Interfaces
{
    public interface IAppSettingsService
    {
        void OpenAppSettings();

        void PromptUserToAllowBackgroundActivity();

        bool IsRealmeDevice();
    }
}