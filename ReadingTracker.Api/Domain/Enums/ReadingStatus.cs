using System.Text.Json.Serialization;

namespace ReadingTracker.Api.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReadingStatus
    {
        QueroLer = 1,
        Lendo = 2,
        Lido = 3
    }
}
