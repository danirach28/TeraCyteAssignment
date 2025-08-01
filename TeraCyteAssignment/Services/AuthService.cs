using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeraCyteAssignment.Models;
using TeraCyteAssignment.Services.Interface;
using System.Net.Http;
using System.Net.Http.Json;

namespace TeraCyteAssignment.Services
{

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private string? _accessToken;
        private string? _refreshToken;

        public bool IsLoggedIn => !string.IsNullOrEmpty(_accessToken);

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest(username, password);
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
                if (!response.IsSuccessStatusCode) return false;

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse is null) return false;

                _accessToken = loginResponse.AccessToken;
                _refreshToken = loginResponse.RefreshToken;
                return true;
            }
            catch
            { 
                return false; 
            }
        }

        public Task<string?> GetAccessTokenAsync()
        {
            return Task.FromResult(_accessToken);
        }

        public async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken)) return false;
            try
            {
                var request = new RefreshRequest(_refreshToken);
                var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", request);
                if (!response.IsSuccessStatusCode)
                {
                    Logout();
                    return false;
                }

                var refreshResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (refreshResponse is null) return false;

                _accessToken = refreshResponse.AccessToken;
                _refreshToken = refreshResponse.RefreshToken;
                return true;
            }
            catch
            {
                Logout();
                return false;
            }
        }

        public void Logout()
        {
            _accessToken = null;
            _refreshToken = null;
        }
    }

}
