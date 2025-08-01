using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TeraCyteAssignment.Models;
using TeraCyteAssignment.Services.Interface;

namespace TeraCyteAssignment.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ApiService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<ImageResponse?> GetImageAsync()
        {
            return await GetWithRetryAsync<ImageResponse>("/api/image");
        }

        public async Task<ResultsResponse?> GetResultsAsync()
        {
            return await GetWithRetryAsync<ResultsResponse>("/api/results");
        }

        private async Task<T?> GetWithRetryAsync<T>(string requestUri) where T : class
        {
            if (!_authService.IsLoggedIn) throw new InvalidOperationException("Not logged in.");

            var token = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _authService.RefreshTokenAsync();
                if (refreshed)
                {
                    var newToken = await _authService.GetAccessTokenAsync();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                    response = await _httpClient.GetAsync(requestUri);
                }
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }

}
