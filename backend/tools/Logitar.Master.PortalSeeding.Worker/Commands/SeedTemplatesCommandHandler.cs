using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal class SeedTemplatesCommandHandler : INotificationHandler<SeedTemplatesCommand>
{
  private static readonly Dictionary<string, string> _contentTypes = new()
  {
    [".html"] = MediaTypeNames.Text.Html,
    [".txt"] = MediaTypeNames.Text.Plain
  };

  private readonly ILogger<SeedTemplatesCommandHandler> _logger;
  private readonly ITemplateClient _templates;

  public SeedTemplatesCommandHandler(ILogger<SeedTemplatesCommandHandler> logger, ITemplateClient templates)
  {
    _logger = logger;
    _templates = templates;
  }

  public async Task Handle(SeedTemplatesCommand _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    SearchResults<Template> results = await _templates.SearchAsync(new SearchTemplatesPayload(), context);
    Dictionary<string, Template> templates = new(capacity: results.Items.Count);
    foreach (Template template in results.Items)
    {
      templates[template.UniqueKey] = template;
    }

    Dictionary<string, Content> contents = await LoadContentsAsync(cancellationToken);

    string json = await File.ReadAllTextAsync("templates.json", Encoding.UTF8, cancellationToken);
    IEnumerable<TemplateSummary>? summaries = JsonSerializer.Deserialize<IEnumerable<TemplateSummary>>(json);
    if (summaries != null)
    {
      foreach (TemplateSummary summary in summaries)
      {
        if (templates.TryGetValue(summary.UniqueKey, out Template? template))
        {
          _logger.LogInformation("The template '{UniqueKey}' already exists (Id={Id}).", template.UniqueKey, template.Id);
        }
        else
        {
          string subject = $"{summary.UniqueKey}_Subject";
          Content content = contents[summary.UniqueKey];
          CreateTemplatePayload payload = new(summary.UniqueKey, subject, content)
          {
            DisplayName = summary.DisplayName,
            Description = summary.Description
          };
          template = await _templates.CreateAsync(payload, context);
          templates[summary.UniqueKey] = template;
          _logger.LogInformation("The template '{UniqueKey}' has been created (Id={Id}).", template.UniqueKey, template.Id);
        }
      }
    }
  }

  private static async Task<Dictionary<string, Content>> LoadContentsAsync(CancellationToken cancellationToken)
  {
    string[] files = Directory.GetFiles("Templates");
    Dictionary<string, Content> contents = new(capacity: files.Length);

    foreach (string path in files)
    {
      string name = Path.GetFileNameWithoutExtension(path);
      string extension = Path.GetExtension(path);

      string type = _contentTypes[extension];
      string text = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);

      contents[name] = new(type, text);
    }

    return contents;
  }
}
