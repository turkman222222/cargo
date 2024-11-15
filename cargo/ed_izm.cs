using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class ed_izm : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на вашу строку подключения

        public ed_izm()
        {
            InitializeComponent();
            LoadUnitsOfMeasurement();
        }

        private void LoadUnitsOfMeasurement()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, ed_iz FROM dbo.ed_iz;";

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
                        dataGridView1.Columns["ed_iz"].HeaderText = "Единица измерения";

                        // Make 'id' column read-only
                        dataGridView1.Columns["id"].ReadOnly = true;
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
            // Validate input
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Пожалуйста, введите единицу измерения.", "Ошибка");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.ed_iz (ed_iz) VALUES (@ed_iz); SELECT SCOPE_IDENTITY();";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ed_iz", textBox1.Text);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        MessageBox.Show($"Единица измерения добавлена с ID: {newId}.", "Успех");
                        LoadUnitsOfMeasurement();
                        textBox1.Clear();
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
                MessageBox.Show("Выберите единицу измерения для редактирования.", "Ошибка");
                return;
            }

            // Validate input
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Пожалуйста, введите единицу измерения.", "Ошибка");
                return;
            }

            int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.ed_iz SET ed_iz = @ed_iz WHERE id = @id;";
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@ed_iz", textBox1.Text);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Единица измерения изменена.", "Успех");
                        LoadUnitsOfMeasurement();
                        textBox1.Clear();
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
                MessageBox.Show("Выберите единицу измерения для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить единицу измерения?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.ed_iz WHERE id = @id;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Единица измерения удалена.", "Успех");
                            LoadUnitsOfMeasurement();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["ed_iz"].Value.ToString();
            }
        }
    }
}