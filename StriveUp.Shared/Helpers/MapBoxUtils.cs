using StriveUp.Shared.DTOs;
using System.Globalization;
using System.Text;

namespace StriveUp.Shared.Helpers
{
    public static class MapboxUtils
    {
        public static string GetStaticMapUrl(List<GeoPointDto> route, string token, int width = 300, int height = 200, int padding = 40)
        {
            if (route == null || route.Count < 2)
                return "";

            var encoded = Encode(route);
            var path = $"path-5+ffa726-0.8({Uri.EscapeDataString(encoded)})";

            var start = route.First();
            var end = route.Last();

            //string startIconUrl = Uri.EscapeDataString("https://yourdomain.com/icons/start.png");
            //string finishIconUrl = Uri.EscapeDataString("https://yourdomain.com/icons/finish.png");

            //string startIconOverlay = $"url-{startIconUrl}({start.Longitude.ToString(CultureInfo.InvariantCulture)},{start.Latitude.ToString(CultureInfo.InvariantCulture)})";
            //string endIconOverlay = $"url-{finishIconUrl}({end.Longitude.ToString(CultureInfo.InvariantCulture)},{end.Latitude.ToString(CultureInfo.InvariantCulture)})";

            string startMarker = $"pin-s-a+285A98({start.Longitude.ToString(CultureInfo.InvariantCulture)},{start.Latitude.ToString(CultureInfo.InvariantCulture)})";
            string endMarker = $"pin-s-b+FF5722({end.Longitude.ToString(CultureInfo.InvariantCulture)},{end.Latitude.ToString(CultureInfo.InvariantCulture)})";

            string overlay = $"{path},{startMarker},{endMarker}";

            return $"https://api.mapbox.com/styles/v1/mapbox/streets-v12/static/{overlay}/auto/{width}x{height}?padding={padding}&access_token={token}";
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