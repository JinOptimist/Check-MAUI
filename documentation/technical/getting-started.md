## Как запустить проект (Getting Started)

### Требования
- Windows 10 21H2 (19041) или новее
- .NET SDK 9.0.x (`dotnet --info`)
- Visual Studio 2022 17.10+ с рабочими нагрузками:
  - .NET Multi-platform App UI development (.NET MAUI)
  - Для Android: Android SDK/Emulator (устанавливается через VS Installer)
  - Для Windows: Windows 10 SDK 19041+
- Для iOS/MacCatalyst требуется macOS и Xcode (сборка с Windows недоступна)

### Клонирование
```bash
git clone <URL-репозитория>
cd Check-MAUI
```

### Установка MAUI workloads
Рекомендуется восстановить рабочие нагрузки через SDK:
```bash
dotnet workload restore
# при необходимости
dotnet workload install maui
```

### Сборка и запуск из Visual Studio
1. Откройте `HelloMaui.sln`.
2. Выберите целевую платформу (Android/Windows/iOS/MacCatalyst).
3. Нажмите Run (F5).

### Сборка и запуск из CLI
- Android (эмулятор/устройство):
```bash
dotnet build -t:Run -f net9.0-android
```
- Windows:
```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```
- iOS/MacCatalyst (на macOS):
```bash
dotnet build -t:Run -f net9.0-ios
dotnet build -t:Run -f net9.0-maccatalyst
```

### Частые проблемы
- Android: убедитесь, что запущен эмулятор и настроен ADB (`adb devices`).
- iOS: требуется Mac с Xcode и соответствующие профили подписания.
- Если workloads не установились, обновите VS Installer и повторите `dotnet workload install maui`.


