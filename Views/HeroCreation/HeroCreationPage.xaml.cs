using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

public partial class HeroCreationPage : ContentPage
{
    public HeroCreationPage()
    {
        InitializeComponent();
    }

    private async void OnStartCreationClicked(object? sender, EventArgs e)
    {
        try
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<HeroCreationPage>)) as ILogger<HeroCreationPage>;
            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore is null)
            {
                await DisplayAlert("Ошибка", "Сервис черновиков недоступен", "OK");
                return;
            }

            var draftId = Guid.NewGuid().ToString("N");
            var draft = await draftStore.GetOrCreateAsync(draftId);
            logger?.LogInformation("Opened hero creation with draft {DraftId}", draft.DraftId);

            await Shell.Current.GoToAsync($"//HeroSteps/HeroStepRace?draftId={draft.DraftId}");
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<HeroCreationPage>)) as ILogger<HeroCreationPage>;
            logger?.LogError(ex, "Failed to start hero creation");
            await DisplayAlert("Ошибка", "Не удалось начать создание героя", "OK");
        }
    }
}


