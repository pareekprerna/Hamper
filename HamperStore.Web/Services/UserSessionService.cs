using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HamperStore.Web.Services
{
    public class UserSessionService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        
        private bool _isAdmin;
        private string? _currentUserPhone;
        private string? _currentCustomerName;
        private bool _isInitialized;

        public bool IsAdmin => _isAdmin;
        public string? CurrentUserPhone => _currentUserPhone;
        public string? CurrentCustomerName => _currentCustomerName;

        public event Action? OnSessionChanged;

        public UserSessionService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            try
            {
                var adminResult = await _sessionStorage.GetAsync<bool>("IsAdmin");
                if (adminResult.Success) _isAdmin = adminResult.Value;

                var phoneResult = await _sessionStorage.GetAsync<string>("CurrentUserPhone");
                if (phoneResult.Success) _currentUserPhone = phoneResult.Value;

                var nameResult = await _sessionStorage.GetAsync<string>("CurrentCustomerName");
                if (nameResult.Success) _currentCustomerName = nameResult.Value;
                
                _isInitialized = true;
            }
            catch
            {
                // ProtectedSessionStorage will fail during prerendering or if JS is not ready.
                // This is expected and caught gracefully.
            }
        }

        public async Task LoginAsAdminAsync()
        {
            _isAdmin = true;
            _currentUserPhone = null;
            _currentCustomerName = "Store Admin";
            
            try
            {
                await _sessionStorage.SetAsync("IsAdmin", true);
                await _sessionStorage.SetAsync("CurrentUserPhone", "");
                await _sessionStorage.SetAsync("CurrentCustomerName", "Store Admin");
            }
            catch
            {
                // Graceful fallback
            }

            NotifyStateChanged();
        }

        public async Task LoginAsCustomerAsync(string phone, string name)
        {
            _isAdmin = false;
            _currentUserPhone = phone;
            _currentCustomerName = name;
            
            try
            {
                await _sessionStorage.SetAsync("IsAdmin", false);
                await _sessionStorage.SetAsync("CurrentUserPhone", phone);
                await _sessionStorage.SetAsync("CurrentCustomerName", name);
            }
            catch
            {
                // Graceful fallback
            }

            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            _isAdmin = false;
            _currentUserPhone = null;
            _currentCustomerName = null;
            
            try
            {
                await _sessionStorage.DeleteAsync("IsAdmin");
                await _sessionStorage.DeleteAsync("CurrentUserPhone");
                await _sessionStorage.DeleteAsync("CurrentCustomerName");
            }
            catch
            {
                // Graceful fallback
            }

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnSessionChanged?.Invoke();
    }
}
