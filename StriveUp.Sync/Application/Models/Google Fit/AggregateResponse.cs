using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using System.Text.Json.Serialization;
using StriveUp.Sync.Application.Converters;

namespace StriveUp.Sync.Application.Models
{
    public class AggregateResponse
    {
        [JsonPropertyName("bucket")]
        public List<Bucket> Buckets { get; set; }
    }

    public class Bucket
    {
        [JsonPropertyName("startTimeMillis")]
        [JsonConverter(typeof(JsonStringToLongConverter))]
        public long StartTimeMillis { get; set; }

        [JsonPropertyName("endTimeMillis")]
        [JsonConverter(typeof(JsonStringToLongConverter))]
        public long EndTimeMillis { get; set; }

        [JsonPropertyName("dataset")]
        public List<Dataset> Datasets { get; set; }
    }

    public class Dataset
    {
        [JsonPropertyName("dataSourceId")]
        public string DataSourceId { get; set; }

        [JsonPropertyName("point")]
        public List<Point> Points { get; set; }
    }

    public class Point
    {
        [JsonPropertyName("startTimeNanos")]
        [JsonConverter(typeof(JsonStringToLongConverter))]
        public long StartTimeNanos { get; set; }

        [JsonPropertyName("endTimeNanos")]
        [JsonConverter(typeof(JsonStringToLongConverter))]
        public long EndTimeNanos { get; set; }

        [JsonPropertyName("value")]
        public List<Value> Values { get; set; }
    }

    public class Value
    {
        [JsonPropertyName("intVal")]
        public int IntVal { get; set; }

        [JsonPropertyName("fpVal")]
        public double FpVal { get; set; }

        [JsonPropertyName("stringVal")]
        public string StringVal { get; set; }
    }
}
