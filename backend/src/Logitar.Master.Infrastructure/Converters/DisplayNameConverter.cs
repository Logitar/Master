using Logitar.Master.Domain.Shared;

namespace Logitar.Master.Infrastructure.Converters;

internal class DisplayNameConverter : JsonConverter<DisplayNameUnit>
{
  public override DisplayNameUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return DisplayNameUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, DisplayNameUnit displayName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(displayName.Value);
  }
}
