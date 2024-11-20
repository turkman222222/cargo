using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class Editdrop : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на свои параметры подключения

        public Editdrop()
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

        private void LoadDropTableData(int dropId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT * FROM drop_table WHERE id = @dropId", connection))
                    {
                        command.Parameters.AddWithValue("@dropId", dropId);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            textBox2.Text = reader["name_2"].ToString();
                            textBox4.Text = reader["col_sk"].ToString();
                            textBox5.Text = reader["cost"].ToString();
                            // Здесь можно добавить другие поля, если необходимо
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных таблицы: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                UpdateCategory((int)comboBox1.SelectedValue);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue != null)
            {
                UpdateUnit((int)comboBox2.SelectedValue);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedValue != null)
            {
                UpdatePost((int)comboBox3.SelectedValue);
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //"UPDATE drop_table SET catecoria = @catecoria, name_2 = @name2, ed_izm, col_sk, cost, post_id) VALUES (@catecoria, @name_2, @ed_izm, @col_sk, @cost, @post_id)", connection)
                    using (SqlCommand command = new SqlCommand("UPDATE drop_table SET catecoria = @catecoria, name_2 = @name_2, ed_izm = @ed_izm, col_sk = @col_sk, cost = @cost, post_id = @post_id WHERE id = @id;", connection))
                    {
                        command.Parameters.AddWithValue("@catecoria", comboBox1.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@name_2", textBox2.Text);
                        command.Parameters.AddWithValue("@ed_izm", comboBox2.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@col_sk", int.Parse(textBox4.Text));
                        command.Parameters.AddWithValue("@cost", float.Parse(textBox5.Text));
                        command.Parameters.AddWithValue("@post_id", comboBox3.SelectedIndex + 1);
                        command.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));

                        command.ExecuteNonQuery();
                    }
                }
                LoadProducts(); // Обновляем данные в DataGridView после сохранения
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

        private void UpdateCategory(int categoryId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE kat SET name = @name WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@name", comboBox1.Text);
                        command.Parameters.AddWithValue("@id", categoryId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления категории: " + ex.Message);
            }
        }

        private void UpdateUnit(int unitId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE ed_iz SET ed_iz = @ed_iz WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@ed_iz", comboBox2.Text);
                        command.Parameters.AddWithValue("@id", unitId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления единицы измерения: " + ex.Message);
            }
        }

        private void UpdatePost(int postId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE post SET name = @name WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@name", comboBox3.Text);
                        command.Parameters.AddWithValue("@id", postId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления поста: " + ex.Message);
            }
        }

        private void Editdrop_Load(object sender, EventArgs e)
        {

        }
    }
}

