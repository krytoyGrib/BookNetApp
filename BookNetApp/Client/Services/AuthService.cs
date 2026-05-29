using Blazored.LocalStorage;
using BookNetApp.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace BookNetApp.Client.Services
{
    public interface IAuthService
    {
        Task<string> Register(UserRegisterDto request);
        Task<string> Login(UserLoginDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage; 
        private readonly AuthenticationStateProvider _authStateProvider; 

        public AuthService(HttpClient http,
                           ILocalStorageService localStorage,
                           AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<string> Register(UserRegisterDto request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                var tokenFromServer = await response.Content.ReadAsStringAsync();
                tokenFromServer = tokenFromServer.Trim('"');

                await _localStorage.SetItemAsync("authToken", tokenFromServer);

                await _authStateProvider.GetAuthenticationStateAsync();

                return "success";
            }
            return "error";
        }



        public async Task<string> Login(UserLoginDto request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                token = token.Trim('"');

                await _localStorage.SetItemAsync("authToken", token);
                await _authStateProvider.GetAuthenticationStateAsync();

                return "success";
            }

            return "error";
        }






    }
}
