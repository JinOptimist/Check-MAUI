using System.Text.Json;
using HelloMaui.Domain.Contracts;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Domain.Validation;

public sealed class AvailabilityEngine : IAvailabilityEngine
{
	private readonly Func<CancellationToken, Task<Stream>> _rulesStreamFactory;
	private readonly ILogger<AvailabilityEngine> _logger;

	private sealed class Rule
	{
		public string Code { get; set; } = string.Empty;
		public Dictionary<string, string> AppliesWhen { get; set; } = new();
		public Dictionary<string, string[]> Blocks { get; set; } = new();
	}

	private sealed class Wrapper { public List<Rule> Rules { get; set; } = new(); }

	private List<Rule>? _rules;

	public AvailabilityEngine(ILogger<AvailabilityEngine> logger, Func<CancellationToken, Task<Stream>> rulesStreamFactory)
	{
		_logger = logger;
		_rulesStreamFactory = rulesStreamFactory;
	}

	public AvailabilityResult Evaluate(HeroDraft draft)
	{
		_logger.LogDebug("Evaluating availability for draft {DraftId}", draft.DraftId);
		var rules = _rules ??= LoadRules();
		var result = new AvailabilityResult();
		foreach (var rule in rules)
		{
			if (IsApplicable(rule, draft))
			{
				foreach (var kv in rule.Blocks)
				{
					foreach (var blockedId in kv.Value)
					{
						var key = $"{kv.Key}:{blockedId}"; // e.g. profession:gladiator
						if (!result.BlockedByReason.TryGetValue(key, out var codes))
							result.BlockedByReason[key] = codes = Array.Empty<string>();
						result.BlockedByReason[key] = codes.Concat(new[] { rule.Code }).ToArray();
					}
				}
			}
		}
		// Basic step validity (filled where required)
		result.StepValid["Race"] = draft.RaceId != null;
		result.StepValid["Background"] = draft.BackgroundId != null;
		var requiresProfession = draft.BackgroundId != "street_child";
		result.StepValid["ProfessionOrSurvival"] = requiresProfession ? draft.ProfessionId != null : draft.SurvivalWayId != null;
		result.StepValid["Hobbies"] = draft.HobbyIds.Count > 0; // not strictly required, can be relaxed
		result.StepValid["Attributes"] = draft.AttributesBase.Count > 0;
		_logger.LogInformation("Availability evaluated for draft {DraftId}: Race={RaceId}, Bg={BgId}, Prof={ProfId}, Surv={SurvId}",
			draft.DraftId, draft.RaceId, draft.BackgroundId, draft.ProfessionId, draft.SurvivalWayId);
		return result;
	}

	private bool IsApplicable(Rule rule, HeroDraft draft)
	{
		foreach (var kv in rule.AppliesWhen)
		{
			var key = kv.Key;
			var value = kv.Value;
			switch (key)
			{
				case "race": if (draft.RaceId != value) return false; break;
				case "background": if (draft.BackgroundId != value) return false; break;
				case "profession": if (draft.ProfessionId != value) return false; break;
			}
		}
		return true;
	}

	private List<Rule> LoadRules()
	{
		_logger.LogInformation("Loading availability rules from package resource");
		try
		{
			using var stream = _rulesStreamFactory(CancellationToken.None).GetAwaiter().GetResult();
			var wrapper = JsonSerializer.Deserialize<Wrapper>(stream) ?? new Wrapper();
			_logger.LogInformation("Loaded {Count} availability rules", wrapper.Rules.Count);
			return wrapper.Rules;
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Failed to deserialize availability rules");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while loading availability rules");
			throw;
		}
	}
}


