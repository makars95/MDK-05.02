using OptTorg.Data;
using System.Windows;

namespace OptTorg
{
    public partial class App : Application
    {
        // Можно оставить пустым или добавить обработку исключений
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Проверка подключения (опционально)
             var context = new OptTorgDbContext();
             if (!context.Database.Exists())
             {
                 MessageBox.Show("Не удалось подключиться к базе данных");
            }
        }
    }
}