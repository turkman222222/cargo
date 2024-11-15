using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class category : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Replace with your connection string

        public category()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name FROM dbo.kat;";
                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Set column headers to Russian
                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["name"].HeaderText = "Название Категории";
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


        private void add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите имя категории.", "Ошибка");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.kat (name) VALUES (@name);";
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", textBox1.Text);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Категория добавлена.", "Успех");
                        LoadCategories(); // Refresh DataGridView
                        textBox1.Clear(); // Clear TextBox
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

        private void edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите категорию для редактирования.", "Ошибка");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите имя категории.", "Ошибка");
                return;
            }


            int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;
            string newName = textBox1.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.kat SET name = @newName WHERE id = @id;";
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newName", newName);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Категория изменена.", "Успех");
                        LoadCategories(); // Refresh DataGridView
                        textBox1.Clear(); // Clear TextBox
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

        private void delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите категорию для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить категорию?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.kat WHERE id = @id;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Категория удалена.", "Успех");
                            LoadCategories();
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
        }

        private void category_Load(object sender, EventArgs e)
        {

        }
    }
}