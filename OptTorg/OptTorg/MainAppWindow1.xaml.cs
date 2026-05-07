using System;
using System.Windows;
using System.Windows.Input;
using OptTorg.ViewModels;
using OptTorg.Models;
using OptTorg.Services;

namespace OptTorg
{
    public partial class MainAppWindow1 : Window
    {
        public MainAppWindow1(Polzovateli user)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(user);

            UpdateThemeButton();

            UpdateMaximizeButton();
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeService.ToggleTheme();
            UpdateThemeButton();
        }

        private void UpdateThemeButton()
        {
            if (ThemeToggleButton != null)
            {
                ThemeToggleButton.IsChecked = ThemeService.CurrentTheme == ThemeService.AppTheme.Light;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            UpdateMaximizeButton();
        }

        private void UpdateMaximizeButton()
        {
            if (MaximizeButton != null)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    MaximizeButton.Content = "❐";
                    MaximizeButton.ToolTip = "Восстановить окно";
                }
                else
                {
                    MaximizeButton.Content = "□";
                    MaximizeButton.ToolTip = "Развернуть на весь экран";
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = AppMessages.ShowQuestion(
                "Вы действительно хотите выйти из системы?",
                "Подтверждение выхода");

            if (result == CustomMessageBox.MessageBoxResult.Yes)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = AppMessages.ShowQuestion(
                "Вы действительно хотите выйти из системы?",
                "Подтверждение выхода");

            if (result == CustomMessageBox.MessageBoxResult.Yes)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            if (hwnd != IntPtr.Zero)
            {
                NativeMethods.SendMessage(hwnd, 0x0112, new IntPtr(0xF008), IntPtr.Zero);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bool loginWindowExists = false;
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    loginWindowExists = true;
                    break;
                }
            }
            if (!loginWindowExists)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            UpdateMaximizeButton();
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}