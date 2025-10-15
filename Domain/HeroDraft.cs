using System.Collections.Generic;

namespace HelloMaui.Domain;

public sealed class HeroDraft
{
	public string DraftId { get; set; } = Guid.NewGuid().ToString();
	public string? RaceId { get; set; }
	public string? BackgroundId { get; set; }
	public string? ProfessionId { get; set; }
	public string? SurvivalWayId { get; set; }
	public HashSet<string> HobbyIds { get; } = new();
	public Dictionary<string, int> AttributesBase { get; } = new();
}


