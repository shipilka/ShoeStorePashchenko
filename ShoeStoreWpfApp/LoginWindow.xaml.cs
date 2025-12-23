using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreWpfApp.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShoeStoreWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ShoeStoreContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ShoeStoreContext();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = _context.Users
                .Include(u => u.UserRoles) // Включаем связанные роли
                .ThenInclude(ur => ur.Role) // Включаем роли
                .FirstOrDefault(u => u.Login == username && u.Password == password);

            if (user != null)
            {
                ResultTextBlock.Text = "Добро пожаловать!";

                var roleName = user.UserRoles.FirstOrDefault()?.Role.RoleName;

                if (roleName != null)
                {
                    // Передаем роль пользователя в ProductsWindow
                    var productsWindow = new ProductsWindow(roleName);
                    productsWindow.Show();
                    this.Close();
                }

                else
                {
                    ResultTextBlock.Text = "У этого пользователя нет ролей.";
                }
            }
            else
            {
                ResultTextBlock.Text = "Неправильный логин или пароль.";
            }
        }
    }
}