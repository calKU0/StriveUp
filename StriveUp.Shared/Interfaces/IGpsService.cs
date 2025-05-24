using Shiny.Locations;

namespace StriveUp.Shared.Interfaces
{
    public interface IGpsService
    {
        event Action<GpsReading> ReadingReceived;
    }
}