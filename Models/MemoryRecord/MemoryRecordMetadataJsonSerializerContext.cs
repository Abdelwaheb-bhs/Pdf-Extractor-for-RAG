using System.Text.Json.Serialization;

namespace Nexia.Memory;

[JsonSerializable(typeof(MemoryRecordMetadata))]
internal sealed partial class MemoryRecordMetadataJsonSerializerContext : JsonSerializerContext
{
}