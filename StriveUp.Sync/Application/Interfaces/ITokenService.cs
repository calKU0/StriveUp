using StriveUp.Shared.DTOs;
using StriveUp.Sync.Application.Services;

namespace StriveUp.Sync.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GetAccessTokenAsync(UserSynchroDto user);
}