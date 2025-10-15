using HelloMaui.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace HelloMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// DI registrations
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.Race>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.Race>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/races.json")));
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.Background>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.Background>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/backgrounds.json")));
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.Profession>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.Profession>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/professions.json")));
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.SurvivalWay>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.SurvivalWay>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/survival_ways.json")));
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.Hobby>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.Hobby>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/hobbies.json")));
		builder.Services.AddSingleton(
			sp => new Data.Catalogs.JsonCatalogRepository<Domain.Entities.AttributeDef>(
				sp.GetRequiredService<ILogger<Data.Catalogs.JsonCatalogRepository<Domain.Entities.AttributeDef>>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/attributes.json")));

		builder.Services.AddSingleton<Domain.Contracts.IHeroDraftStore>(sp =>
			new Data.Drafts.InMemoryHeroDraftStore(
				sp.GetRequiredService<ILogger<Data.Drafts.InMemoryHeroDraftStore>>()));
		builder.Services.AddSingleton<Domain.Contracts.IAvailabilityEngine>(sp =>
			new Domain.Validation.AvailabilityEngine(
				sp.GetRequiredService<ILogger<Domain.Validation.AvailabilityEngine>>(),
				ct => FileSystem.OpenAppPackageFileAsync("catalogs/rules.availability.json")));

		builder.Logging.SetMinimumLevel(LogLevel.Debug);
		builder.Logging.AddDebug();
		builder.Logging.AddProvider(new FileLoggerProvider(
			ts =>
			{
#if WINDOWS
				var baseDir = AppContext.BaseDirectory;
#else
				var baseDir = FileSystem.AppDataDirectory;
#endif
				var logsDir = Path.Combine(baseDir, "logs");
				var fileName = $"app-{ts:yyyy-MM-dd}.log";
				return Path.Combine(logsDir, fileName);
			},
			LogLevel.Debug));

		// Global exception handler
		builder.Services.AddSingleton<HelloMaui.Infrastructure.Logging.GlobalExceptionHandler>();

		// Ensure the log file is created at startup (helps verify logging path)
		try
		{
#if WINDOWS
			var baseDir = AppContext.BaseDirectory;
#else
			var baseDir = FileSystem.AppDataDirectory;
#endif
			var logsDir = Path.Combine(baseDir, "logs");
			Directory.CreateDirectory(logsDir);
			var bootstrapPath = Path.Combine(logsDir, $"app-{DateTimeOffset.UtcNow:yyyy-MM-dd}.log");
			File.AppendAllText(bootstrapPath, $"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss.fff 'Z'} [Information] Bootstrap: Application starting{Environment.NewLine}");
		}
		catch { /* ignore bootstrap errors */ }

		var app = builder.Build();
		try
		{
			app.Services.GetRequiredService<HelloMaui.Infrastructure.Logging.GlobalExceptionHandler>().Initialize();
		}
		catch { }
		return app;
	}
}
