using Logitar.Master.Domain.Shared;

namespace Logitar.Master.Infrastructure.Converters;

internal class DescriptionConverter : JsonConverter<DescriptionUnit>
{
  public override DescriptionUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return DescriptionUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, DescriptionUnit description, JsonSerializerOptions options)
  {
    writer.WriteStringValue(description.Value);
  }
}
