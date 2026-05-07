using System.Windows;

namespace OptTorg
{
    public static class AppMessages
    {
        public static CustomMessageBox.MessageBoxResult ShowInfo(string message, string title = "Информация")
        {
            var dialog = new CustomMessageBox(message, title, CustomMessageBox.MessageBoxType.Information);
            return dialog.ShowDialog();
        }

        public static CustomMessageBox.MessageBoxResult ShowWarning(string message, string title = "Предупреждение")
        {
            var dialog = new CustomMessageBox(message, title, CustomMessageBox.MessageBoxType.Warning);
            return dialog.ShowDialog();
        }

        public static CustomMessageBox.MessageBoxResult ShowError(string message, string title = "Ошибка")
        {
            var dialog = new CustomMessageBox(message, title, CustomMessageBox.MessageBoxType.Error);
            return dialog.ShowDialog();
        }

        public static CustomMessageBox.MessageBoxResult ShowQuestion(string message, string title = "Подтверждение")
        {
            var dialog = new CustomMessageBox(message, title, CustomMessageBox.MessageBoxType.Question);
            return dialog.ShowDialog();
        }
    }
}