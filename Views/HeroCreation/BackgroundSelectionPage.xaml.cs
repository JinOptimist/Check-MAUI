using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

public partial class BackgroundSelectionPage : ContentPage
{
    public string DraftId { get; set; } = string.Empty;

    public BackgroundSelectionPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var draftStoreInit = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
        if (draftStoreInit != null && string.IsNullOrWhiteSpace(DraftId))
        {
            var draft = await draftStoreInit.GetOrCreateAsync(Guid.NewGuid().ToString("N"));
            DraftId = draft.DraftId;
        }

        var repo = Application.Current?.Handler?.MauiContext?.Services.GetService(
            typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.Background>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.Background>;
        if (repo != null)
        {
            var items = await repo.GetAllAsync(default);
            BackgroundsView.ItemsSource = items;
        }
    }

    private async void OnBackgroundSelected(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as Domain.Entities.Background;
            if (selected == null)
                return;

            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore == null)
                return;

            var draft = await draftStore.GetOrCreateAsync(DraftId);
            draft.BackgroundId = selected.Id;
            // сбрасываем взаимоисключающие поля
            if (selected.Id == "street_child")
                draft.ProfessionId = null;
            else
                draft.SurvivalWayId = null;
            await draftStore.SaveAsync(draft);
            var nextRoute = selected.Id == "street_child" ? "HeroStepProfOrSurv" : "HeroStepProfOrSurv"; // оба ведут на общую страницу
            await Shell.Current.GoToAsync($"//HeroSteps/{nextRoute}?draftId={draft.DraftId}");
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<BackgroundSelectionPage>)) as ILogger<BackgroundSelectionPage>;
            logger?.LogError(ex, "Failed to handle background selection");
            await DisplayAlert("Ошибка", "Не удалось выбрать предысторию", "OK");
        }
    }
}



