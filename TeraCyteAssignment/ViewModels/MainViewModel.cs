using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TeraCyteAssignment.Configuration;
using TeraCyteAssignment.Models;
using TeraCyteAssignment.Services;
using TeraCyteAssignment.Services.Interface;

namespace TeraCyteAssignment.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService? _authService;
    private readonly IDataPollingService? _pollingService;
    private readonly Credentials? _credentials;


    [ObservableProperty] private string _statusMessage = "Ready.";
    [ObservableProperty] private bool _isLoading = true;

    [ObservableProperty]
    private ImageResultPairViewModel? _selectedHistoryItem;

    public ObservableCollection<ImageResultPairViewModel> History { get; } = new();

    public MainViewModel(IAuthService authService, IDataPollingService pollingService, Credentials credentials)
    {
        _authService = authService;
        _pollingService = pollingService;
        _credentials = credentials;

        if (_pollingService != null)
        {
            _pollingService.NewDataReceived += OnNewDataReceived;
            _pollingService.ErrorOccurred += OnErrorOccurred;
        }

        LoginCommand = new AsyncRelayCommand(LoginAndStartPollingAsync);
    }

    public IAsyncRelayCommand LoginCommand { get; }

    private async Task LoginAndStartPollingAsync()
    {
        if (_authService is null || _pollingService is null || _credentials is null) return;

        IsLoading = true;
        StatusMessage = "Authenticating...";
        bool success = await _authService.LoginAsync(_credentials.Username, _credentials.Password);
        if (success)
        {
            StatusMessage = "Authentication successful. Starting data polling...";
            _pollingService.StartPolling();
        }
        else
        {
            StatusMessage = "Authentication failed. Please check credentials or server connection.";
        }
        IsLoading = false;
    }

    private void OnNewDataReceived(ImageResultPaireData data)
    {
        StatusMessage = $"Successfully loaded data for Image ID: {data.ImageId}";

        Application.Current.Dispatcher.Invoke(() =>
        {
            StatusMessage = $"Successfully loaded data for Image ID: {data.ImageId}";

            var newInferenceVm = new ImageResultPairViewModel(data);
            History.Insert(0, newInferenceVm);
            SelectedHistoryItem = newInferenceVm;
        });
    }

    private void OnErrorOccurred(string errorMessage)
    {
        StatusMessage = errorMessage;
    }
}
