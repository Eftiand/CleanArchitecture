using System.Text.Json.Serialization;

namespace coaches.Modules.Shared.Domain.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityEventType
{
    Created,
    Updated,
    Deleted
}
