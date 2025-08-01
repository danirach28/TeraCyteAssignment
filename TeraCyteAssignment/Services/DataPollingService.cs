using System;
using System.Collections.Generic;
using System.IO;
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
            _pollingTimer = new DispatcherTimer { Interval = TimeSpan.FromMicroseconds(500) };
            _pollingTimer.Tick += OnTimerTick;
        }

        //private async Task<InferenceData> GetInferenceResult()
        //{
        //    var imageResponse = await _apiService.GetImageAsync();
        //    return new InferenceData(
        //                    imageResponse.ImageId,
        //                    imageResponse.Base64ImageData,
        //                    resultsResponse.ClassificationLabel,
        //                    resultsResponse.FocusScore,
        //                    resultsResponse.IntensityAverage,
        //                    resultsResponse.Histogram
        //                );
        //}

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
                        ValidateImage(imageResponse.Base64ImageData);
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

        private void ValidateImage(string base64Image)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Image);
                // We can do a quick check to see if it's a valid image stream.
                // Creating the full BitmapImage isn't necessary here, just validating the data.
                using var stream = new MemoryStream(imageBytes);
                if (stream.Length == 0)
                {
                    throw new InvalidDataException("Decoded image stream is empty.");
                }
            }
            catch (FormatException ex)
            {
                // This catches invalid base64 characters.
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
