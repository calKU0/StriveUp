﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
