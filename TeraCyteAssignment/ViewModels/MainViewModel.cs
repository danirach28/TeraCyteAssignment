using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TeraCyteAssignment.Configuration;
using TeraCyteAssignment.Models;
using TeraCyteAssignment.Services;
using TeraCyteAssignment.Services.Interface;

namespace TeraCyteAssignment.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private readonly IAuthService? _authService;
        private readonly IDataPollingService? _pollingService;
        private readonly Credentials? _credentials;

        [ObservableProperty] private string _statusMessage = "Ready.";
        [ObservableProperty] private bool _isLoading = true;
        [ObservableProperty] private InferenceData? _currentData;
        [ObservableProperty] private ImageSource? _currentImage;

        public MainViewModel(IAuthService authService, IDataPollingService pollingService, Credentials credentials)
        {
            _authService = authService;
            _pollingService = pollingService;
            _credentials = credentials;

            // Null checks to satisfy the compiler since the parameterless constructor doesn't initialize these.
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

        private void OnNewDataReceived(InferenceData data)
        {
            StatusMessage = $"Successfully loaded data for Image ID: {data.ImageId}";
            try
            { 
                using var stream = new MemoryStream(data.imageBytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                CurrentImage = bitmap;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Failed to decode image: {ex.Message} Retying");
                return;
                //CurrentImage = null;
            }

            //HistogramSeries.Clear();
            //HistogramSeries.Add(new ColumnSeries<int>
            //{
            //    Values = data.Histogram,
            //    Name = "Count",
            //    Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            //});
            CurrentData = data;
        }

        private void OnErrorOccurred(string errorMessage)
        {
            StatusMessage = errorMessage;
            IsLoading = false;
        }
    }
}

