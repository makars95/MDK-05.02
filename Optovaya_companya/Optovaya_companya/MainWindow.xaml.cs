using Npgsql;
using Optovaya_companya.Models;
using Optovaya_companya.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Optovaya_companya
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=23ip1;Password=23ip1;";
        private int currentUserId = -1;
        private string currentUserLogin = "";


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Visibility == Visibility.Visible
                ? PasswordBox.Password
                : PasswordVisibleTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Пожалуйста, введите логин и пароль", true);
                return;
            }

            try
            {
                LoginButton.IsEnabled = false;
                LoginButton.Content = "ПРОВЕРКА...";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT ""Avtorizaciya_id"", ""Login"", ""Parol"" 
                        FROM ""Avtorizaciya"" 
                        WHERE ""Login"" = @login";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        using (NpgsqlDataReader reader = (NpgsqlDataReader)await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int userId = reader.GetInt32(0);
                                string dbLogin = reader.GetString(1);
                                string dbPassword = reader.GetString(2);

                                if (password == dbPassword)
                                {
                                    currentUserId = userId;
                                    currentUserLogin = dbLogin;

                                    ShowMessage($"Добро пожаловать, {dbLogin}!", false);
                                    await Task.Delay(500);

                                    OpenMainAppWindow();
                                }
                                else
                                {
                                    ShowMessage("Неверный пароль", true);
                                }
                            }
                            else
                            {
                                ShowMessage("Пользователь не найден", true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка подключения: {ex.Message}", true);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "ВОЙТИ";
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string surname = RegSurnameTextBox.Text.Trim();
            string name = RegNameTextBox.Text.Trim();
            string patronymic = RegPatronymicTextBox.Text.Trim();
            string email = RegEmailTextBox.Text.Trim();
            string phone = RegPhoneTextBox.Text.Trim();
            string login = RegLoginTextBox.Text.Trim();
            string password = RegPasswordBox.Visibility == Visibility.Visible
                ? RegPasswordBox.Password
                : RegPasswordVisibleTextBox.Text;
            string confirmPassword = ConfirmPasswordBox.Visibility == Visibility.Visible
                ? ConfirmPasswordBox.Password
                : ConfirmPasswordVisibleTextBox.Text;

            if (string.IsNullOrWhiteSpace(surname))
            {
                ShowMessage("Пожалуйста, введите фамилию", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowMessage("Пожалуйста, введите имя", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowMessage("Пожалуйста, введите электронную почту", true);
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowMessage("Пожалуйста, введите корректный email", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowMessage("Пожалуйста, введите телефон", true);
                return;
            }

            string phoneDigits = new string(phone.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != phone.Length)
            {
                ShowMessage("Телефон должен содержать только цифры", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Пожалуйста, заполните все поля", true);
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("Пароли не совпадают", true);
                return;
            }

            if (password.Length < 4)
            {
                ShowMessage("Пароль должен содержать минимум 4 символа", true);
                return;
            }

            if (login.Length < 3)
            {
                ShowMessage("Логин должен содержать минимум 3 символа", true);
                return;
            }

            try
            {
                RegisterButton.IsEnabled = false;
                RegisterButton.Content = "РЕГИСТРАЦИЯ...";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string checkSql = @"SELECT COUNT(*) FROM ""Avtorizaciya"" WHERE ""Login"" = @login";

                            using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkSql, connection))
                            {
                                checkCmd.Parameters.AddWithValue("@login", login);
                                long count = (long)await checkCmd.ExecuteScalarAsync();

                                if (count > 0)
                                {
                                    ShowMessage("Пользователь с таким логином уже существует", true);
                                    return;
                                }
                            }

                            string insertAuthSql = @"
                        INSERT INTO ""Avtorizaciya"" (""Login"", ""Parol"") 
                        VALUES (@login, @password)
                        RETURNING ""Avtorizaciya_id""";

                            int authId;
                            using (NpgsqlCommand insertAuthCmd = new NpgsqlCommand(insertAuthSql, connection))
                            {
                                insertAuthCmd.Parameters.AddWithValue("@login", login);
                                insertAuthCmd.Parameters.AddWithValue("@password", password);

                                authId = Convert.ToInt32(await insertAuthCmd.ExecuteScalarAsync());
                            }

                            string insertUserSql = @"
                        INSERT INTO ""Polzovateli"" 
                        (""Familiya"", ""Imya"", ""Otchestvo"", ""Email"", ""Telefon"", ""Avtorizaciya_id"") 
                        VALUES (@surname, @name, @patronymic, @email, @phone, @authId)";

                            using (NpgsqlCommand insertUserCmd = new NpgsqlCommand(insertUserSql, connection))
                            {
                                insertUserCmd.Parameters.AddWithValue("@surname", surname);
                                insertUserCmd.Parameters.AddWithValue("@name", name);
                                insertUserCmd.Parameters.AddWithValue("@patronymic", string.IsNullOrWhiteSpace(patronymic) ? (object)DBNull.Value : patronymic);
                                insertUserCmd.Parameters.AddWithValue("@email", email);
                                insertUserCmd.Parameters.AddWithValue("@phone", long.Parse(phone));
                                insertUserCmd.Parameters.AddWithValue("@authId", authId);

                                int rowsAffected = await insertUserCmd.ExecuteNonQueryAsync();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();

                                    ShowMessage("Регистрация успешна! Теперь вы можете войти.", false);

                                    RegSurnameTextBox.Text = "";
                                    RegNameTextBox.Text = "";
                                    RegPatronymicTextBox.Text = "";
                                    RegEmailTextBox.Text = "";
                                    RegPhoneTextBox.Text = "";
                                    RegLoginTextBox.Text = "";
                                    RegPasswordBox.Password = "";
                                    RegPasswordVisibleTextBox.Text = "";
                                    ConfirmPasswordBox.Password = "";
                                    ConfirmPasswordVisibleTextBox.Text = "";

                                    await Task.Delay(2000);
                                    ShowLoginPanel(null, null);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowMessage("Ошибка при регистрации: не удалось добавить данные пользователя", true);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                ShowMessage($"Ошибка БД: {ex.Message}", true);
                System.Diagnostics.Debug.WriteLine($"Детали: {ex}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка: {ex.Message}", true);
                System.Diagnostics.Debug.WriteLine($"Детали: {ex}");
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "ЗАРЕГИСТРИРОВАТЬСЯ";
            }
        }
        private void RegPhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void OpenMainAppWindow()
        {
            var user = new Polzovateli
            {
                Polzovatel_id = currentUserId,
                Familiya = currentUserLogin,
                Imya = ""
            };

            var mainWindow = new MainAppWindow(user);
            mainWindow.Show();
            this.Close();
        }

        private void ShowMessage(string message, bool isError)
        {
            MessageText.Text = isError ? $"❌ {message}" : $"✅ {message}";
            MessageText.Foreground = isError ?
                (Brush)this.FindResource("ErrorRed") :
                (Brush)this.FindResource("SuccessGreen");
            MessageText.Visibility = Visibility.Visible;

            Task.Delay(30000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => MessageText.Visibility = Visibility.Collapsed);
            });
        }

        private void ShowRegistrationPanel(object sender, MouseButtonEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            MessageText.Visibility = Visibility.Collapsed;

            RegSurnameTextBox.Text = "";
            RegNameTextBox.Text = "";
            RegPatronymicTextBox.Text = "";
            RegEmailTextBox.Text = "";
            RegPhoneTextBox.Text = "";
            RegLoginTextBox.Text = "";
            RegPasswordBox.Password = "";
            RegPasswordVisibleTextBox.Text = "";
            ConfirmPasswordBox.Password = "";
            ConfirmPasswordVisibleTextBox.Text = "";

            RegPasswordBox.Visibility = Visibility.Visible;
            RegPasswordVisibleTextBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordVisibleTextBox.Visibility = Visibility.Collapsed;
            ToggleRegPasswordButton.Content = "👁️";
            ToggleConfirmPasswordButton.Content = "👁️";
        }

        private void ShowLoginPanel(object sender, MouseButtonEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            MessageText.Visibility = Visibility.Collapsed;

            LoginTextBox.Text = "";
            PasswordBox.Password = "";
            PasswordVisibleTextBox.Text = "";

            PasswordBox.Visibility = Visibility.Visible;
            PasswordVisibleTextBox.Visibility = Visibility.Collapsed;
            TogglePasswordButton.Content = "👁️";
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PasswordBox.Visibility == Visibility.Visible)
                {
                    PasswordVisibleTextBox.Text = PasswordBox.Password;
                    PasswordVisibleTextBox.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Collapsed;
                    TogglePasswordButton.Content = "🙈";
                }
                else
                {
                    PasswordBox.Password = PasswordVisibleTextBox.Text;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordVisibleTextBox.Visibility = Visibility.Collapsed;
                    TogglePasswordButton.Content = "👁️";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в TogglePasswordVisibility: {ex.Message}");
            }
        }

        private void ToggleRegPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RegPasswordBox.Visibility == Visibility.Visible)
                {
                    RegPasswordVisibleTextBox.Text = RegPasswordBox.Password;
                    RegPasswordVisibleTextBox.Visibility = Visibility.Visible;
                    RegPasswordBox.Visibility = Visibility.Collapsed;
                    ToggleRegPasswordButton.Content = "🙈";
                }
                else
                {
                    RegPasswordBox.Password = RegPasswordVisibleTextBox.Text;
                    RegPasswordBox.Visibility = Visibility.Visible;
                    RegPasswordVisibleTextBox.Visibility = Visibility.Collapsed;
                    ToggleRegPasswordButton.Content = "👁️";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в ToggleRegPasswordVisibility: {ex.Message}");
            }
        }

        private void ToggleConfirmPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfirmPasswordBox.Visibility == Visibility.Visible)
                {
                    ConfirmPasswordVisibleTextBox.Text = ConfirmPasswordBox.Password;
                    ConfirmPasswordVisibleTextBox.Visibility = Visibility.Visible;
                    ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                    ToggleConfirmPasswordButton.Content = "🙈";
                }
                else
                {
                    ConfirmPasswordBox.Password = ConfirmPasswordVisibleTextBox.Text;
                    ConfirmPasswordBox.Visibility = Visibility.Visible;
                    ConfirmPasswordVisibleTextBox.Visibility = Visibility.Collapsed;
                    ToggleConfirmPasswordButton.Content = "👁️";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в ToggleConfirmPasswordVisibility: {ex.Message}");
            }
        }
    }
}