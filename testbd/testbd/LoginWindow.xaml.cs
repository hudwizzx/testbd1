using System;
using System.Data.SqlClient;
using System.Windows;

namespace testbd
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "SELECT typeID FROM Users WHERE login = @login AND password = @password";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        
                        // Получаем роль пользователя
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            int roleId = Convert.ToInt32(result);

                            // Логика для открытия соответствующего окна
                            MessageBox.Show("Вы успешно вошли!");

                            // В зависимости от roleId открываем нужное окно
                            OpenRoleSpecificWindow(roleId);
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void OpenRoleSpecificWindow(int roleId)
        {
            // Открываем окно в зависимости от роли
            if (roleId == 1)
            {
                // Открыть окно администратора
                MessageBox.Show("Открыто окно Менеджер.");
                ManagerWindow managerWindow = new ManagerWindow();
                managerWindow.Show();
            }
            else if (roleId == 2)
            {
                // Открыть окно пользователя
                MessageBox.Show("Открыто окно Мастер.");
            }
            else if (roleId == 3)
            {
                // Открыть окно оператора
                MessageBox.Show("Открыто окно Оператор.");
            }
            else if (roleId == 4)
            {
                // Открыть окно оператора
                MessageBox.Show("Открыто окно Заказчик.");
            }
            else
            {
                MessageBox.Show("Неизвестная роль пользователя.");
            }
        }
    }
}
