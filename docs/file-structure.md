## MAUI Project File Structure

This project follows a clear MVVM-friendly layout with platform separation and resource organization.

### Root (bootstrap only)
- `App.xaml`, `App.xaml.cs` – application resources and startup
- `AppShell.xaml`, `AppShell.xaml.cs` – navigation shell
- `MauiProgram.cs` – DI and app builder
- `GlobalUsings.cs`, `*.csproj`

### UI
- `Views/Pages/` – XAML pages with their `.xaml.cs` code-behind
- `Views/Controls/` – reusable custom controls
- `Views/Templates/` – data templates
- `Views/Converters/` – `IValueConverter` implementations
- `Views/Behaviors/` – behaviors and triggers

### MVVM
- `ViewModels/` – one view model per page (e.g., `MainPageViewModel`)
- `Models/` – domain models / DTOs

### Services and Data
- `Services/` – app services (subfolders by domain like `Api/`, `Storage/`)
- `Data/` – repositories, local DB, persistence

### Resources
- `Resources/Styles/` – `Colors.xaml`, `Styles.xaml`, optional `Themes/`
- `Resources/Fonts/`, `Resources/Images/`, `Resources/Raw/`
- `Resources/AppIcon/`, `Resources/Splash/`

### Platforms
- `Platforms/<Platform>/` – platform-specific code (Android, iOS, MacCatalyst, Windows, Tizen)

### Namespaces and XAML
- Namespaces mirror folders (e.g., `HelloMaui.Views.Pages`)
- XAML `x:Class` uses the fully-qualified class (e.g., `HelloMaui.Views.Pages.MainPage`)
- When moving files, update `x:Class`, code-behind namespace, and Shell routes/`xmlns` aliases

### Conventions
- Keep pages and code-behind together
- Avoid XAML or page `.xaml.cs` in the project root
- Reflect any structure change in this document


