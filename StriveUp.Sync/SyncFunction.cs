using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StriveUp.Sync.Application.Interfaces;

namespace StriveUp.Sync
{
    public class SyncFunction
    {
        private readonly ILogger _logger;
        private readonly IUserSyncService _userSyncService;

        public SyncFunction(ILoggerFactory loggerFactory, IUserSyncService userSyncService)
        {
            _logger = loggerFactory.CreateLogger<SyncFunction>();
            _userSyncService = userSyncService;
        }

        [Function("UserSyncFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _userSyncService.SyncUsersAsync();
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}