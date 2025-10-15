using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

[QueryProperty(nameof(DraftId), "draftId")]
public partial class RaceSelectionPage : ContentPage
{
    public string DraftId { get; set; } = string.Empty;

    public RaceSelectionPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var racesRepo = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.Race>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.Race>;
        var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
        if (draftStore != null && string.IsNullOrWhiteSpace(DraftId))
        {
            // Если пришли напрямую через вкладку — создаём новый черновик
            var draft = await draftStore.GetOrCreateAsync(Guid.NewGuid().ToString("N"));
            DraftId = draft.DraftId;
        }
        if (racesRepo != null)
        {
            var races = await racesRepo.GetAllAsync(default);
            RacesView.ItemsSource = races;
        }
    }

    private async void OnRaceSelected(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as Domain.Entities.Race;
            if (selected == null)
                return;

            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore == null)
                return;

            var draft = await draftStore.GetOrCreateAsync(DraftId);
            draft.RaceId = selected.Id;
            await draftStore.SaveAsync(draft);
            await Shell.Current.GoToAsync($"//HeroSteps/HeroStepBackground?draftId={draft.DraftId}");
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<RaceSelectionPage>)) as ILogger<RaceSelectionPage>;
            logger?.LogError(ex, "Failed to handle race selection");
            await DisplayAlert("Ошибка", "Не удалось выбрать расу", "OK");
        }
    }
}


