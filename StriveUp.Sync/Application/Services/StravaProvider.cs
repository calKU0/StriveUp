using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Sync.Application.Helpers;
using StriveUp.Sync.Application.Interfaces;
using StriveUp.Sync.Application.Models.Fitbit;
using StriveUp.Sync.Application.Models.Strava;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StriveUp.Sync.Application.Services
{
    public class StravaProvider : IHealthDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StravaProvider> _logger;

        public StravaProvider(IHttpClientFactory httpClient, ILogger<StravaProvider> logger)
        {
            _httpClient = httpClient.CreateClient("StravaClient");
            _logger = logger;
        }

        public async Task<List<CreateUserActivityDto>> GetUserActivitiesAsync(UserSynchroDto userSynchro, string token)
        {
            var result = new List<CreateUserActivityDto>();

            var now = DateTime.UtcNow;
            var fiveMinutesAgo = now.AddMinutes(-6);
            var epochTimestamp = ((DateTimeOffset)fiveMinutesAgo).ToUnixTimeSeconds();

            var activitiesUrl = $"athlete/activities?after={epochTimestamp}&per_page=100";
            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Remove("accept-language");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("accept-language", "en_GB");

            var activities = await _httpClient.GetFromJsonAsync<List<StravaActivityResponse>>(activitiesUrl);

            // 2. For each activity, create a basic activity DTO with activity metadata
            if (activities != null)
            {
                foreach (var activity in activities)
                {
                    var activityDto = new CreateUserActivityDto
                    {
                        UserId = userSynchro.UserId,
                        ActivityId = ActivityHelpers.MapStravaActivityType(activity.SportType),
                        DateStart = activity.StartDate,
                        DateEnd = activity.StartDate.AddSeconds(activity.ElapsedTime),
                        Title = activity.Name,
                        Description = $"Synchronized from Strava",
                        SynchroProviderName = "Strava",
                        isSynchronized = true,
                        SynchroId = activity.Id.ToString(),
                        AvarageSpeed = activity.AverageSpeed,
                        MaxSpeed = activity.MaxSpeed,
                        MaxHr = Convert.ToInt32(activity.MaxHeartrate),
                        AvarageHr = Convert.ToInt32(activity.AverageHeartrate),
                        ElevationGain = Convert.ToInt32(activity.TotalElevationGain),
                        IsPrivate = activity.Private,
                        IsManuallyAdded = activity.Manual,
                        Distance = Convert.ToInt32(activity.Distance),
                        Route = new List<GeoPointDto>(),
                        HrData = new List<ActivityHrDto>(),
                        SpeedData = new List<ActivitySpeedDto>(),
                        ElevationData = new List<ActivityElevationDto>()
                    };

                    // 3. Fetch aggregated detailed data for this session time range
                    var streamData = await FetchActivityStreams(activity.Id, token);

                    // 4. Enrich activityDto with the TCX data
                    if (streamData != null)
                    {
                        EnrichActivityWithStreams(activityDto, streamData, activity.StartDate);
                    }

                    result.Add(activityDto);
                }
            }
            _logger.LogInformation($"Fetched {result.Count} Strava activities for user {userSynchro.UserId}");
            return result;
        }

        private async Task<StravaStreamSet?> FetchActivityStreams(long activityId, string token)
        {
            var streamTypes = new[]
            {
                "time", "latlng", "heartrate", "velocity_smooth", "altitude"
            };

            var streamUrl = $"activities/{activityId}/streams?keys={string.Join(",", streamTypes)}&key_by_type=true";

            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                return await _httpClient.GetFromJsonAsync<StravaStreamSet>(streamUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch stream data for activity {activityId}");
                return null;
            }
        }

        private void EnrichActivityWithStreams(
            CreateUserActivityDto activityDto,
            StravaStreamSet streamSet,
            DateTime activityStartTime)
        {
            var timeStream = streamSet.Time?.Data;
            var latLngStream = streamSet.LatLng?.Data;
            var heartRateStream = streamSet.Heartrate?.Data;
            var speedStream = streamSet.VelocitySmooth?.Data;
            var altitudeStream = streamSet.Altitude?.Data;

            for (int i = 0; i < (timeStream?.Count ?? 0); i++)
            {
                var timestamp = activityStartTime.AddSeconds(timeStream[i]);

                if (latLngStream != null && i < latLngStream.Count)
                {
                    activityDto.Route.Add(new GeoPointDto
                    {
                        Latitude = latLngStream[i][0],
                        Longitude = latLngStream[i][1],
                        Timestamp = timestamp
                    });
                }

                if (heartRateStream != null && i < heartRateStream.Count)
                {
                    activityDto.HrData.Add(new ActivityHrDto
                    {
                        HearthRateValue = heartRateStream[i],
                        TimeStamp = timestamp
                    });
                }

                if (speedStream != null && i < speedStream.Count)
                {
                    activityDto.SpeedData.Add(new ActivitySpeedDto
                    {
                        SpeedValue = speedStream[i],
                        TimeStamp = timestamp
                    });
                }

                if (altitudeStream != null && i < altitudeStream.Count)
                {
                    activityDto.ElevationData.Add(new ActivityElevationDto
                    {
                        ElevationValue = altitudeStream[i],
                        TimeStamp = timestamp
                    });
                }
            }
        }
    }
}