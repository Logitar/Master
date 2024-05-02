using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal class SeedDictionariesCommandHandler : INotificationHandler<SeedDictionariesCommand>
{
  private readonly IDictionaryClient _dictionaries;
  private readonly ILogger<SeedDictionariesCommandHandler> _logger;

  public SeedDictionariesCommandHandler(IDictionaryClient dictionaries, ILogger<SeedDictionariesCommandHandler> logger)
  {
    _dictionaries = dictionaries;
    _logger = logger;
  }

  public async Task Handle(SeedDictionariesCommand _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    SearchResults<Dictionary> results = await _dictionaries.SearchAsync(new SearchDictionariesPayload(), context);
    Dictionary<string, Dictionary> dictionaries = new(capacity: results.Items.Count);
    foreach (Dictionary dictionary in results.Items)
    {
      dictionaries[dictionary.Locale.Code] = dictionary;
    }

    string[] files = Directory.GetFiles("Dictionaries");
    foreach (string path in files)
    {
      string locale = Path.GetFileNameWithoutExtension(path);
      if (dictionaries.TryGetValue(locale, out Dictionary? dictionary))
      {
        _logger.LogInformation("The dictionary '{Locale}' already exists (Id={Id}).", dictionary.Locale.Code, dictionary.Id);
      }
      else
      {
        CreateDictionaryPayload payload = new(locale);
        dictionary = await _dictionaries.CreateAsync(payload, context);
        dictionaries[locale] = dictionary;

        string json = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
        Dictionary<string, string>? entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (entries != null)
        {
          ReplaceDictionaryPayload replace = new(locale);
          foreach (KeyValuePair<string, string> entry in entries)
          {
            replace.Entries.Add(new DictionaryEntry(entry));
          }
          dictionary = await _dictionaries.ReplaceAsync(dictionary.Id, replace, dictionary.Version, context)
            ?? throw new InvalidOperationException($"The dictionary 'Id={dictionary.Id}' replace result should not be null.");
        }

        _logger.LogInformation("The dictionary '{Locale}' has been created (Id={Id}).", dictionary.Locale.Code, dictionary.Id);
      }
    }
  }
}
