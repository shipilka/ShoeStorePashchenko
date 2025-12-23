using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ShoeStoreWpfApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        private readonly ShoeStoreContext _context;
        public string _userRole;

        public string UserRole { get; set; }

        public ProductsWindow(string userRole)
        {
            UserRole = userRole;
            _userRole = userRole;
            InitializeComponent();

            DataContext = this;

            _context = new ShoeStoreContext();
            LoadManufacturers(); // Загружаем производителей
            LoadProducts();

            // Отображение кнопки в зависимости от роли
            if (_userRole == "Администратор" || _userRole == "Менеджер")
            {
                AddEditProductButton.Visibility = Visibility.Visible;
            }
            else
            {
                AddEditProductButton.Visibility = Visibility.Collapsed;
            }

        }

        private void LoadManufacturers()
        {
            var manufacturers = _context.Manufacturers.ToList();

            ManufacturerComboBox.Items.Clear(); // Очищаем предыдущие элементы
            ManufacturerComboBox.Items.Add(new ComboBoxItem { Content = "Все производители", Tag = "All" });

            foreach (var manufacturer in manufacturers)
            {
                ManufacturerComboBox.Items.Add(new ComboBoxItem { Content = manufacturer.ManufacturerName, Tag = manufacturer.ManufacturerId });
            }

            ManufacturerComboBox.SelectedIndex = 0; // Устанавливаем первый элемент по умолчанию
        }

        private void LoadProducts(string searchTerm = null)
        {
            var productsQuery = _context.Products
        .Include(p => p.Manufacturer)  // Подключение производителя
        .Include(p => p.ProductName)    // Подключение ProductName
        .Include(p => p.Category)    // Подключение категорий
        .Include(p => p.Supplier)    // Подключение поставщика
        .Include(p => p.UnitOfMeasure)    // Подключение ед.измерения
        .AsQueryable();

            // Фильтрация по введенному запросу
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "Поиск по описанию")
            {
                productsQuery = productsQuery.Where(p =>
                    (p.ProductName.ProductName != null && p.ProductName.ProductName.Contains(searchTerm)) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(searchTerm)));
            }

            // Фильтрация по производителю
            var selectedManufacturer = ManufacturerComboBox.SelectedItem as ComboBoxItem;
            if (selectedManufacturer != null && selectedManufacturer.Tag.ToString() != "All")
            {
                productsQuery = productsQuery.Where(p => p.ManufacturerId == (int)selectedManufacturer.Tag);
            }

            // Фильтрация по максимальной цене
            if (decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice) && maxPrice > 0)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            // Фильтрация по наличию скидки
            if (DiscountCheckBox.IsChecked == true)
            {
                productsQuery = productsQuery.Where(p => p.ActiveDiscount > 0);
            }

            // Фильтрация по наличию на складе
            if (InStockCheckBox.IsChecked == true)
            {
                productsQuery = productsQuery.Where(p => p.StockQuantity > 0);
            }

            // Сортировка
            switch ((SortByComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString())
            {
                case "Name":
                    productsQuery = productsQuery.OrderBy(p => p.ProductName);
                    break;
                case "Supplier":
                    productsQuery = productsQuery.OrderBy(p => p.Supplier.SupplierName);
                    break;
                case "Price":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "PriceDesc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
            }

            var products = productsQuery.ToList();
            ProductsItemsControl.ItemsSource = products;

        }

        private void UpdateProductList()
        {
            LoadProducts(); // Обновляем список товаров
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchTerm = SearchTermTextBox.Text;
            LoadProducts(searchTerm); // Фильтрация по введенному запросу
        }

        private void SearchTermTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTermTextBox.Text == "Поиск по описанию")
            {
                SearchTermTextBox.Text = "";
                SearchTermTextBox.Foreground = Brushes.Black; // Установка черного цвета текста
            }
        }

        private void SearchTermTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTermTextBox.Text))
            {
                SearchTermTextBox.Text = "Поиск по описанию";
                SearchTermTextBox.Foreground = Brushes.Gray; // Установка серого цвета текста
            }
        }

        public byte[] Photo { get; set; } // Массив байтов для фото

        public string ImagePath
        {
            get
            {
                // 1. Проверка на пустоту
                if (Photo == null || Photo.Length == 0)
                {
                    return "pack://application:,,,/Assets/picture.png"; // Заглушка
                }

                try
                {
                    // 2. Проверяем, являются ли первые байты заголовком изображения
                    // JPEG: FF D8 | PNG: 89 50 4E 47
                    bool isRawImage = (Photo.Length > 2 && Photo[0] == 0xFF && Photo[1] == 0xD8) ||
                                      (Photo.Length > 3 && Photo[0] == 0x89 && Photo[1] == 0x50);

                    if (isRawImage)
                    {
                        // Если это само изображение — возвращаем Base64 для WPF
                        return $"data:image/jpeg;base64,{Convert.ToBase64String(Photo)}";
                    }
                    else
                    {
                        // 3. Если это не заголовок картинки, значит это ПУТЬ (текст в байтах)
                        string decodedFileName = Encoding.UTF8.GetString(Photo);

                        // Формируем относительный путь, включая папку Images
                        string relativePath = System.IO.Path.Combine(decodedFileName);

                        // 4. Указываем полный путь
                        string localPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                        // Проверяем, существует ли файл по этому пути
                        if (File.Exists(localPath))
                        {
                            return localPath;
                        }
                    }
                }
                catch
                {
                    // В случае ошибки декодирования возвращаем заглушку
                }

                return "pack://application:,,,/Assets/picture.png"; // Заглушка
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем текстовое поле поиска
            SearchTermTextBox.Text = "Поиск по описанию";
            SearchTermTextBox.Foreground = Brushes.Gray;

            // Сброс значений ComboBox
            ManufacturerComboBox.SelectedIndex = 0; // Выбор "Все производители"

            // Очищаем текстовое поле для максимальной цены
            MaxPriceTextBox.Text = string.Empty;

            // Сбрасываем чекбоксы
            DiscountCheckBox.IsChecked = false;
            InStockCheckBox.IsChecked = false;

            // Сбрасываем сортировку
            SortByComboBox.SelectedIndex = -1; 

            // Загрузка всех продуктов без фильтров
            LoadProducts();
        }



        private void AddEditProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли роли у текущего пользователя
            if (_userRole == "Администратор" || _userRole == "Менеджер")
            {
                // Если роль подходит, открываем окно редактирования продукта
                ProductEditWindow editWindow = new ProductEditWindow();
                // Подписка на событие
                editWindow.ProductUpdated += UpdateProductList; // Подписка на событие обновления
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("У вас нет прав для доступа к этому действию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string productId = button.Tag.ToString();

                // Получаем продукт из базы данных
                var product = _context.Products.Include(p => p.ProductName)
                                               .Include(p => p.Manufacturer)
                                               .Include(p => p.Category)
                                               .Include(p => p.Supplier)
                                               .Include(p => p.UnitOfMeasure)
                                               .SingleOrDefault(p => p.ProductId == productId);

                if (product != null)
                {
                    ProductUpdateWindow updateWindow = new ProductUpdateWindow(product);
                    updateWindow.ProductUpdated += UpdateProductList; // Подписываемся на событие
                    updateWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Продукт не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем идентификатор товара из Tag кнопки
            var button = sender as Button;
            if (button == null) return;

            string productId = button.Tag.ToString(); // Получаем productId как строку

            // Подтверждаем удаление товара
            var result = MessageBox.Show("Вы уверены, что хотите удалить данный товар?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Находим товар в базе данных
                var product = _context.Products.Find(productId); // Ищем товар по строковому идентификатору
                if (product != null)
                {
                    // Удаляем товар
                    _context.Products.Remove(product);
                    _context.SaveChanges(); // Сохраняем изменения в базе данных

                    // Обновляем список товаров в интерфейсе
                    LoadProducts();
                    MessageBox.Show("Товар успешно удален.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}