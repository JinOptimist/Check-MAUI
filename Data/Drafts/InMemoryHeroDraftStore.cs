using HelloMaui.Domain;
using HelloMaui.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Data.Drafts;

public sealed class InMemoryHeroDraftStore : IHeroDraftStore
{
	private readonly Dictionary<string, HeroDraft> _drafts = new();
	private readonly ILogger<InMemoryHeroDraftStore> _logger;

	public InMemoryHeroDraftStore(ILogger<InMemoryHeroDraftStore> logger)
	{
		_logger = logger;
	}

	public Task<HeroDraft> GetOrCreateAsync(string draftId)
	{
		_logger.LogDebug("GetOrCreate draft with id {DraftId}", draftId);
		if (!_drafts.TryGetValue(draftId, out var draft))
		{
			draft = new HeroDraft { DraftId = draftId };
			_drafts[draftId] = draft;
			_logger.LogInformation("Created new draft with id {DraftId}", draftId);
		}
		return Task.FromResult(draft);
	}

	public Task SaveAsync(HeroDraft draft)
	{
		_logger.LogDebug("Saving draft {DraftId}", draft.DraftId);
		_drafts[draft.DraftId] = draft;
		return Task.CompletedTask;
	}

	public Task<IReadOnlyList<string>> ListDraftIdsAsync()
	{
		var ids = _drafts.Keys.ToList();
		_logger.LogDebug("Listing {Count} draft ids", ids.Count);
		return Task.FromResult<IReadOnlyList<string>>(ids);
	}
}


