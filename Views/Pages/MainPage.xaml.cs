using HelloMaui.Domain.Validation;
using Microsoft.Extensions.Logging;

namespace HelloMaui.Views.Pages;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly ILogger<MainPage> _logger;

    public MainPage(ILogger<MainPage> logger)
    {
        InitializeComponent();
        _logger = logger;
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("OnCounterClicked");
        count++;

        _logger.LogDebug($"count: {count}");

        if (count == 1)
        {
            CounterBtn.Text = $"Clicked {count} time";
        }
        else
        {
            CounterBtn.Text = $"Clicked {count} times";
        }

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}

