using System.Data.Entity;  // ВАЖНО: добавить для Include с лямбдой
using System.Linq;
using OptTorg.Data;
using OptTorg.Models;

namespace OptTorg.Services
{
    public class AuthService
    {
        private readonly OptTorgDbContext _context;

        public AuthService()
        {
            _context = new OptTorgDbContext();
        }

        public Polzovateli Authenticate(string login, string password)
        {
            try
            {
                // Работает благодаря using System.Data.Entity;
                var authorization = _context.Avtorizaciya
                    .Include(a => a.Polzovatel)
                    .FirstOrDefault(a => a.Login == login && a.Parol == password);

                return authorization?.Polzovatel;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка авторизации: {ex.Message}");
                return null;
            }
        }

        public bool CheckConnection()
        {
            try
            {
                return _context.Database.Exists();
            }
            catch
            {
                return false;
            }
        }
    }
}