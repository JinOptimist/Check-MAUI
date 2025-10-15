using System.Text.Json;
using HelloMaui.Domain.Contracts;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Data.Catalogs;

public sealed class JsonCatalogRepository<T> : ICatalogRepository<T>
{
	private readonly Func<CancellationToken, Task<Stream>> _streamFactory;
	private readonly ILogger<JsonCatalogRepository<T>> _logger;
	private IReadOnlyList<T>? _cache;
	private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true
	};

	private sealed class Wrapper
	{
		public List<T> Items { get; set; } = new();
	}

	public JsonCatalogRepository(ILogger<JsonCatalogRepository<T>> logger, Func<CancellationToken, Task<Stream>> streamFactory)
	{
		_logger = logger;
		_streamFactory = streamFactory;
	}

	public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct)
	{
		if (_cache != null)
		{
			_logger.LogDebug("Catalog {CatalogType} served from cache with {Count} items", typeof(T).Name, _cache.Count);
			return _cache;
		}
		_logger.LogInformation("Loading catalog {CatalogType} from package resource", typeof(T).Name);
		try
		{
			await using var stream = await _streamFactory(ct);
			var wrapper = await JsonSerializer.DeserializeAsync<Wrapper>(stream, _jsonOptions, ct)
				?? new Wrapper();
			_cache = wrapper.Items;
			_logger.LogInformation("Catalog {CatalogType} loaded with {Count} items", typeof(T).Name, _cache.Count);
			return _cache;
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Loading catalog {CatalogType} was cancelled", typeof(T).Name);
			throw;
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Failed to deserialize catalog {CatalogType}", typeof(T).Name);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while loading catalog {CatalogType}", typeof(T).Name);
			throw;
		}
	}

	public async Task<T?> GetByIdAsync(string id, CancellationToken ct)
	{
		_logger.LogDebug("Fetching {CatalogType} by id {Id}", typeof(T).Name, id);
		var all = await GetAllAsync(ct);
		var prop = typeof(T).GetProperty("Id");
		var entity = all.FirstOrDefault(x => (string?)prop?.GetValue(x)! == id);
		if (entity == null)
			_logger.LogWarning("{CatalogType} with id {Id} not found", typeof(T).Name, id);
		else
			_logger.LogDebug("{CatalogType} with id {Id} found", typeof(T).Name, id);
		return entity;
	}
}


