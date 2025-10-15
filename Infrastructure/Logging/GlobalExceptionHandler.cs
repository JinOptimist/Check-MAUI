using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Infrastructure.Logging;

internal sealed class GlobalExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public void Initialize()
	{
		// AppDomain unhandled exceptions
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		// Task scheduler unobserved exceptions
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		// UI thread unhandled exceptions (where available)
#if WINDOWS
		try
		{
			var winApp = Microsoft.UI.Xaml.Application.Current;
			if (winApp != null)
			{
				winApp.UnhandledException += (sender, args) =>
				{
					try { _logger.LogCritical(args.Exception, "UI thread unhandled exception"); } catch { }
					args.Handled = true;
					SafeTerminate();
				};
			}
		}
		catch { }
#endif
#if ANDROID
		try
		{
			Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
			{
				try { _logger.LogCritical(e.Exception, "Android runtime unhandled exception"); } catch { }
				e.Handled = true; // we will terminate ourselves
				SafeTerminate();
			};
		}
		catch { }
#endif
	}

	private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			var ex = e.ExceptionObject as Exception;
			if (ex != null)
				_logger.LogCritical(ex, "AppDomain unhandled exception (IsTerminating={IsTerminating})", e.IsTerminating);
			else
				_logger.LogCritical("Non-exception unhandled error: {Error}", e.ExceptionObject);
		}
		catch { }
		finally
		{
			SafeTerminate();
		}
	}

	private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		try
		{
			_logger.LogCritical(e.Exception, "Unobserved task exception");
		}
		catch { }
		finally
		{
			e.SetObserved();
			SafeTerminate();
		}
	}

	private void SafeTerminate()
	{
		// Ensure the log gets flushed to the file. Our FileLogger writes synchronously,
		// but a tiny delay can help when system is under stress.
		try { System.Threading.Thread.Sleep(50); } catch { }

#if ANDROID
		try
		{
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
			Java.Lang.JavaSystem.Exit(0);
		}
		catch { }
#elif WINDOWS
		try
		{
			Application.Current?.Quit();
		}
		catch { }
#elif IOS || MACCATALYST
		// iOS/macOS apps are not allowed to programmatically exit; attempt to terminate gracefully
		try
		{
			UIApplication.SharedApplication.PerformSelector(new ObjCRuntime.Selector("terminate:"), UIApplication.SharedApplication, 0);
		}
		catch { }
#else
		try { Environment.Exit(-1); } catch { }
#endif
	}
}


