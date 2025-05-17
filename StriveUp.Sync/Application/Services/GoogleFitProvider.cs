using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Sync.Application.Helpers;
using StriveUp.Sync.Application.Interfaces;
using StriveUp.Sync.Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StriveUp.Sync.Application.Services
{
    public class GoogleFitProvider : IHealthDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleFitProvider> _logger;
        private readonly ITokenService _tokenService;

        private static readonly HashSet<string> AllowedDataTypes = new()
        {
            "com.google.activity.segment",
            "com.google.speed",
            "com.google.heart_rate.bpm",
            "com.google.location.sample",
            "com.google.calories.expended",
            "com.google.distance.delta"
        };

        public GoogleFitProvider(HttpClient httpClient, ILogger<GoogleFitProvider> logger, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<List<CreateUserActivityDto>> GetUserActivitiesAsync(UserSynchroDto userSynchro)
        {
            var result = new List<CreateUserActivityDto>();

            var tokenResult = await _tokenService.GetAccessTokenAsync(userSynchro);
            if (string.IsNullOrEmpty(tokenResult?.AccessToken))
            {
                _logger.LogWarning($"Failed to get access token for user {userSynchro.UserId}");
                return result;
            }

            // 1. Fetch sessions for last 5 minutes
            var now = DateTime.UtcNow;
            var fiveMinutesAgo = now.AddMinutes(-5).AddDays(-6);

            var sessionsUrl = $"users/me/sessions?startTime={fiveMinutesAgo:O}&endTime={now:O}";
            var sessionsRequest = new HttpRequestMessage(HttpMethod.Get, sessionsUrl);
            sessionsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);

            var sessionsResponse = await _httpClient.SendAsync(sessionsRequest);
            if (!sessionsResponse.IsSuccessStatusCode)
            {
                var error = await sessionsResponse.Content.ReadAsStringAsync();
                _logger.LogWarning($"Failed to get sessions for user {userSynchro.UserId}. Status: {sessionsResponse.StatusCode}. Response: {error}");
                return result;
            }

            var sessionsJson = await sessionsResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(sessionsJson);

            if (!doc.RootElement.TryGetProperty("session", out var sessions))
            {
                _logger.LogInformation($"No sessions found for user {userSynchro.UserId} in last 5 minutes");
                return result;
            }

            // 2. For each session, create a basic activity DTO with session metadata
            foreach (var session in sessions.EnumerateArray())
            {
                var sessionId = session.GetProperty("id").GetString();
                var sessionName = session.TryGetProperty("name", out var nameProp)
                    ? nameProp.GetString()
                    : "Unnamed Session";

                var startTimeMillis = long.Parse(session.GetProperty("startTimeMillis").GetString());
                var endTimeMillis = long.Parse(session.GetProperty("endTimeMillis").GetString());

                var startTime = DateTimeOffset.FromUnixTimeMilliseconds(startTimeMillis).UtcDateTime;
                var endTime = DateTimeOffset.FromUnixTimeMilliseconds(endTimeMillis).UtcDateTime;

                var activityDto = new CreateUserActivityDto
                {
                    UserId = userSynchro.UserId,
                    ActivityId = ActivityHelpers.MapActivityType(session.GetProperty("activityType").GetInt32()),
                    DateStart = startTime,
                    DateEnd = endTime,
                    Title = sessionName,
                    Description = $"Synchronized from Google Fit" + Environment.NewLine + session.GetProperty("description").GetString(),
                    isSynchronized = true,
                    SynchroId = sessionId,
                    IsManuallyAdded = false,
                    Route = new List<GeoPointDto>(),
                    HrData = new List<ActivityHrDto>(),
                    SpeedData = new List<ActivitySpeedDto>()
                };

                // 3. Fetch aggregated detailed data for this session time range
                var aggregatedData = await FetchAggregatedDataAsync(tokenResult.AccessToken, startTimeMillis, endTimeMillis);

                // 4. Enrich activityDto with the aggregated data
                EnrichActivityWithAggregatedData(activityDto, aggregatedData);

                result.Add(activityDto);
            }

            _logger.LogInformation($"Fetched {result.Count} Google Fit sessions for user {userSynchro.UserId}");
            return result;
        }

        // Helper method to fetch aggregated data grouped by dataSourceId for a given time range
        private async Task<AggregateResponse> FetchAggregatedDataAsync(string accessToken, long startTimeMillis, long endTimeMillis)
        {
            // Fetch dataSources first to get dataSourceIds
            var dataSourcesRequest = new HttpRequestMessage(HttpMethod.Get, "users/me/dataSources");
            dataSourcesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var dataSourcesResponse = await _httpClient.SendAsync(dataSourcesRequest);
            if (!dataSourcesResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to get data sources while fetching aggregated data. Status: {dataSourcesResponse.StatusCode}");
                return null;
            }

            var dataSourcesJson = await dataSourcesResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(dataSourcesJson);

            if (!doc.RootElement.TryGetProperty("dataSource", out var dataSources))
                return null;

            var aggregateBy = new List<object>();

            foreach (var dataSource in dataSources.EnumerateArray())
            {
                if (dataSource.TryGetProperty("dataType", out var dataTypeElement)
                    && dataTypeElement.TryGetProperty("name", out var dataTypeNameElement))
                {
                    var dataTypeName = dataTypeNameElement.GetString();
                    if (AllowedDataTypes.Contains(dataTypeName))
                    {
                        if (dataSource.TryGetProperty("dataStreamId", out var dataSourceIdElement))
                        {
                            var dataSourceId = dataSourceIdElement.GetString();
                            aggregateBy.Add(new { dataSourceId });
                        }
                    }
                }
            }

            if (aggregateBy.Count == 0) return null;

            var requestBody = new
            {
                aggregateBy,
                startTimeMillis,
                endTimeMillis
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "users/me/dataset:aggregate")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Failed to get aggregated data. Status: {response.StatusCode}. Response: {errorContent}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var aggregatedData = JsonSerializer.Deserialize<AggregateResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return aggregatedData;
        }

        // Helper method to enrich your DTO with aggregated data from Google Fit response
        private void EnrichActivityWithAggregatedData(CreateUserActivityDto activity, AggregateResponse aggregatedData)
        {
            if (aggregatedData?.Buckets == null) return;

            foreach (var bucket in aggregatedData.Buckets)
            {
                foreach (var dataset in bucket.Datasets)
                {
                    foreach (var point in dataset.Points)
                    {
                        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(point.StartTimeNanos / 1_000_000).UtcDateTime;

                        if (dataset.DataSourceId.Contains("heart_rate"))
                        {
                            foreach (var val in point.Values)
                            {
                                activity.HrData.Add(new ActivityHrDto
                                {
                                    HearthRateValue = val.IntVal,
                                    TimeStamp = timestamp
                                });
                            }
                        }
                        else if (dataset.DataSourceId.Contains("speed"))
                        {
                            foreach (var val in point.Values)
                            {
                                activity.SpeedData.Add(new ActivitySpeedDto
                                {
                                    SpeedValue = val.FpVal,
                                    TimeStamp = timestamp
                                });
                            }
                        }
                        else if (dataset.DataSourceId.Contains("location"))
                        {
                            if (point.Values.Count >= 2)
                            {
                                activity.Route.Add(new GeoPointDto
                                {
                                    Latitude = point.Values[0].FpVal,
                                    Longitude = point.Values[1].FpVal,
                                    Timestamp = timestamp
                                });
                            }
                        }
                        else if (dataset.DataSourceId.Contains("distance"))
                        {
                            foreach (var val in point.Values)
                            {
                                activity.Distance += (int)val.FpVal;
                            }
                        }
                    }
                }
            }
        }
    }
}
