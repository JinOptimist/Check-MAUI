using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

public partial class HobbySelectionPage : ContentPage
{
    public string DraftId { get; set; } = string.Empty;

    public HobbySelectionPage()
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
            typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.Hobby>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.Hobby>;
        if (repo != null)
        {
            HobbiesView.ItemsSource = await repo.GetAllAsync(default);
        }
    }

    private async void OnHobbiesChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore == null)
                return;

            var draft = await draftStore.GetOrCreateAsync(DraftId);

            draft.HobbyIds.Clear();
            foreach (var item in e.CurrentSelection)
            {
                if (item is Domain.Entities.Hobby h)
                    draft.HobbyIds.Add(h.Id);
            }
            await draftStore.SaveAsync(draft);
            await Shell.Current.GoToAsync($"//HeroSteps/HeroStepAttributes?draftId={draft.DraftId}");
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<HobbySelectionPage>)) as ILogger<HobbySelectionPage>;
            logger?.LogError(ex, "Failed to update hobbies");
            await DisplayAlert("Ошибка", "Не удалось сохранить хобби", "OK");
        }
    }
}



