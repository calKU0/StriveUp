using System.Globalization;
using Microsoft.Maui.Devices.Sensors;
using System.Text;
using PolylineEncoder.Net.Models;
using PolylineEncoder.Net.Utility;
using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Helpers
{
    public static class MapboxUtils
    {
        public static string GetStaticMapUrl(List<GeoPointDto> route, string token, int width = 400, int height = 200)
        {
            if (route == null || route.Count < 2)
                return ""; // Or fallback image

            var encoded = Encode(route);
            var path = $"path-5+f44-0.5({Uri.EscapeDataString(encoded)})";

            return $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/{path}/auto/{width}x{height}?padding=40,40,40,40?access_token={token}";
        }


        public static string Encode(IEnumerable<GeoPointDto> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;
                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;
            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);
                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);
                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }
    }
}
