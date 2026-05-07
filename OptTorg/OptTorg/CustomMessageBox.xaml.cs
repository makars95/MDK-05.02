using System.Windows;

namespace OptTorg
{
    public partial class CustomMessageBox : Window
    {
        public enum MessageBoxType
        {
            Information,
            Warning,
            Error,
            Question
        }

        public enum MessageBoxResult
        {
            Ok,
            Yes,
            No
        }

        private MessageBoxResult _result = MessageBoxResult.No;

        public CustomMessageBox(string message, string title, MessageBoxType type)
        {
            InitializeComponent();

            TitleText.Text = title;
            MessageText.Text = message;

            switch (type)
            {
                case MessageBoxType.Information:
                    IconBlock.Text = "ℹ️";
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Warning:
                    IconBlock.Text = "⚠️";
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Error:
                    IconBlock.Text = "❌";
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Question:
                    IconBlock.Text = "❓";
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    _result = MessageBoxResult.No;
                    break;
            }
        }

        public new MessageBoxResult ShowDialog()
        {
            base.ShowDialog();
            return _result;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Ok;
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (OkButton.Visibility == Visibility.Visible)
            {
                _result = MessageBoxResult.Ok;
            }
            else
            {
                _result = MessageBoxResult.No;
            }
            this.Close();
        }
    }
}