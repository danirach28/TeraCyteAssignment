using Polly;
using Polly.Retry;
using System;
using System.IO;
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
        private readonly AsyncRetryPolicy _retryPolicy;
        private string _lastImageId = string.Empty;

        public event Action<ImageResultPaireData>? NewDataReceived;
        public event Action<string>? ErrorOccurred;

        public DataPollingService(IApiService apiService, IAuthService authService)
        {
            _apiService = apiService;
            _authService = authService;

            _pollingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _pollingTimer.Tick += OnTimerTick;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        ErrorOccurred?.Invoke($"[Retry {retryCount}] Error: {exception.Message}. Retrying in {timeSpan.Seconds}s...");
                    });
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

                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var inferenceDataResult = await ExtractInferenceData();

                    if (inferenceDataResult.ImageId == _lastImageId)
                    {
                        return;
                    }

                    NewDataReceived?.Invoke(inferenceDataResult);
                });
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Failed to retrieve data after multiple retries: {ex.Message}");
            }
            finally
            {
                if (_authService.IsLoggedIn)
                {
                    _pollingTimer.Start();
                }
            }
        }

        private async Task<ImageResultPaireData> ExtractInferenceData()
        {
            var imageResponse = await _apiService.GetImageAsync();
            if (imageResponse == null) throw new InvalidOperationException("API returned a null image response.");

            var resultsResponse = await _apiService.GetResultsAsync();
            if (resultsResponse == null) throw new InvalidOperationException("API returned a null results response.");

            if (resultsResponse.ImageId != imageResponse.ImageId)
            {
                throw new InvalidOperationException($"Result ID '{resultsResponse.ImageId}' does not match Image ID '{imageResponse.ImageId}'. Retrying for matching data...");
            }

            var imageBytes = ValidateImage(imageResponse.Base64ImageData);
            return new ImageResultPaireData(
                        imageResponse.ImageId,
                        imageBytes,
                        resultsResponse.ClassificationLabel,
                        resultsResponse.FocusScore,
                        resultsResponse.IntensityAverage,
                        resultsResponse.Histogram
                    );
        }

        private byte[]? ValidateImage(string base64Image)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Image);
                if (imageBytes.Length == 0)
                {
                    throw new InvalidDataException("Decoded image stream is empty.");
                }
                return imageBytes;
            }
            catch (FormatException ex)
            {
                throw new InvalidDataException("Image data is not a valid base64 string.", ex);
            }
        }

        public void StartPolling()
        {
            _lastImageId = string.Empty;
            _pollingTimer.Start();
        }

        public void StopPolling() => _pollingTimer.Stop();
    }
}