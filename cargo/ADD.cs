using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class ADD : Form
    {
        private string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=\"111111111111 (1)\";Integrated Security=True"; // Замените на свои параметры подключения

        public ADD()
        {
            InitializeComponent();
            LoadCategories();
            LoadUnits();
            LoadPosts();
            LoadProducts(); // Загружаем данные в DataGridView при инициализации
        }

        private void LoadCategories()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT id, name FROM kat", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(new { Text = reader["name"].ToString(), Value = reader["id"] });
                        }
                    }
                }
                comboBox1.DisplayMember = "Text";
                comboBox1.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message);
            }
        }

        private void LoadUnits()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT id, ed_iz FROM ed_iz", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(new { Text = reader["ed_iz"].ToString(), Value = reader["id"] });
                        }
                    }
                }
                comboBox2.DisplayMember = "Text";
                comboBox2.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки единиц измерения: " + ex.Message);
            }
        }

        private void LoadPosts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT id, name FROM post", connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            comboBox3.Items.Add(new { Text = reader["name"].ToString(), Value = reader["id"] });
                        }
                    }
                }
                comboBox3.DisplayMember = "Text";
                comboBox3.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки постов: " + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT dt.id, k.name AS Category, dt.name_2, ei.ed_iz AS Unit, dt.col_sk, dt.cost, p.name AS Post FROM drop_table dt JOIN kat k ON dt.catecoria = k.id JOIN ed_iz ei ON dt.ed_izm = ei.id JOIN post p ON dt.post_id = p.id", connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;

                        // Изменение заголовков колонок на русский
                        dataGridView1.Columns["Category"].HeaderText = "Категория";
                        dataGridView1.Columns["name_2"].HeaderText = "Название";
                        dataGridView1.Columns["Unit"].HeaderText = "Единица измерения";
                        dataGridView1.Columns["col_sk"].HeaderText = "Количество";
                        dataGridView1.Columns["cost"].HeaderText = "Стоимость";
                        dataGridView1.Columns["Post"].HeaderText = "Пост";
                        dataGridView1.Columns["id"].HeaderText = "ID"; // Если нужно, можете оставить или изменить
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продуктов: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                comboBox1.SelectedValue = row.Cells["Category"].Value;
                textBox2.Text = row.Cells["name_2"].Value.ToString();
                comboBox2.SelectedValue = row.Cells["Unit"].Value;
                textBox4.Text = row.Cells["col_sk"].Value.ToString();
                textBox5.Text = row.Cells["cost"].Value.ToString();
                comboBox3.SelectedValue = row.Cells["Post"].Value;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO drop_table (catecoria, name_2, ed_izm, col_sk, cost, post_id) VALUES (@catecoria, @name_2, @ed_izm, @col_sk, @cost, @post_id)", connection))
                    {
                        command.Parameters.AddWithValue("@catecoria", comboBox1.SelectedValue);
                        command.Parameters.AddWithValue("@name_2", textBox2.Text);
                        command.Parameters.AddWithValue("@ed_izm", comboBox2.SelectedValue);
                        command.Parameters.AddWithValue("@col_sk", int.Parse(textBox4.Text));
                        command.Parameters.AddWithValue("@cost", float.Parse(textBox5.Text));
                        command.Parameters.AddWithValue("@post_id", comboBox3.SelectedValue);

                        command.ExecuteNonQuery();
                    }
                }
                LoadProducts(); // Обновляем данные в DataGridView после добавления
                MessageBox.Show("Запись успешно добавлена!");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Ошибка базы данных: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Пожалуйста, выберите товар для обновления.");
                        return;
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE drop_table SET catecoria = @catecoria, name_2 = @name_2, ed_izm = @ed_izm, col_sk = @col_sk, cost = @cost, post_id = @post_id WHERE id = @id;", connection))
                        {
                        command.Parameters.AddWithValue("@catecoria", comboBox1.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@name_2", textBox2.Text);
                        command.Parameters.AddWithValue("@ed_izm", comboBox2.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@col_sk", int.Parse(textBox4.Text));
                        command.Parameters.AddWithValue("@cost", float.Parse(textBox5.Text));
                        command.Parameters.AddWithValue("@post_id", comboBox3.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value));

                            command.ExecuteNonQuery();
                        }
                    }
                    LoadProducts(); // Обновляем данные в DataGridView после сохранения
                    MessageBox.Show("Запись успешно обновлена!");
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Ошибка базы данных: " + sqlEx.Message);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Пожалуйста, проверьте вводимые данные.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            

        }
    }
}

