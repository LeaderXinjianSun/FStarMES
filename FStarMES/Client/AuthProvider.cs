using FStarMES.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace FStarMES.Client
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        public string UserName { get; set; }
        public AuthProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<UserDto>("api/Auth/GetUser");
            if (result?.Name == null)
            {
                MarkUserAsLoggedOut();
                return new AuthenticationState(new ClaimsPrincipal());
            }
            else
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name,result.Name));
                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims,"apiauth"));
                return new AuthenticationState(authenticatedUser);
            }
        }
        public void MarkUserAsAuthenticated(UserDto userDto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",userDto.Token);
            UserName = userDto.Name;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name,userDto.Name));
            claims.Add(new Claim("Admin","Admin"));

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        /// <summary>
        /// 标记注销
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            UserName = null;
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
