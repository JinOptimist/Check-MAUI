using Microsoft.Extensions.Logging;

namespace HelloMaui.Infrastructure.Logging;

internal sealed class FileLoggerProvider : ILoggerProvider
{
	private readonly Func<DateTimeOffset, string> _pathFactory;
	private readonly LogLevel _minLevel;

	public FileLoggerProvider(Func<DateTimeOffset, string> pathFactory, LogLevel minLevel)
	{
		_pathFactory = pathFactory;
		_minLevel = minLevel;
	}

	public ILogger CreateLogger(string categoryName)
		=> new FileLogger(categoryName, _pathFactory, _minLevel);

	public void Dispose()
	{
		// nothing to dispose
	}
}


