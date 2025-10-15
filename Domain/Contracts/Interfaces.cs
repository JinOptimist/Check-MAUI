using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelloMaui.Domain.Contracts;

public interface ICatalogRepository<T>
{
	Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
	Task<T?> GetByIdAsync(string id, CancellationToken ct);
}

public interface IHeroDraftStore
{
	Task<HeroDraft> GetOrCreateAsync(string draftId);
	Task SaveAsync(HeroDraft draft);
	Task<IReadOnlyList<string>> ListDraftIdsAsync();
}

public interface IAvailabilityEngine
{
	AvailabilityResult Evaluate(HeroDraft draft);
}

public sealed class AvailabilityResult
{
	public Dictionary<string, string[]> BlockedByReason { get; } = new();
	public Dictionary<string, bool> StepValid { get; } = new();
}


