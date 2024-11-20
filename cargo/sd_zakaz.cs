using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class sd_zakaz : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на вашу строку подключения
        private int userId; // Идентификатор текущего пользователя

        public sd_zakaz(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Сохраняем идентификатор пользователя
            LoadCategoriesIntoComboBox();
        }

        private void LoadCategoriesIntoComboBox()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name FROM dbo.kat;";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                comboBoxCategories.DataSource = dt;
                                comboBoxCategories.DisplayMember = "name";
                                comboBoxCategories.ValueMember = "id";
                            }
                            else
                            {
                                MessageBox.Show("Нет доступных категорий.", "Информация");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void comboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategories.SelectedValue != null)
            {
                int selectedCategoryId = (int)comboBoxCategories.SelectedValue;
                LoadProductsIntoComboBox(selectedCategoryId);
            }
        }

        private void LoadProductsIntoComboBox(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name_2 FROM dbo.drop_table WHERE catecoria = @CategoryId;";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryId", categoryId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                comboBoxProducts.DataSource = dt;
                                comboBoxProducts.DisplayMember = "name_2";
                                comboBoxProducts.ValueMember = "id";
                            }
                            else
                            {
                                MessageBox.Show("Нет доступных продуктов для выбранной категории.", "Информация");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private int GetRandomCollectorId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 id FROM dbo.sbor ORDER BY NEWID();"; // Используем таблицу sbor
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object idObj = command.ExecuteScalar();

                    if (idObj != null && idObj != DBNull.Value)
                    {
                        return Convert.ToInt32(idObj);
                    }
                    else
                    {
                        throw new Exception("Не удалось получить случайный идентификатор сборщика.");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxProducts.SelectedValue != null && numericUpDownQuantity.Value > 0)
            {
                int selectedProductId = (int)comboBoxProducts.SelectedValue;
                int quantity = (int)numericUpDownQuantity.Value;

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        decimal productCost = GetProductCost(selectedProductId);

                        if (productCost <= 0)
                        {
                            MessageBox.Show($"Ошибка: Стоимость товара {selectedProductId} не найдена или некорректна.", "Ошибка");
                            return;
                        }

                        int randomCollectorId = GetRandomCollectorId(); // Получаем случайный идентификатор сборщика

                        string query = @"INSERT INTO zakaz (id_zak, drop_id, col, cost, sbor_id, date) VALUES (@id_zak, @drop_id, @col, @cost, @sbor_id, GETDATE()); SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id_zak", this.userId); // Используем userId
                            command.Parameters.AddWithValue("@drop_id", selectedProductId);
                            command.Parameters.AddWithValue("@col", quantity);
                            command.Parameters.AddWithValue("@cost", productCost);
                            command.Parameters.AddWithValue("@sbor_id", randomCollectorId); // Добавляем случайный идентификатор сборщика

                            int newOrderId = Convert.ToInt32(command.ExecuteScalar());
                            MessageBox.Show($"Заказ {newOrderId} успешно создан.", "Успех");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт и укажите количество.", "Ошибка");
            }
        }

        private decimal GetProductCost(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT cost FROM dbo.drop_table WHERE id = @ProductId;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    object costObj = command.ExecuteScalar();

                    if (costObj == null || costObj == DBNull.Value)
                    {
                        return 0;
                    }
                    return Convert.ToDecimal(costObj);
                }
            }
        }

        private void labelTotalCost_Click(object sender, EventArgs e)
        {
            // Этот метод можно оставить пустым или удалить, если не используется
        }

        private void sd_zakaz_Load(object sender, EventArgs e)
        {

        }
    }
}
