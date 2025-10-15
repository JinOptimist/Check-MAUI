## Технологический стек

### Платформа и фреймворки
- **.NET**: 9.0 (SDK)
- **.NET MAUI**: мультиплатформенная UI (Android, iOS, MacCatalyst, Windows)

### Целевые фреймворки (Target Frameworks)
- `net9.0-android`
- `net9.0-ios`
- `net9.0-maccatalyst`
- `net9.0-windows10.0.19041.0`

### Язык и разметка
- **C# 12**
- **XAML** для UI и ресурсов

### Средства разработки
- Visual Studio 2022 17.10+
- .NET CLI (`dotnet`)

### Зависимости проекта
- Библиотеки MAUI, поставляемые с шаблоном (шрифты, изображения, стили)
- Плагины и внешние пакеты — отсутствуют/минимальны (см. `.csproj`)

### Ресурсы
- `Resources/Styles/Colors.xaml`, `Resources/Styles/Styles.xaml`
- `Resources/Fonts/` (OpenSans)
- `Resources/Images/`, `Resources/AppIcon/`, `Resources/Splash/`

### Поддерживаемые платформы
- Android 8.0+
- iOS 13+
- macOS 12+ (MacCatalyst)
- Windows 10 2004 (19041)+

### CI/CD (при наличии)
- Локальная сборка через Visual Studio и `dotnet`
- Интеграция с CI может быть добавлена позднее (GitHub Actions/Azure DevOps)


