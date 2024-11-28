using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace testbd
{
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
            LoadRequest();  // Для загрузки данных при старте окна
        }

        private void LoadRequest()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "SELECT requestID, orgTechModel, problemDescryption, requestStatus, startDate FROM Requests";
                DataTable requestDataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Создаем SqlDataAdapter для заполнения DataTable
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(requestDataTable); // Заполнение DataTable данными
                    }
                }

                // Привязка DataTable к DataGrid
                requestDataGrid.ItemsSource = requestDataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void requestDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика обработки выбора элемента в DataGrid
        }

        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            string model = ModelTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string status = StatusTextBox.Text.Trim();
            DateTime? startDate = StartDatePicker.SelectedDate;

            if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(status) || !startDate.HasValue)
            {
                MessageBox.Show("Заполните все поля для добавления заявки!");
                return;
            }

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "INSERT INTO Requests (orgTechModel, problemDescryption, requestStatus, startDate) VALUES (@model, @description, @status, @startDate)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@model", model);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@startDate", startDate.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Заявка успешно добавлена!");
                            LoadRequest(); // Обновляем данные в DataGrid
                            ClearInputFields();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить заявку.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}");
            }
        }

        private void ClearInputFields()
        {
            ModelTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            StatusTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = null;
        }

    }
}
