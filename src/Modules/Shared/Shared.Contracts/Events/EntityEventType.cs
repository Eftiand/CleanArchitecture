using System.Text.Json.Serialization;

namespace coaches.Modules.Shared.Contracts.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityEventType
{
    Created,
    Updated,
    Deleted
}
