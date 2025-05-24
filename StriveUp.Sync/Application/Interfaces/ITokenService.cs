using StriveUp.Shared.DTOs;
using StriveUp.Sync.Application.Models;

namespace StriveUp.Sync.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GetAccessTokenAsync(UserSynchroDto user);
}