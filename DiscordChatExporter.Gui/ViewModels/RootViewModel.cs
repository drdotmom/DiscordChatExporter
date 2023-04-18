using System;
using System.Threading.Tasks;
using DiscordChatExporter.Gui.Services;
using DiscordChatExporter.Gui.Utils;
using DiscordChatExporter.Gui.ViewModels.Components;
using DiscordChatExporter.Gui.ViewModels.Dialogs;
using DiscordChatExporter.Gui.ViewModels.Messages;
using DiscordChatExporter.Gui.ViewModels.Framework;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace DiscordChatExporter.Gui.ViewModels;

public class RootViewModel : Screen, IHandle<NotificationMessage>, IDisposable
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    public SnackbarMessageQueue Notifications { get; } = new(TimeSpan.FromSeconds(5));

    public DashboardViewModel Dashboard { get; }

    public RootViewModel(
        IViewModelFactory viewModelFactory,
        IEventAggregator eventAggregator,
        DialogManager dialogManager,
        SettingsService settingsService)
    {
        _viewModelFactory = viewModelFactory;
        _dialogManager = dialogManager;
        _settingsService = settingsService;

        eventAggregator.Subscribe(this);

        Dashboard = _viewModelFactory.CreateDashboardViewModel();

        DisplayName = $"{App.Name} v{App.VersionString}";
    }

    public void OnViewFullyLoaded()
    {
        return;
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        _settingsService.Load();

        // Sync theme with settings
        if (_settingsService.IsDarkModeEnabled)
        {
            App.SetDarkTheme();
        }
        else
        {
            App.SetLightTheme();
        }
    }

    protected override void OnClose()
    {
        base.OnClose();

        _settingsService.Save();
    }

    public void Handle(NotificationMessage message) =>
        Notifications.Enqueue(message.Text);

    public void Dispose() => Notifications.Dispose();
}