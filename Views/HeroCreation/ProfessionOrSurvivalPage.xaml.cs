using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

public partial class ProfessionOrSurvivalPage : ContentPage
{
    public string DraftId { get; set; } = string.Empty;

    public ProfessionOrSurvivalPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
        if (draftStore != null && string.IsNullOrWhiteSpace(DraftId))
        {
            var draftNew = await draftStore.GetOrCreateAsync(Guid.NewGuid().ToString("N"));
            DraftId = draftNew.DraftId;
        }
        var profRepo = Application.Current?.Handler?.MauiContext?.Services.GetService(
            typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.Profession>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.Profession>;
        var survRepo = Application.Current?.Handler?.MauiContext?.Services.GetService(
            typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.SurvivalWay>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.SurvivalWay>;

        if (draftStore == null || profRepo == null || survRepo == null)
            return;

        var draft = await draftStore.GetOrCreateAsync(DraftId);
        var requiresProfession = draft.BackgroundId != "street_child";
        Header.Text = requiresProfession ? "Выберите профессию" : "Выберите способ выживания";

        if (requiresProfession)
            ItemsView.ItemsSource = await profRepo.GetAllAsync(default);
        else
            ItemsView.ItemsSource = await survRepo.GetAllAsync(default);
    }

    private async void OnItemSelected(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore == null)
                return;

            var draft = await draftStore.GetOrCreateAsync(DraftId);
            var requiresProfession = draft.BackgroundId != "street_child";

            if (requiresProfession)
            {
                var selected = e.CurrentSelection?.FirstOrDefault() as Domain.Entities.Profession;
                if (selected == null) return;
                draft.ProfessionId = selected.Id;
                await draftStore.SaveAsync(draft);
                await Shell.Current.GoToAsync($"//HeroSteps/HeroStepHobbies?draftId={draft.DraftId}");
            }
            else
            {
                var selected = e.CurrentSelection?.FirstOrDefault() as Domain.Entities.SurvivalWay;
                if (selected == null) return;
                draft.SurvivalWayId = selected.Id;
                await draftStore.SaveAsync(draft);
                await Shell.Current.GoToAsync($"//HeroSteps/HeroStepHobbies?draftId={draft.DraftId}");
            }
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<ProfessionOrSurvivalPage>)) as ILogger<ProfessionOrSurvivalPage>;
            logger?.LogError(ex, "Failed to handle selection");
            await DisplayAlert("Ошибка", "Не удалось сохранить выбор", "OK");
        }
    }
}



