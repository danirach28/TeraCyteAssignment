using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TeraCyteAssignment.Models;
using TeraCyteAssignment.Services.Interface;

namespace TeraCyteAssignment.Services
{
    public class DataPollingService : IDataPollingService
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;
        private readonly DispatcherTimer _pollingTimer;
        private string _lastImageId = string.Empty;

        public event Action<InferenceData>? NewDataReceived;
        public event Action<string>? ErrorOccurred;

        public DataPollingService(IApiService apiService, IAuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
            _pollingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _pollingTimer.Tick += OnTimerTick;
        }

        private async void OnTimerTick(object? sender, EventArgs e)
        {
            _pollingTimer.Stop();
            try
            {
                if (!_authService.IsLoggedIn)
                {
                    ErrorOccurred?.Invoke("Authentication lost. Polling stopped.");
                    return;
                }

                var imageResponse = await _apiService.GetImageAsync();
                if (imageResponse != null && imageResponse.ImageId != _lastImageId)
                {
                    var resultsResponse = await _apiService.GetResultsAsync();
                    if (resultsResponse != null && resultsResponse.ImageId == imageResponse.ImageId)
                    {
                        _lastImageId = imageResponse.ImageId;
                        var newData = new InferenceData(
                            imageResponse.ImageId,
                            imageResponse.Base64ImageData,
                            resultsResponse.ClassificationLabel,
                            resultsResponse.FocusScore,
                            resultsResponse.IntensityAverage,
                            resultsResponse.Histogram
                        );
                        NewDataReceived?.Invoke(newData);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"An error occurred: {ex.Message} Retrying.");
            }
            _pollingTimer.Start();
        }

        public void StartPolling()
        {
            _lastImageId = string.Empty;
            _pollingTimer.Start();
        }

        public void StopPolling() => _pollingTimer.Stop();
    }

}
