using OptTorg.Interfaces;
using OptTorg.Models;
using OptTorg.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OptTorg.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        private string _login;
        private string _errorMessage;
        private IGettingPassword _passwordProvider;
        private readonly AuthService _authService;

        public static Polzovateli CurrentUser { get; private set; }

        public AuthorizationViewModel()
        {
            _authService = new AuthService();
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ErrorMessage = string.Empty;
            }
        }

        public IGettingPassword PasswordProvider
        {
            get => _passwordProvider;
            set
            {
                _passwordProvider = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        private string Password => PasswordProvider?.GetPassword() ?? string.Empty;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool CanLogin => !string.IsNullOrWhiteSpace(Login) &&
                                 PasswordProvider != null &&
                                 !string.IsNullOrWhiteSpace(Password);

        public void SetPasswordProvider(IGettingPassword provider)
        {
            PasswordProvider = provider;
        }

        public bool LogIn()
        {
            try
            {
                var user = _authService.Authenticate(Login, Password);

                if (user != null)
                {
                    CurrentUser = user;
                    ErrorMessage = string.Empty;
                    return true;
                }

                ErrorMessage = "Неверный логин или пароль!";
                return false;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                return false;
            }
        }

        public static void LogOut()
        {
            CurrentUser = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}