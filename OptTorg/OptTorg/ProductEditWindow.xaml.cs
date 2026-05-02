using Microsoft.Win32;
using Npgsql;
using OptTorg.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OptTorg
{
    public partial class ProductEditWindow : Window
    {
        private Tovar _editingTovar;
        private bool _isEditMode;
        private string _connectionString = "Host=localhost;Port=5432;Database=postgres;Username=23ip1;Password=23ip1;";
        private string _selectedImagePath;

        public ProductEditWindow(Tovar tovar = null)
        {
            InitializeComponent();

            if (tovar != null && tovar.Tovar_id > 0)
            {
                _isEditMode = true;
                _editingTovar = tovar;
                TitleText.Text = "Редактирование товара";
                LoadCategories();
                LoadTovarData();
            }
            else
            {
                _isEditMode = false;
                _editingTovar = new Tovar
                {
                    Tovar_id = 0,
                    Edinica_izmereniya = "шт"
                };
                TitleText.Text = "Добавление товара";
                LoadCategories();
            }
        }

        private async void LoadCategories()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT \"Kategoriya_id\", \"Nazvanie\" FROM \"Kategoriya\" ORDER BY \"Nazvanie\"";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            CategoryComboBox.Items.Clear();
                            CategoryComboBox.Items.Add(new CategoryItem { Id = 0, Name = "Без категории" });

                            while (await reader.ReadAsync())
                            {
                                CategoryComboBox.Items.Add(new CategoryItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                CategoryComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTovarData()
        {
            if (_editingTovar == null) return;

            NaimenovanieTextBox.Text = _editingTovar.Naimenovanie;
            ArtikulTextBox.Text = _editingTovar.Artikul.ToString();
            ProizvoditelTextBox.Text = _editingTovar.Proizvoditel;
            OpisanieTextBox.Text = _editingTovar.Opisanie;

            // Выбираем единицу измерения
            for (int i = 0; i < EdinicaComboBox.Items.Count; i++)
            {
                if ((EdinicaComboBox.Items[i] as ComboBoxItem)?.Content.ToString() == _editingTovar.Edinica_izmereniya)
                {
                    EdinicaComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Выбираем категорию
            if (_editingTovar.Kategoriya_id > 0)
            {
                for (int i = 0; i < CategoryComboBox.Items.Count; i++)
                {
                    var item = CategoryComboBox.Items[i] as CategoryItem;
                    if (item != null && item.Id == _editingTovar.Kategoriya_id)
                    {
                        CategoryComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Показываем текущее изображение
            if (!string.IsNullOrEmpty(_editingTovar.ImagePath))
            {
                CurrentImageLabel.Visibility = Visibility.Visible;
                CurrentImageBorder.Visibility = Visibility.Visible;
                try
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_editingTovar.ImagePath, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    CurrentImage.Source = bitmap;
                }
                catch { }
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string projectImagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Products");
                    if (!Directory.Exists(projectImagesPath))
                        Directory.CreateDirectory(projectImagesPath);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(openFileDialog.FileName);
                    string destPath = Path.Combine(projectImagesPath, fileName);
                    File.Copy(openFileDialog.FileName, destPath, true);

                    _selectedImagePath = $"/Resources/Images/Products/{fileName}";

                    // Обновляем превью
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(destPath, UriKind.Absolute);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImagePreview.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка копирования изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(NaimenovanieTextBox.Text))
            {
                MessageBox.Show("Введите наименование товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NaimenovanieTextBox.Focus();
                return;
            }

            decimal artikul = 0;
            if (!string.IsNullOrWhiteSpace(ArtikulTextBox.Text))
            {
                if (!decimal.TryParse(ArtikulTextBox.Text, out artikul))
                {
                    MessageBox.Show("Введите корректный артикул", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ArtikulTextBox.Focus();
                    return;
                }
            }

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "СОХРАНЕНИЕ...";

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string edinica = "шт";
                    if (EdinicaComboBox.SelectedItem is ComboBoxItem selectedEdinica)
                        edinica = selectedEdinica.Content.ToString();

                    int kategoriyaId = 0;
                    if (CategoryComboBox.SelectedItem is CategoryItem selectedCategory && selectedCategory.Id > 0)
                        kategoriyaId = selectedCategory.Id;

                    if (!_isEditMode || _editingTovar.Tovar_id == 0)
                    {
                        // ========== ТОЛЬКО ДОБАВЛЕНИЕ В ТАБЛИЦУ Tovari ==========
                        string insertTovarSql = @"
                            INSERT INTO ""Tovari"" 
                            (""Naimenovanie"", ""Artikul"", ""Opisanie"", ""Proizvoditel"", 
                             ""Edinica_izmereniya"", ""Kategoriya_id"", ""ImagePath"") 
                            VALUES (@naimenovanie, @artikul, @opisanie, @proizvoditel, 
                                    @edinica, @kategoriya_id, @imagePath)
                            RETURNING ""Tovar_id""";

                        using (NpgsqlCommand command = new NpgsqlCommand(insertTovarSql, connection))
                        {
                            command.Parameters.AddWithValue("@naimenovanie", NaimenovanieTextBox.Text.Trim());
                            command.Parameters.AddWithValue("@artikul", artikul);
                            command.Parameters.AddWithValue("@opisanie", OpisanieTextBox.Text.Trim() ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@proizvoditel", ProizvoditelTextBox.Text.Trim() ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@edinica", edinica);
                            command.Parameters.AddWithValue("@kategoriya_id", kategoriyaId == 0 ? (object)DBNull.Value : kategoriyaId);
                            command.Parameters.AddWithValue("@imagePath", _selectedImagePath ?? (object)DBNull.Value);

                            int newId = (int)await command.ExecuteScalarAsync();
                            _editingTovar.Tovar_id = newId;
                        }
                    }
                    else
                    {
                        // ========== ТОЛЬКО ОБНОВЛЕНИЕ В ТАБЛИЦЕ Tovari ==========
                        string updateTovarSql = @"
                            UPDATE ""Tovari"" SET 
                                ""Naimenovanie"" = @naimenovanie,
                                ""Artikul"" = @artikul,
                                ""Opisanie"" = @opisanie,
                                ""Proizvoditel"" = @proizvoditel,
                                ""Edinica_izmereniya"" = @edinica,
                                ""Kategoriya_id"" = @kategoriya_id,
                                ""ImagePath"" = @imagePath
                            WHERE ""Tovar_id"" = @id";

                        using (NpgsqlCommand command = new NpgsqlCommand(updateTovarSql, connection))
                        {
                            command.Parameters.AddWithValue("@naimenovanie", NaimenovanieTextBox.Text.Trim());
                            command.Parameters.AddWithValue("@artikul", artikul);
                            command.Parameters.AddWithValue("@opisanie", OpisanieTextBox.Text.Trim() ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@proizvoditel", ProizvoditelTextBox.Text.Trim() ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@edinica", edinica);
                            command.Parameters.AddWithValue("@kategoriya_id", kategoriyaId == 0 ? (object)DBNull.Value : kategoriyaId);
                            command.Parameters.AddWithValue("@imagePath", _selectedImagePath ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@id", _editingTovar.Tovar_id);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                MessageBox.Show("Товар успешно сохранен в каталоге!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505")
                {
                    MessageBox.Show("Ошибка: товар с таким артикулом уже существует.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = "💾 Сохранить";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}