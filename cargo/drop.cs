using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo
{
    public partial class drop : Form
    {
        private string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=\"111111111111 (1)\";Integrated Security=True";

        public drop()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        dt.id,
                        dt.catecoria AS [Категория],
                        dt.name_2 AS [Название],
                        dt.ed_izm AS [Ед. измерения],
                        dt.col_sk AS [Кол-во на складе],
                        dt.cost AS [Цена],
                        dt.post_id AS [Поставщик]
                    FROM dbo.drop_table dt";

                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Установка заголовков столбцов
                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["Название"].HeaderText = "Название";
                        dataGridView1.Columns["Ед. измерения"].HeaderText = "Ед. измерения";
                        dataGridView1.Columns["Кол-во на складе"].HeaderText = "Кол-во на складе";
                        dataGridView1.Columns["Цена"].HeaderText = "Цена";
                        dataGridView1.Columns["Поставщик"].HeaderText = "Поставщик";
                        dataGridView1.Columns["Категория"].HeaderText = "Категория";

                        // Сделать столбец 'id' только для чтения
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
            // Валидация ввода
            if (string.IsNullOrEmpty(textBox1.Text) || // catecoria
                string.IsNullOrEmpty(textBox2.Text) || // name_2
                string.IsNullOrEmpty(textBox3.Text) || // ed_izm
                string.IsNullOrEmpty(textBox4.Text) || // col_sk
                string.IsNullOrEmpty(textBox5.Text) || // cost
                string.IsNullOrEmpty(textBox6.Text))   // post_id
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            // Преобразование входных данных в правильные типы данных
            if (!int.TryParse(textBox1.Text, out int catecoria) ||
                !int.TryParse(textBox3.Text, out int ed_izm) ||
                !int.TryParse(textBox4.Text, out int col_sk) ||
                !decimal.TryParse(textBox5.Text, out decimal cost) ||
                !int.TryParse(textBox6.Text, out int post_id)) // post_id
            {
                MessageBox.Show("Неверный формат данных. Проверьте введенные значения.", "Ошибка");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.drop_table (catecoria, name_2, ed_izm, col_sk, cost, post_id) VALUES (@catecoria, @name_2, @ed_izm, @col_sk, @cost, @post_id); SELECT SCOPE_IDENTITY();";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@catecoria", catecoria);
                        command.Parameters.AddWithValue("@name_2", textBox2.Text); // name_2 - строка
                        command.Parameters.AddWithValue("@ed_izm", ed_izm);
                        command.Parameters.AddWithValue("@col_sk", col_sk);
                        command.Parameters.AddWithValue("@cost", cost);
                        command.Parameters.AddWithValue("@post_id", post_id);

                        int newId = Convert.ToInt32(command.ExecuteScalar()); // Получение нового ID
                        MessageBox.Show($"Товар добавлен с ID: {newId}.", "Успех");
                        LoadProducts();
                        ClearTextBoxes(); // Очистка полей ввода
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
            Editdrop gg = new Editdrop();
            gg.Show();

            
            

            
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить товар?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.drop_table WHERE id = @id;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Товар удален.", "Успех");
                            LoadProducts();
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
            textBox5.Clear();
            textBox6.Clear();
             // Очистка поля для ID
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Заполнить поля для редактирования
                
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Категория"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Название"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["Ед. измерения"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Кол-во на складе"].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells["Цена"].Value.ToString();
                textBox6.Text = dataGridView1.SelectedRows[0].Cells["Поставщик"].Value.ToString();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void drop_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
