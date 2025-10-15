## Архитектура

### Общая схема
Приложение построено на .NET MAUI с разделением на слои UI, представлений и ресурсов, с платформенно-специфическим кодом в папке `Platforms/`.

```
UI (XAML Pages)
  └── Code-behind (.xaml.cs)
MVVM (планируется/опционально)
  ├── ViewModels/
  └── Models/
Services/Data (при необходимости)
Resources (стили, шрифты, картинки)
Platforms (Android/iOS/MacCatalyst/Windows)
```

### Навигация
- Используется `AppShell` для декларативной навигации и маршрутов.

### Ресурсы и темы
- Общие ресурсы в `App.xaml` и `Resources/Styles/*`.
- Шрифты и изображения подключены через `Resources/Fonts` и `Resources/Images`.

### Платформенный код
- `Platforms/<Platform>/` содержит точки входа и специфичные настройки (манифесты, делегаты приложений, т.п.).

### Точки расширения
- Добавление слоев `ViewModels/`, `Models/`, `Services/` для перехода к полноценному MVVM.
- Регистрация зависимостей в `MauiProgram.cs` (DI контейнер `builder.Services`).

### Сборка и таргеты
- Проект таргетирует `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, `net9.0-windows10.0.19041.0`.


