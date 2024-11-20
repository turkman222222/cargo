using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class pol : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на вашу строку подключения

        public pol()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles(); // Загружаем роли при инициализации формы
        }

        private void LoadUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.id_polzov, p.name, p.log, p.password, p.mail, r.name AS rol_id " +
                               "FROM dbo.polzov p " +
                               "INNER JOIN dbo.ROL r ON p.rol_id = r.id;"; // Исправлено на r.id

                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Установка заголовков столбцов на русский
                        dataGridView1.Columns["id_polzov"].HeaderText = "ID";
                        dataGridView1.Columns["name"].HeaderText = "Имя";
                        dataGridView1.Columns["log"].HeaderText = "Логин";
                        dataGridView1.Columns["password"].HeaderText = "Пароль";
                        dataGridView1.Columns["rol_id"].HeaderText = "Роль";
                        dataGridView1.Columns["mail"].HeaderText = "Email";

                        // Сделать столбец 'id_polzov' только для чтения
                        dataGridView1.Columns["id_polzov"].ReadOnly = true;
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

        private void LoadRoles()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name FROM dbo.ROL;";

                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "name"; // Отображаемое значение
                        comboBox1.ValueMember = "id"; // Значение, которое будет использоваться
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
            // Проверка на заполненность полей
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            int rol_id = (int)comboBox1.SelectedValue; // Получаем значение из ComboBox

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.polzov (name, log, password, rol_id, mail) VALUES (@name, @log, @password, @rol_id, @mail); SELECT SCOPE_IDENTITY();";

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", textBox1.Text);
                        command.Parameters.AddWithValue("@log", textBox2.Text);
                        command.Parameters.AddWithValue("@password", textBox3.Text);
                        command.Parameters.AddWithValue("@rol_id", rol_id);
                        command.Parameters.AddWithValue("@mail", textBox5.Text);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        MessageBox.Show($"Пользователь добавлен с ID: {newId}.", "Успех");
                        LoadUsers();
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
                MessageBox.Show("Выберите пользователя для редактирования.", "Ошибка");
                return;
            }

            // Проверка на заполненность полей
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            int id_polzov = (int)dataGridView1.SelectedRows[0].Cells["id_polzov"].Value;
            int rol_id = (int)comboBox1.SelectedValue; // Получаем значение из ComboBox

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.polzov SET name = @name, log = @log, password = @password, rol_id = @rol_id, mail = @mail WHERE id_polzov = @id_polzov;";
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_polzov", id_polzov);
                        command.Parameters.AddWithValue("@name", textBox1.Text);
                        command.Parameters.AddWithValue("@log", textBox2.Text);
                        command.Parameters.AddWithValue("@password", textBox3.Text);
                        command.Parameters.AddWithValue("@rol_id", rol_id);
                        command.Parameters.AddWithValue("@mail", textBox5.Text);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Пользователь изменен.", "Успех");
                        LoadUsers();
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
                MessageBox.Show("Выберите пользователя для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить пользователя?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id_polzov = (int)dataGridView1.SelectedRows[0].Cells["id_polzov"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.polzov WHERE id_polzov = @id_polzov;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id_polzov", id_polzov);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Пользователь удален.", "Успех");
                            LoadUsers();
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
            textBox5.Clear();
            comboBox1.SelectedIndex = -1; // Сбрасываем выбор в ComboBox
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["name"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["log"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["password"].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells["mail"].Value.ToString();

                // Устанавливаем выбранную роль в ComboBox
                comboBox1.SelectedValue = dataGridView1.SelectedRows[0].Cells["rol_id"].Value;
            }
        }

        private void pol_Load(object sender, EventArgs e)
        {
            // Здесь можно добавить код, который нужно выполнить при загрузке формы
        }
    }
}