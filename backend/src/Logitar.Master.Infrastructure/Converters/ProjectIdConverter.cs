using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Infrastructure.Converters;

internal class ProjectIdConverter : JsonConverter<ProjectId>
{
  public override ProjectId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return ProjectId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, ProjectId projectId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(projectId.Value);
  }
}
