using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Sync.Application.Helpers;
using StriveUp.Sync.Application.Interfaces;
using StriveUp.Sync.Application.Models.Fitbit;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Xml.Serialization;

namespace StriveUp.Sync.Application.Services
{
    public class FitbitProvider : IHealthDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FitbitProvider> _logger;

        public FitbitProvider(IHttpClientFactory httpClient, ILogger<FitbitProvider> logger)
        {
            _httpClient = httpClient.CreateClient("FitbitClient");
            _logger = logger;
        }

        public async Task<List<CreateUserActivityDto>> GetUserActivitiesAsync(UserSynchroDto userSynchro, string token)
        {
            var result = new List<CreateUserActivityDto>();

            // 0. Activity types => Fetch once and store in a file/database/cache

            //var activityTypesUrl = "1/activities.json";
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //var activityTypesResponse = await _httpClient.GetAsync(activityTypesUrl);
            //if (!activityTypesResponse.IsSuccessStatusCode)
            //{
            //    var error = await activityTypesResponse.Content.ReadAsStringAsync();
            //    _logger.LogWarning($"Failed to get Fitbit activityTypes. Status: {activityTypesResponse.StatusCode}. Response: {error}");
            //    return result;
            //}
            //var activityTypesJson = await activityTypesResponse.Content.ReadAsStringAsync();
            //var parsedJson = JsonDocument.Parse(activityTypesJson);
            //var formattedJson = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //});
            //await File.WriteAllTextAsync("fitbit-activity-types.json", formattedJson);

            //1.Fetch activities for last 5 minutes

            var now = DateTime.UtcNow;
            var fiveMinutesAgo = now.AddMinutes(-6);
            var afterDate = Uri.EscapeDataString(fiveMinutesAgo.ToString("yyyy-MM-ddTHH:mm:ss"));

            var activitiesUrl = $"/1/user/-/activities/list.json?afterDate={afterDate}&sort=asc&offset=0&limit=10";
            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Remove("accept-language");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("accept-language", "en_GB");

            var activities = await _httpClient.GetFromJsonAsync<FitbitActivityResponse>(activitiesUrl);

            // 2. For each activity, create a basic activity DTO with activity metadata
            if (activities != null)
            {
                foreach (var activity in activities.Activities)
                {
                    var activityDto = new CreateUserActivityDto
                    {
                        UserId = userSynchro.UserId,
                        ActivityId = ActivityHelpers.MapFitbitActivityType(Convert.ToInt32(activity.ActivityTypeId)),
                        DateStart = activity.OriginalStartTime,
                        DateEnd = activity.OriginalStartTime.AddMilliseconds(activity.OriginalDuration),
                        Title = activity.ActivityName,
                        Description = "Synchronized from Fitbit",
                        isSynchronized = true,
                        SynchroId = activity.LogId.ToString(),
                        SynchroProviderName = "Fitbit",
                        IsManuallyAdded = activity.LogType == "manual",
                        AvarageSpeed = activity.Speed / 3.6,
                        AvarageHr = activity.AverageHeartRate,
                        ElevationGain = Convert.ToInt32(activity.ElevationGain),
                        Distance = (int)Math.Round(activity.Distance * 1000),
                        Route = new List<GeoPointDto>(),
                        HrData = new List<ActivityHrDto>(),
                        SpeedData = new List<ActivitySpeedDto>()
                    };

                    // 3. Fetch aggregated detailed data for this session time range
                    var activityTcx = await FetchActivityTcx(activity.LogId);

                    // 4. Enrich activityDto with the TCX data
                    EnrichActivityWithActivityTcx(activityDto, activityTcx);

                    result.Add(activityDto);
                }
            }
            _logger.LogInformation($"Fetched {result.Count} Fitbit activities for user {userSynchro.UserId}");
            return result;
        }

        private async Task<ActivityTcxResponse?> FetchActivityTcx(long activityLogId)
        {
            var activityTcxUrl = $"/1/user/-/activities/{activityLogId}.tcx";

            await using var stream = await _httpClient.GetStreamAsync(activityTcxUrl);
            var serializer = new XmlSerializer(typeof(ActivityTcxResponse));
            var activityTcx = serializer.Deserialize(stream) as ActivityTcxResponse;

            return activityTcx;
        }

        private void EnrichActivityWithActivityTcx(CreateUserActivityDto activityDto, ActivityTcxResponse activityTcx)
        {
            if (activityTcx?.Activities == null || !activityTcx.Activities.Any())
                return;

            var trackpoints = activityTcx.Activities
                .SelectMany(a => a.Laps)
                .Where(l => l.Track?.Trackpoints != null)
                .SelectMany(l => l.Track.Trackpoints)
                .OrderBy(tp => tp.Time)
                .ToList();

            if (!trackpoints.Any())
                return;

            foreach (var tp in trackpoints)
            {
                // Route
                GeoPointDto? currentPoint = null;
                if (tp.Position != null)
                {
                    currentPoint = new GeoPointDto
                    {
                        Latitude = tp.Position.LatitudeDegrees,
                        Longitude = tp.Position.LongitudeDegrees,
                        Timestamp = tp.Time
                    };
                    activityDto.Route.Add(currentPoint);
                }

                // Heart Rate
                if (tp.HeartRateBpm != null)
                {
                    activityDto.HrData.Add(new ActivityHrDto
                    {
                        HearthRateValue = tp.HeartRateBpm.Value,
                        TimeStamp = tp.Time
                    });
                }
            }
        }
    }
}