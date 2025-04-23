using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Services
{
    public class MauiPlatformService : IPlatformService
    {
        public bool IsNativeApp() => true;
    }

}
