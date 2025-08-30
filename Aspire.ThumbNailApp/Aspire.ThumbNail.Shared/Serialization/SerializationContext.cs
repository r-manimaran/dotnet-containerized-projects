using System.Text.Json.Serialization;

namespace Aspire.ThumbNail.Shared.Serialization;
[JsonSerializable(typeof(UploadResult))]
public sealed partial class SerializationContext : JsonSerializerContext
{

}
