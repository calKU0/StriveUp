using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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