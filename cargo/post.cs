using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class post : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на вашу строку подключения

        public post()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name, adress, phone_number, email FROM dbo.post;";

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
                        dataGridView1.Columns["name"].HeaderText = "Название";
                        dataGridView1.Columns["adress"].HeaderText = "Адрес";
                        dataGridView1.Columns["phone_number"].HeaderText = "Номер телефона";
                        dataGridView1.Columns["email"].HeaderText = "Email";

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
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.post (name, adress, phone_number, email) VALUES (@name, @adress, @phone_number, @email); SELECT SCOPE_IDENTITY();";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", textBox1.Text);
                        command.Parameters.AddWithValue("@adress", textBox2.Text);
                        command.Parameters.AddWithValue("@phone_number", textBox3.Text);
                        command.Parameters.AddWithValue("@email", textBox4.Text);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        MessageBox.Show($"Поставщик добавлен с ID: {newId}.", "Успех");
                        LoadSuppliers();
                        ClearTextBoxes();
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
                MessageBox.Show("Выберите поставщика для редактирования.", "Ошибка");
                return;
            }

            // Validate input
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.post SET name = @name, adress = @adress, phone_number = @phone_number, email = @email WHERE id = @id;";
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", textBox1.Text);
                        command.Parameters.AddWithValue("@adress", textBox2.Text);
                        command.Parameters.AddWithValue("@phone_number", textBox3.Text);
                        command.Parameters.AddWithValue("@email", textBox4.Text);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Поставщик изменен.", "Успех");
                        LoadSuppliers();
                        ClearTextBoxes();
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
                MessageBox.Show("Выберите поставщика для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить поставщика?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.post WHERE id = @id;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Поставщик удален.", "Успех");
                            LoadSuppliers();
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

        private void ClearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["name"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["adress"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["phone_number"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["email"].Value.ToString();
            }
        }

        private void post_Load(object sender, EventArgs e)
        {

        }
    }
}