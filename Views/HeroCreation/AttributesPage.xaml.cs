using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.HeroCreation;

public partial class AttributesPage : ContentPage
{
    private const int TotalBudget = 75;
    public string DraftId { get; set; } = string.Empty;

    private sealed class AttributeItem : INotifyPropertyChanged
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private readonly ObservableCollection<AttributeItem> _items = new();

    public AttributesPage()
    {
        InitializeComponent();
        AttributesView.ItemsSource = _items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var attrsRepo = Application.Current?.Handler?.MauiContext?.Services.GetService(
            typeof(Data.Catalogs.JsonCatalogRepository<Domain.Entities.AttributeDef>)) as Data.Catalogs.JsonCatalogRepository<Domain.Entities.AttributeDef>;
        var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
        if (draftStore != null && string.IsNullOrWhiteSpace(DraftId))
        {
            var draftNew = await draftStore.GetOrCreateAsync(Guid.NewGuid().ToString("N"));
            DraftId = draftNew.DraftId;
        }
        if (attrsRepo == null || draftStore == null)
            return;

        var defs = await attrsRepo.GetAllAsync(default);
        var draft = await draftStore.GetOrCreateAsync(DraftId);
        _items.Clear();
        foreach (var d in defs)
        {
            draft.AttributesBase.TryGetValue(d.Id, out var val);
            _items.Add(new AttributeItem { Id = d.Id, Name = d.Name, Value = val });
        }
        UpdateBudgetLabel();
    }

    private async void PersistAsync()
    {
        try
        {
            var draftStore = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(Domain.Contracts.IHeroDraftStore)) as Domain.Contracts.IHeroDraftStore;
            if (draftStore == null) return;
            var draft = await draftStore.GetOrCreateAsync(DraftId);
            draft.AttributesBase.Clear();
            foreach (var it in _items)
                draft.AttributesBase[it.Id] = it.Value;
            await draftStore.SaveAsync(draft);
        }
        catch (Exception ex)
        {
            var logger = Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(ILogger<AttributesPage>)) as ILogger<AttributesPage>;
            logger?.LogError(ex, "Failed to persist attributes");
        }
    }

    private int CurrentSpent() => _items.Sum(i => i.Value);

    private void UpdateBudgetLabel()
    {
        var remaining = TotalBudget - CurrentSpent();
        BudgetLabel.Text = $"Осталось: {remaining}";
    }

    private void OnIncrementClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is AttributeItem item)
        {
            if (CurrentSpent() >= TotalBudget) return;
            item.Value += 1;
            UpdateBudgetLabel();
            PersistAsync();
        }
    }

    private void OnDecrementClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is AttributeItem item)
        {
            if (item.Value <= 0) return;
            item.Value -= 1;
            UpdateBudgetLabel();
            PersistAsync();
        }
    }
}



