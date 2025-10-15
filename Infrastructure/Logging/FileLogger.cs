using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Infrastructure.Logging;

internal sealed class FileLogger : ILogger
{
	private readonly string _categoryName;
	private readonly Func<DateTimeOffset, string> _pathFactory;
	private readonly LogLevel _minLevel;
	private static readonly ConcurrentDictionary<string, object> _fileLocks = new();

	public FileLogger(string categoryName, Func<DateTimeOffset, string> pathFactory, LogLevel minLevel)
	{
		_categoryName = categoryName;
		_pathFactory = pathFactory;
		_minLevel = minLevel;
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel && logLevel != LogLevel.None;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel)) return;
		var now = DateTimeOffset.UtcNow;
		var filePath = _pathFactory(now);
		var line = FormatLine(now, logLevel, _categoryName, formatter(state, exception), exception);
		WriteLine(filePath, line);
	}

	private static string FormatLine(DateTimeOffset timestampUtc, LogLevel level, string category, string message, Exception? ex)
	{
		var ts = timestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff 'Z'");
		var text = $"{ts} [{level}] {category}: {message}";
		if (ex != null) text += $" | Exception: {ex}";
		return text + Environment.NewLine;
	}

	private static void WriteLine(string path, string line)
	{
		var dir = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
		var key = path;
		var sync = _fileLocks.GetOrAdd(key, _ => new object());
		lock (sync)
		{
			File.AppendAllText(path, line);
		}
	}
}


