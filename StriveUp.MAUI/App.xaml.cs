using StriveUp.MAUI.Services;
using StriveUp.Shared.Interfaces;
using static StriveUp.MAUI.MauiProgram;

namespace StriveUp.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "StriveUp" };
        }

        protected override void OnStart()
        {
            SetBackground(false);
        }

        protected override void OnSleep()
        {
            SetBackground(true);
        }

        protected override void OnResume()
        {
            SetBackground(false);
        }

        private void SetBackground(bool isInBackground)
        {
            if (MauiServiceProvider.Services.GetService<IAppStateService>() is AppStateService service)
            {
                service.SetBackgroundState(isInBackground);
            }
        }
    }
}