## Структура файлов проекта MAUI

Этот проект следует понятной структуре, совместимой с MVVM, с разделением по платформам и аккуратной организацией ресурсов.

### Корень (только загрузка приложения)
- `App.xaml`, `App.xaml.cs` — ресурсы приложения и стартовая инициализация
- `AppShell.xaml`, `AppShell.xaml.cs` — навигационная оболочка
- `MauiProgram.cs` — DI и конфигурация приложения
- `GlobalUsings.cs`, `*.csproj`

### UI
- `Views/Pages/` — XAML‑страницы с соответствующими файлами `.xaml.cs`
- `Views/Controls/` — повторно используемые пользовательские контролы
- `Views/Templates/` — шаблоны данных
- `Views/Converters/` — реализации `IValueConverter`
- `Views/Behaviors/` — поведения и триггеры

### MVVM
- `ViewModels/` — один ViewModel на страницу (например, `MainPageViewModel`)
- `Models/` — доменные модели / DTO

### Сервисы и данные
- `Services/` — сервисы приложения (подпапки по доменам, например, `Api/`, `Storage/`)
- `Data/` — репозитории, локальная БД, слои персистентности

### Ресурсы
- `Resources/Styles/` — `Colors.xaml`, `Styles.xaml`, при необходимости `Themes/`
- `Resources/Fonts/`, `Resources/Images/`, `Resources/Raw/`
- `Resources/AppIcon/`, `Resources/Splash/`

### Платформы
- `Platforms/<Platform>/` — платформенно-специфичный код (Android, iOS, MacCatalyst, Windows, Tizen)

### Пространства имён и XAML
- Пространства имён зеркалируют папки (например, `HelloMaui.Views.Pages`)
- В XAML `x:Class` указывает полностью квалифицированный тип (например, `HelloMaui.Views.Pages.MainPage`)
- При перемещении файлов обновляйте `x:Class`, пространства имён в код‑бихайнд, маршруты Shell и алиасы `xmlns`

### Конвенции
- Держите страницы и их `.xaml.cs` рядом
- Не размещайте XAML или `.xaml.cs` страниц в корне проекта
- Любые изменения структуры отражайте в этом документе


