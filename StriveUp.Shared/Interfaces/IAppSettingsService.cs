﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IAppSettingsService
    {
        void OpenAppSettings();

        void PromptUserToAllowBackgroundActivity();

        bool IsRealmeDevice();
    }
}